namespace DbFlow

type Dependent<'identity, 'a when 'identity : comparison> = {
    Content : 'a 
    
    Contains : 'identity Set  
    DependsOn : 'identity Set

    Priority : int
}

module Dependent =
    let create content contains_objects depends_on priority = 
        {
            Content = content  
            
            Contains = contains_objects |> Set.ofList
            DependsOn = depends_on |> Set.ofList

            Priority = priority
        }

    // Objects needs to be created in correct order. 
    // - A schemabound view can not be created before all the referenced views has been created
    // - The same is true for procedures etc.
    
    // Ensures that a list of Scripts is in "definable order" so that referenced are defined first
    let ensureExecutableOrder' (allScripts : Dependent<_,_> list) = 
        let objectScripts =
            allScripts
            |> List.fold 
                (fun m (s : Dependent<_,_>) ->
                    s.Contains
                    |> Set.fold 
                        (fun m' object_id -> 
                            if Map.containsKey object_id m'
                            then failwithf "Duplicate key: %A" object_id
                            Map.add object_id s m') 
                        m)
                Map.empty
        
        let rec resolve path visited resolvedList resolvedSet (scripts : Dependent<_,_> list) =
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
                            script.DependsOn
                            |> Set.fold (fun acc object_id -> match Map.tryFind object_id objectScripts with Some s -> s :: acc | None -> acc) []
                        let path'' = path' // "DEP" :: path'
                        resolve path'' (Set.add script visited) resolvedList resolvedSet depends_on
                    resolve path' visited' (script :: resolvedList') (Set.add script resolvedSet') scripts'
        
        let (_visited, resolvedList, _resolvedSet) = resolve [] Set.empty [] Set.empty allScripts
    
        resolvedList |> List.rev
    
    let ensureExecutableOrder scripts =
        scripts
        |> List.groupBy (fun s -> s.Priority)
        |> List.map (fun (prio, ss) -> prio, ensureExecutableOrder' ss)
        |> List.sortBy (fun (prio,_) -> prio)
        |> List.collect (fun (_prio, ss) -> ss)
    
    let addDependencies dependencies (script : Dependent<_,_>) =
        let contains = script.Contains
        // Add dependencies
        let dependsOn' =
            contains
            |> Set.fold 
                (fun acc o -> 
                    match Map.tryFind o dependencies with
                    | Some os -> os |> List.fold (fun acc' o' -> Set.add o' acc') acc
                    | None -> acc)
                script.DependsOn
        // Clean up - remove self from dependencies
        let dependsOn =
            contains
            |> Set.fold (fun s o -> Set.remove o s) dependsOn'
            
        { script with DependsOn = dependsOn; Contains = contains }
    
    let resolveOrder dependencies (scripts : Dependent<_,_> list) =
        scripts
        |> List.map (addDependencies dependencies)
        |> ensureExecutableOrder
    
