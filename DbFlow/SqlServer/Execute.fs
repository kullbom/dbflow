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
        |> DbTr.sequence_

    // https://learn.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-refreshsqlmodule-transact-sql?view=sql-server-ver17

    // Trigger definitions can also be out of sync ...
    // - investigate if the updating of trigger and view definitions is really needed or if these could be used instead...

    //let refreshSqlModuleMetadata logger connection =
    //    
    //
    //    ()

    // https://learn.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-refreshview-transact-sql?view=sql-server-ver17

    let refreshViewMetadata logger connection =
        DbTr.reader 
            "SELECT 
                v.name AS ViewName,
                SCHEMA_NAME(v.schema_id) AS SchemaName
             FROM sys.views v
             INNER JOIN sys.sql_modules m ON v.object_id = m.object_id
             WHERE m.is_schema_bound = 0"
            []
            (fun acc r -> 
                let refreshScript = 
                    let viewName = Readers.readString "ViewName" r
                    let schemaName = Readers.readString "SchemaName" r 
                    $"EXECUTE sp_refreshview N'[{schemaName}].[{viewName}]'"
                refreshScript :: acc)
            []
        |> DbTr.commit_ connection
        |> List.map (fun refreshScript -> refreshScript |> ScriptContent.single |> scriptTransaction)
        |> DbTr.sequence_ 
        |> DbTr.commit_ connection 

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

/// Read the schema of a database given a connection
let readSchema logger (options : ReadOptions) connection =
    // Ensure the current user has enough privileges to access the schema
    DbTr.reader "SELECT IS_ROLEMEMBER('db_ddladmin') CanRead" []
        (fun acc r -> (Readers.readInt32 "CanRead" r = 1) :: acc) []
    |> DbTr.commit_ connection
    |> function
        | [true] -> ()
        | r -> failwithf "Missing privileges to read schema" 
    
    if options.RefreshViewMetadata
    then 
        Internal.refreshViewMetadata logger 
        |> Logger.logTime logger "Refresh view meta data" connection 

    DatabaseSchema.read logger options connection

/// Clone a schema into a database given a target connection
let clone logger (options : ScriptOptions) (sourceDb : DatabaseSchema) (targetConnection : SqlConnection) =
    let (settingsScripts, collectedScripts) = 
        Internal.collectScriptsFromSchema options 
        |> Logger.logTime logger "Collect scripts" sourceDb
    
    let resolvedScripts =
        Dependent.resolveOrder (fun d -> d.Content) sourceDb.Dependencies
        |> Logger.logTime logger "Resolve scripts dependencies" collectedScripts

    // The "settings script" can not be run as part of the same transaction as the other scripts
    (fun () -> 
        settingsScripts
        |> List.map (fun script -> Internal.scriptTransaction script.Content.Content)
        |> DbTr.sequence_
        |> DbTr.exe targetConnection)
    |> Logger.logTime logger "Execute database setup scripts" ()

    (fun () -> 
        resolvedScripts
        |> List.map (fun script -> Internal.scriptTransaction script.Content.Content) 
        |> DbTr.sequence_
        |> DbTr.commit_ targetConnection)
    |> Logger.logTime logger "Resolve and execute scripts" ()

    SqlConnection.ClearPool targetConnection

let cloneToLocal logger (options : ScriptOptions) (sourceDb : DatabaseSchema) =
    let localDb = new LocalTempDb(logger)
    use conn = new Microsoft.Data.SqlClient.SqlConnection(localDb.ConnectionString)
    conn.Open ()
    clone logger options sourceDb conn
    localDb

/// Generate scripts of a schema to a folder structure
let generateScriptFiles (opt : ScriptOptions) (schema : DatabaseSchema) directory =
    let tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), (System.Guid.NewGuid ()).ToString().Replace("-", ""))
    Scripts.Generate.generateScripts opt schema
        (fun () _isDatabaseSettings script ->
            let subfolder = 
                match script.Content.Subdirectory with
                | Some sDir -> System.IO.Path.Combine(tempDir, sDir)
                | None -> tempDir
            if not <| System.IO.Directory.Exists subfolder
            then System.IO.Directory.CreateDirectory (subfolder) |> ignore
            
            let file = System.IO.Path.Combine(subfolder, script.Content.Filename)
            System.IO.File.WriteAllText (file, script.Content.Content |> ScriptContent.toString)
            ())
        ()

    if System.IO.Directory.Exists directory
    then System.IO.Directory.Delete(directory,true)

    System.IO.Directory.Move(tempDir, directory)


/// Schema compare. Compares two schemas returning a list of differences
let compare (d0 : DatabaseSchema) (d1 : DatabaseSchema) =
    CompareGen.Collect (d0, d1) [] []


/// Db upgrade (experimental)
let performDbUpgrade logger connectionStr scriptFolder =
    let scripts = 
        Internal.collectScriptsFromFolder 
        |> Logger.logTime logger "Upgrade - Collect scripts in folder" scriptFolder
    
    let updateTransaction =
        (fun () -> 
            scripts
            |> List.map (ScriptContent.single >> Internal.scriptTransaction)
            |> DbTr.sequence_)
        |> Logger.logTime logger "Upgrade - Prepare transaction" ()
            
    (fun dbTransaction -> 
        use connection = new Microsoft.Data.SqlClient.SqlConnection(connectionStr)
        connection.Open()
        DbTr.commit_ connection dbTransaction)
    |> Logger.logTime logger "Upgrade - Execute scripts" updateTransaction
                