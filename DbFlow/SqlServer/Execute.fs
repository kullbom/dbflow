module DbFlow.SqlServer.Execute

open DbFlow
open DbFlow.SqlServer.Schema
open DbFlow.SqlServer.Scripts.Generate

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

    let redefineViews logger options connection =
        let (views, deps) = DatabaseSchema.preReadAllViews logger connection
        // Drop scripts
        let dropScripts = 
            views 
            |> List.map 
                (fun view -> 
                    let drop_script = $"DROP VIEW [{view.Schema.Name}].[{view.Name}]"
                    Dependent.create drop_script [view.Object.ObjectId] [] 0)
            |> Dependent.resolveOrder (fun d -> d.Content) deps 
        // Create scripts
        let createScripts = 
            views 
            |> List.fold 
                (fun scripts view -> 
                    let create_script =
                        Dependent.create view.Definition [view.Object.ObjectId] [] 0
                    let view_scripts =
                        let view_name = $"[{view.Schema.Name}].[{view.Name}]"
                        view.Indexes
                        |> Array.fold 
                            (fun acc index ->
                                match Scripts.Generate.getIndexDefinitionStr options view_name true index with
                                | None -> acc
                                | Some indexScript -> 
                                    let index_contains_objects =
                                        match index.Object with Some o -> [o.ObjectId] | None -> []
                                    Dependent.create indexScript index_contains_objects [view.Object.ObjectId] 0 :: acc)
                            (create_script :: scripts)
                    view_scripts)
                []
            |> Dependent.resolveOrder (fun d -> d.Content) deps 
            
        // The drop scripts needs to be reverted since dependency works on the asumption that objects are created...
        dropScripts 
        |> List.fold 
            (fun acc s -> scriptTransaction (ScriptContent.single s.Content) :: acc)
            (createScripts |> List.map (fun s -> s.Content |> ScriptContent.single |> scriptTransaction))
        |> DbTr.sequence_ 
        |> DbTr.commit_ connection 
        ()

    let collectScriptsFromSchema (options : Options) (sourceDb : DatabaseSchema) =
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
let readSchema logger (options : Options) connection =
    // Ensure the current user has enough privileges to access the schema
    DbTr.reader "SELECT IS_ROLEMEMBER('db_ddladmin') CanRead" []
        (fun acc r -> (Readers.readInt32 "CanRead" r = 1) :: acc) []
    |> DbTr.commit_ connection
    |> function
        | [true] -> ()
        | r -> failwithf "Missing privileges to read schema" 
    
    Internal.redefineViews logger options 
    |> Logger.logTime logger "Refresh view meta data" connection 

    DatabaseSchema.read logger options connection

/// Clone a schema into a database given a target connection
let clone logger (options : Options) (sourceDb : DatabaseSchema) (targetConnection : System.Data.IDbConnection) =
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

let cloneToLocal logger (options : Options) (sourceDb : DatabaseSchema) =
    let localDb = new LocalTempDb(logger)
    use conn = new Microsoft.Data.SqlClient.SqlConnection(localDb.ConnectionString)
    conn.Open ()
    clone logger options sourceDb conn
    localDb

/// Generate scripts of a schema to a folder structure
let generateScriptFiles (opt : Options) (schema : DatabaseSchema) directory =
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
                