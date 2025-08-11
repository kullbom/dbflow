namespace DbFlow

module Option =
    let escape v o = match o with Some x -> x | None -> v

module Tuple =
    let pair x y = x, y

type PickMap<'key, 'data when 'key : comparison> = { mutable Map : Map<'key, int * 'data> }

module PickMap =
    let ofMap m = { Map = Map.map (fun _ v -> 0, v) m }

    let fold f seed (pm : PickMap<_, _>) =
        pm.Map |> Map.fold (fun acc key (c, d) -> f acc key c d) seed

    let toList (pm : PickMap<'key, 'data>) =
        pm |> fold (fun acc _ _ v -> v :: acc) []
        
    let tryPick key (pm : PickMap<'key, 'data>) =
        match Map.tryFind key pm.Map with
        | Some (c, d) ->
            pm.Map <- Map.add key (c + 1, d) pm.Map
            Some d
        | None -> None

    let pick key (pm : PickMap<'key, 'data>) =
        let (c,d) = Map.find key pm.Map 
        pm.Map <- Map.add key (c + 1, d) pm.Map
        d

    let unused exclude (pm : PickMap<'key, 'data>) =
        pm |> fold (fun acc key c d -> if c = 0 && not (exclude d) then (key, d) :: acc else acc) []
    
module Logger =
    let logTime logger (id : string) arg fn =
        let ws = System.Diagnostics.Stopwatch ()
        ws.Start ()
        let result = fn arg
        let timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff")
        logger $"{timestamp} Executed {id} (took {ws.ElapsedMilliseconds} ms)"
        result

module IO =
    let writeToFile filename f =
        // Write the file first to a memory stream
        let bytes =
            use ms = new System.IO.MemoryStream()
            (use w = new System.IO.StreamWriter(ms)
             f w)
            ms.ToArray ()
        // ... and then to the actual file (to minimize the IO time)
        System.IO.File.WriteAllBytes (filename, bytes)
