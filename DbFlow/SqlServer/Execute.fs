module DbFlow.SqlServer.Execute

open DbFlow
open DbFlow.Dependencies
open DbFlow.SqlServer.Schema

module Internal = 
    let scriptTransaction (script : string) = 
        script
        |> SqlParser.Batches.splitInSqlBatches 
        |> List.collect
            (fun sqlBatch ->
                [
                    "SET QUOTED_IDENTIFIER ON"
                    "SET ANSI_NULLS ON" 
                    sqlBatch 
                    "SET QUOTED_IDENTIFIER OFF"
                    "SET ANSI_NULLS OFF"
                ])
        |> List.map  (fun s -> DbTr.nonQuery s [])
        |> DbTr.sequence_

    let redefineViews logger options connection =
        let (views, deps) = DATABASE.preReadAllViews logger connection
        // Drop scripts
        let dropScripts = 
            views 
            |> List.map 
                (fun view -> 
                    let drop_script = $"DROP VIEW [{view.schema.name}].[{view.view_name}]"
                    Dependent.create drop_script [view.object.object_id] [] 0)
            |> Dependencies.resolveScriptOrder deps 
        // Create scripts
        let createScripts = 
            views 
            |> List.fold 
                (fun scripts view -> 
                    let create_script =
                        Dependent.create view.definition [view.object.object_id] [] 0
                    let view_scripts =
                        let view_name = $"[{view.schema.name}].[{view.view_name}]"
                        view.indexes
                        |> Array.fold 
                            (fun acc index ->
                                match Scripts.Generate.getIndexDefinitionStr options view_name index with
                                | None -> acc
                                | Some indexScript -> 
                                    let index_contains_objects =
                                        match index.object with Some o -> [o.object_id] | None -> []
                                    Dependent.create indexScript index_contains_objects [view.object.object_id] 0 :: acc)
                            (create_script :: scripts)
                    view_scripts)
                []
            |> Dependencies.resolveScriptOrder deps 
            
        // The drop scripts needs to be reverted since dependency works on the asumption that objects are created...
        dropScripts 
        |> List.fold 
            (fun acc s -> scriptTransaction s.content :: acc)
            (createScripts |> List.map (fun s -> scriptTransaction s.content))
        |> DbTr.sequence_ 
        |> DbTr.commit_ connection 
        ()

    let collectScriptsFromSchema (options : Options) (sourceDb : DATABASE) =
        let mutable scripts = []
        
        Scripts.Generate.generateScripts options sourceDb
            (fun script -> scripts <- script :: scripts)
        scripts

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
    DbTr.reader "SELECT IS_ROLEMEMBER('db_ddladmin') CanRead" []
        (fun acc r -> (Readers.readInt32 "CanRead" r = 1) :: acc) []
    |> DbTr.commit_ connection
    |> function
        | [true] -> ()
        | r -> failwithf "Missing privileges to read schema" 
    
    if not options.SchemazenCompatibility
    then 
        Internal.redefineViews logger options 
        |> Logger.logTime logger "Refresh view meta data" connection 

    DATABASE.read logger options connection

/// Clone a schema into a database given a target connection
let clone logger (options : Options) (sourceDb : DATABASE) (targetConnection : System.Data.IDbConnection) =
    let collectedScripts = 
        Internal.collectScriptsFromSchema options 
        |> Logger.logTime logger "DbFlow - collect scripts" sourceDb
    
    let resolvedScripts =
        Dependencies.resolveScriptOrder sourceDb.dependencies
        |> Logger.logTime logger "DbFlow - resolve scripts dependencies" collectedScripts

    (fun () -> 
        //logger $"Executing script {script.directory_name}\\{script.filename}"
        resolvedScripts
        |> List.map (fun script -> Internal.scriptTransaction script.content.content) 
        |> DbTr.sequence_
        |> DbTr.commit_ targetConnection)
    |> Logger.logTime logger "DbFlow - resolve and execute scripts" ()

/// Generate scripts of a schema to a folder structure
let generateScriptFiles (opt : Options) (schema : DATABASE) folder =
    if System.IO.Directory.Exists folder
    then System.IO.Directory.Delete(folder,true)

    Scripts.Generate.generateScripts opt schema
        (fun script ->
            let subfolder = System.IO.Path.Combine(folder, script.content.directory_name)
            if not <| System.IO.Directory.Exists subfolder
            then System.IO.Directory.CreateDirectory (subfolder) |> ignore
            
            let file = System.IO.Path.Combine(subfolder, script.content.filename)
            System.IO.File.WriteAllText (file, script.content.content)
            ())


/// Schema compare. Compares two schemas returning a list of differences
let compare (d0 : DATABASE) (d1 : DATABASE) =
    CompareGen.Collect (d0, d1) [] []


/// Db upgrade (experimental)
let performDbUpgrade logger connectionStr scriptFolder =
    let scripts = 
        Internal.collectScriptsFromFolder 
        |> Logger.logTime logger "Upgrade - Collect scripts in folder" scriptFolder
    
    let updateTransaction =
        (fun () -> 
            scripts
            |> List.map Internal.scriptTransaction
            |> DbTr.sequence_)
        |> Logger.logTime logger "Upgrade - Prepare transaction" ()
            
    (fun dbTransaction -> 
        use connection = new Microsoft.Data.SqlClient.SqlConnection(connectionStr)
        connection.Open()
        DbTr.commit_ connection dbTransaction)
    |> Logger.logTime logger "Upgrade - Execute scripts" updateTransaction
                