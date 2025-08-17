module DbFlow.SqlServer.Scripts.Execute

open DbFlow
open DbFlow.SqlServer.Schema

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

let collectScripts (options : Options) (sourceDb : DATABASE) =
    let mutable scripts = []
    
    Generate.generateScripts options sourceDb
        (fun script -> scripts <- script :: scripts)
    scripts

let clone logger (options : Options) (sourceDb : DATABASE) (targetConnection : System.Data.IDbConnection) =
    let collectedScripts = 
        collectScripts options 
        |> Logger.logTime logger "DbFlow - collect scripts" sourceDb
    
    let resolvedScripts =
        Dependencies.resolveScriptOrder sourceDb.dependencies
        |> Logger.logTime logger "DbFlow - resolve scripts dependencies" collectedScripts

    (fun () -> 
        //logger $"Executing script {script.directory_name}\\{script.filename}"
        resolvedScripts
        |> List.map (fun script -> scriptTransaction script.content.content) 
        |> DbTr.sequence_
        |> DbTr.commit_ targetConnection)
    |> Logger.logTime logger "DbFlow - resolve and execute scripts" ()


let generateScriptFiles (opt : Options) (db : DATABASE) folder =
    if System.IO.Directory.Exists folder
    then System.IO.Directory.Delete(folder,true)

    Generate.generateScripts opt db
        (fun script ->
            let subfolder = System.IO.Path.Combine(folder, script.content.directory_name)
            if not <| System.IO.Directory.Exists subfolder
            then System.IO.Directory.CreateDirectory (subfolder) |> ignore
            
            let file = System.IO.Path.Combine(subfolder, script.content.filename)
            System.IO.File.WriteAllText (file, script.content.content)
            ())


// Db compare

let compare (d0 : DATABASE) (d1 : DATABASE) =
    CompareGen.Collect (d0, d1, [],[])


// Db upgrade

let collectAllScripts (scriptsFolder : string)=
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

let performDbUpgrade logger connectionStr scriptFolder =
    let scripts = 
        collectAllScripts 
        |> Logger.logTime logger "Upgrade - Collect scripts in folder" scriptFolder
    
    let updateTransaction =
        (fun () -> 
            scripts
            |> List.map scriptTransaction
            |> DbTr.sequence_)
        |> Logger.logTime logger "Upgrade - Prepare transaction" ()
    
            
    (fun dbTransaction -> 
        use connection = new Microsoft.Data.SqlClient.SqlConnection(connectionStr)
        connection.Open()
        DbTr.commit_ connection dbTransaction)
    |> Logger.logTime logger "Upgrade - Execute scripts" updateTransaction
                