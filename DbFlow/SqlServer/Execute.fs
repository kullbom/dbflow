module DbFlow.SqlServer.Execute

open DbFlow
open DbFlow.SqlServer.Schema
open DbFlow.SqlServer.Scripts.Generate
open Microsoft.Data.SqlClient

module Internal = 
    let scriptTransaction (script : ScriptContent) = 
        script
        |> ScriptContent.toString
        |> SqlParser.Batches.splitInSqlBatches 
        |> List.collect
            (fun sqlBatch ->
                [
                    "SET QUOTED_IDENTIFIER ON"
                    "SET ANSI_NULLS ON" 
                    sqlBatch 
                    "SET QUOTED_IDENTIFIER OFF"
                ])
        |> List.map  (fun s -> DbTr.nonQuery s [])
        |> IO.sequence_

    // https://learn.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-refreshsqlmodule-transact-sql?view=sql-server-ver17

    let refreshSqlModuleMetadata logger connection =
        let modules = SqlModule.readAll connection
        // For now - only update views
        // Functions can be refered to by check constraints and is then not possible to refresh ... :(
        DbTr.reader 
            "SELECT 
                o.name AS ObjectName,
                SCHEMA_NAME(o.schema_id) AS SchemaName
             FROM sys.sql_modules m
             INNER JOIN sys.objects o ON o.object_id = m.object_id
             WHERE m.is_schema_bound = 0 
               AND o.type = 'V '"
            []
            (fun acc r -> 
                let refreshScript = 
                    let objectName = Readers.readString "ObjectName" r
                    let schemaName = Readers.readString "SchemaName" r 
                    $"EXECUTE sp_refreshsqlmodule N'[{schemaName}].[{objectName}]'"
                refreshScript :: acc)
            []
        |> DbTr.commit_ connection
        |> IO.bind
            (fun refreshScripts -> 
                refreshScripts
                |> List.map (fun refreshScript -> refreshScript |> ScriptContent.single |> scriptTransaction)
                |> IO.sequence_ 
                |> DbTr.commit_ connection)

    let collectScriptsFromSchema (options : ScriptOptions) (sourceDb : DatabaseSchema) =
        Scripts.Generate.generateScripts options sourceDb
            (fun (settingsScripts, scripts) isDatabaseSettings script -> 
                if isDatabaseSettings 
                then script :: settingsScripts, scripts 
                else settingsScripts, script :: scripts)
            ([],[])

    let collectScriptsFromFolder (scriptsFolder : string)=
        let stripName =
            let fLength = scriptsFolder.Length
            fun (s : string) -> s.Substring fLength
        let rec collectAllScripts acc currentFolder =
            let acc' =
                System.IO.Directory.GetFiles currentFolder
                |> Array.fold   
                    (fun acc' f -> 
                        (stripName f, System.IO.File.ReadAllText f) :: acc')
                    acc
            System.IO.Directory.GetDirectories currentFolder
            |> Array.fold collectAllScripts acc'
        collectAllScripts [] scriptsFolder
        |> List.sortBy fst
        |> List.map snd

let readSchema' logger (options : ReadOptions) connection =
    // Ensure the current user has enough privileges to access the schema
    IO.io {
        do! DbTr.reader "SELECT IS_ROLEMEMBER('db_ddladmin') CanRead" []
                (fun acc r -> (Readers.readInt32 "CanRead" r = 1) :: acc) []
            |> DbTr.commit_ connection
            |> IO.map
                (function
                    | [true] -> ()
                    | r -> failwithf "Missing privileges to read schema") 
    
        do! if options.RefreshSqlModulesMetadata
            then 
                Internal.refreshSqlModuleMetadata logger connection
                |> Logger.logTimeIO logger "Refresh meta data" 
            else
                IO.ret ()

        return! DatabaseSchema.read logger options connection
    }

/// Read the schema of a database given a connection
let readSchema logger (options : ReadOptions) connection =
    readSchema' logger options connection |> IO.run

let readSchemaAsync logger (options : ReadOptions) connection =
    readSchema' logger options connection |> IO.runAsync


let clone' logger (options : ScriptOptions) (sourceDb : DatabaseSchema) (targetConnection : SqlConnection) =
    let (settingsScripts, collectedScripts) = 
        Internal.collectScriptsFromSchema options 
        |> Logger.logTime logger "Collect scripts" sourceDb
    
    let resolvedScripts =
        Dependent.resolveOrder (fun d -> d.Content) sourceDb.Dependencies
        |> Logger.logTime logger "Resolve scripts dependencies" collectedScripts

    IO.io {
        // The "settings script" can not be run as part of the same transaction as the other scripts
        do! settingsScripts
            |> List.map (fun script -> Internal.scriptTransaction script.Content.Content)
            |> IO.sequence_
            |> DbTr.exe targetConnection
            |> Logger.logTimeIO logger "Execute database setup scripts"
    
        do!
            resolvedScripts
            |> List.map (fun script -> Internal.scriptTransaction script.Content.Content) 
            |> IO.sequence_
            |> DbTr.commit_ targetConnection
            |> Logger.logTimeIO logger "Resolve and execute scripts"
            
        do SqlConnection.ClearPool targetConnection

        return ()
    }

let cloneToLocal' logger (options : ScriptOptions) (sourceDb : DatabaseSchema) =
    let localDb = new LocalTempDb(logger)
    IO.io {
        use conn = new Microsoft.Data.SqlClient.SqlConnection(localDb.ConnectionString)
        conn.Open ()
        do! clone' logger options sourceDb conn
        return localDb
    }


/// Clone a schema into a database given a target connection
let clone logger (options : ScriptOptions) (sourceDb : DatabaseSchema) (targetConnection : SqlConnection) =
    clone' logger options sourceDb targetConnection |> IO.run

let cloneAsync logger (options : ScriptOptions) (sourceDb : DatabaseSchema) (targetConnection : SqlConnection) =
    clone' logger options sourceDb targetConnection |> IO.runAsync

let cloneToLocal logger (options : ScriptOptions) (sourceDb : DatabaseSchema) =
    cloneToLocal' logger options sourceDb |> IO.run

let cloneToLocalAsync logger (options : ScriptOptions) (sourceDb : DatabaseSchema) =
    cloneToLocal' logger options sourceDb |> IO.runAsync


let generateScriptFiles' (opt : ScriptOptions) (schema : DatabaseSchema) directory =
    // TODO: "Move" requires the source and target to be on the same device. 
    //  Rewrite to use a "temp directory" in the same folder as the target directory... instead of GetTempPath
    let tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), (System.Guid.NewGuid ()).ToString().Replace("-", ""))
    Scripts.Generate.generateScripts opt schema
        (fun io _isDatabaseSettings script ->
            let subfolder = 
                match script.Content.Subdirectory with
                | Some sDir -> System.IO.Path.Combine(tempDir, sDir)
                | None -> tempDir
            if not <| System.IO.Directory.Exists subfolder
            then System.IO.Directory.CreateDirectory (subfolder) |> ignore
            
            let file = System.IO.Path.Combine(subfolder, script.Content.Filename)
            io |> IO.bind (fun () -> FileSystem.writeAllText file (script.Content.Content |> ScriptContent.toString)))
        (IO.ret ())
    |> IO.map
        (fun () -> 
            if System.IO.Directory.Exists directory
            then System.IO.Directory.Delete(directory,true)

            System.IO.Directory.Move(tempDir, directory))
    
