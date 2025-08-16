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
    
// Objects needs to be created in correct order. 
// - A schemabound view can not be created before all the referenced views has been created
// - The same is true for procedures etc.

// Ensures that a list of Scripts is in "definable order" so that referenced are defined first
let ensureExecutableOrder' (allScripts : Script<OBJECT> list) = 
    let objectScripts =
        allScripts
        |> List.fold 
            (fun m (s : Script<OBJECT>) ->
                s.contains_objects
                |> Set.fold 
                    (fun m' (o : OBJECT) -> 
                        if Map.containsKey o.object_id m'
                        then failwithf "Duplicate key: %A" o
                        Map.add o.object_id s m') 
                    m)
            Map.empty
    
    let rec resolve path visited resolvedList resolvedSet (scripts : Script<OBJECT> list) =
        match scripts with
        | [] -> visited, resolvedList, resolvedSet
        | script :: scripts' ->
            if Set.contains script resolvedSet 
            then resolve path visited resolvedList resolvedSet scripts'
            else
                let path' = path // $"{script.directory_name}\{script.filename}" :: path
                
                if Set.contains script visited 
                then failwithf "circular: %A" path'
                
                let (visited', resolvedList', resolvedSet') = 
                    let depends_on = 
                        script.depends_on
                        |> Set.fold (fun acc o -> match Map.tryFind o.object_id objectScripts with Some s -> s :: acc | None -> acc) []
                    let path'' = path' // "DEP" :: path'
                    resolve path'' (Set.add script visited) resolvedList resolvedSet depends_on
                resolve path' visited' (script :: resolvedList') (Set.add script resolvedSet') scripts'
    
    let (_visited, resolvedList, _resolvedSet) = resolve [] Set.empty [] Set.empty allScripts

    resolvedList |> List.rev

let ensureExecutableOrder scripts (sourceDb : DATABASE) =
    let objectScripts =
        let objectLookup =
            sourceDb.all_objects
            |> List.map (fun o -> o.object_id, o)
            |> Map.ofList

        scripts
        |> List.map (fun s -> { 
                directory_name = s.directory_name 
                filename = s.filename 
                content = s.content
                
                contains_objects = 
                    s.contains_objects 
                    |> Set.fold (fun s id -> match Map.tryFind id objectLookup with Some o -> Set.add o s | None -> s) Set.empty 
                depends_on = 
                    s.depends_on 
                    |> Set.fold (fun s id -> match Map.tryFind id objectLookup with Some o -> Set.add o s | None -> s) Set.empty 
                priority = s.priority
            })
    objectScripts
    |> List.groupBy (fun s -> s.priority)
    |> List.map (fun (prio, ss) -> prio, ensureExecutableOrder' ss)
    |> List.sortBy (fun (prio,_) -> prio)
    |> List.collect (fun (_prio, ss) -> ss)

let scriptsWithAddedDependencies (options : Options) (sourceDb : DATABASE) =
    let mutable scripts = []
    
    Generate.generateScripts options sourceDb
        (fun script -> 
            let contains_objects = script.contains_objects
            let depends_on' =
                contains_objects
                |> Set.fold 
                    (fun acc o -> 
                        match Map.tryFind o sourceDb.dependencies with
                        | Some os -> os |> List.fold (fun acc' o' -> Set.add o' acc') acc
                        | None -> acc)
                    script.depends_on
            let depends_on =
                contains_objects
                |> Set.fold (fun s o -> Set.remove o s) depends_on'
                
            scripts <- { script with depends_on = depends_on; contains_objects = contains_objects } :: scripts)

    scripts

let clone logger (options : Options) (sourceDb : DATABASE) (targetConnection : System.Data.IDbConnection) =
    let scripts = 
        scriptsWithAddedDependencies options 
        |> Logger.logTime logger "DbFlow - collect scripts" sourceDb
    
    let resolvedScripts =
        ensureExecutableOrder scripts
        |> Logger.logTime logger "DbFlow - resolve scripts dependencies" sourceDb

    (fun () -> 
        //logger $"Executing script {script.directory_name}\\{script.filename}"
        resolvedScripts
        |> List.map (fun script -> scriptTransaction script.content) 
        |> DbTr.sequence_
        |> DbTr.commit_ targetConnection)
    |> Logger.logTime logger "DbFlow - resolve and execute scripts" () 


let generateScriptFiles (opt : Options) (db : DATABASE) folder =
    if System.IO.Directory.Exists folder
    then System.IO.Directory.Delete(folder,true)

    Generate.generateScripts opt db
        (fun script ->
            let subfolder = System.IO.Path.Combine(folder, script.directory_name)
            if not <| System.IO.Directory.Exists subfolder
            then System.IO.Directory.CreateDirectory (subfolder) |> ignore
            
            let file = System.IO.Path.Combine(subfolder, script.filename)
            System.IO.File.WriteAllText (file, script.content)
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
                