/// Generate scripts of a schema to a folder structure
let generateScriptFiles (opt : ScriptOptions) (schema : DatabaseSchema) directory =
    generateScriptFiles' opt schema directory |> IO.run

let generateScriptFilesAsync (opt : ScriptOptions) (schema : DatabaseSchema) directory =
    generateScriptFiles' opt schema directory |> IO.runAsync

/// Schema compare. Compares two schemas returning a list of differences
let compare (d0 : DatabaseSchema) (d1 : DatabaseSchema) =
    CompareGen.Collect (d0, d1) [] []


let performDbUpgrade' logger connectionStr scriptFolder =
    let scripts = 
        Internal.collectScriptsFromFolder 
        |> Logger.logTime logger "Upgrade - Collect scripts in folder" scriptFolder
    
    let updateTransaction =
        (fun () -> 
            scripts
            |> List.map (ScriptContent.single >> Internal.scriptTransaction)
            |> IO.sequence_)
        |> Logger.logTime logger "Upgrade - Prepare transaction" ()
            
    use connection = new Microsoft.Data.SqlClient.SqlConnection(connectionStr)
    connection.Open()
    DbTr.commit_ connection updateTransaction
    |> Logger.logTimeIO logger "Upgrade - Execute scripts" 

/// Db upgrade (experimental)
let performDbUpgrade logger connectionStr scriptFolder =
    performDbUpgrade' logger connectionStr scriptFolder |> IO.run
                
/// Db upgrade (experimental)
let performDbUpgradeAsync logger connectionStr scriptFolder =
    performDbUpgrade' logger connectionStr scriptFolder |> IO.runAsync
