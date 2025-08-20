namespace DbFlow

module Option =
    let escape v o = match o with Some x -> x | None -> v

module Tuple =
    let pair x y = x, y

module Array =
    let joinBy (separator : string) (formatter : _ -> string) xs =
        xs 
        |> Array.map formatter
        |> fun ss -> System.String.Join (separator, ss)


/// A reference counting map - used to ensure that all objects are referenced/used
type RCMap<'key, 'data when 'key : comparison> = { mutable Map : Map<'key, int * 'data> }

module RCMap =
    let ofMap m = { Map = Map.map (fun _ v -> 0, v) m }

    let fold f seed (pm : RCMap<_, _>) =
        pm.Map |> Map.fold (fun acc key (c, d) -> f acc key c d) seed

    let toList (pm : RCMap<'key, 'data>) =
        pm |> fold (fun acc _ _ v -> v :: acc) []
        
    let tryPick key (pm : RCMap<'key, 'data>) =
        match Map.tryFind key pm.Map with
        | Some (c, d) ->
            pm.Map <- Map.add key (c + 1, d) pm.Map
            Some d
        | None -> None

    let pick key (pm : RCMap<'key, 'data>) =
        let (c,d) = Map.find key pm.Map 
        pm.Map <- Map.add key (c + 1, d) pm.Map
        d

    let unused exclude (pm : RCMap<'key, 'data>) =
        pm |> fold (fun acc key c d -> if c = 0 && not (exclude d) then (key, d) :: acc else acc) []
    
type Logger = Logger of (string -> unit)
    with member x.info s = let (Logger f) = x in f s 

module Logger =
    let dummy = Logger (fun _ -> ())
    
    let create f = Logger f
    
    /// C# friendly 
    let fromFunc (f : System.Action<string>) = create f.Invoke
    
    let decorate f (Logger logger') = Logger (fun m -> logger' (f m))
        
    let logTime (logger : Logger) (id : string) arg fn =
        let ws = System.Diagnostics.Stopwatch ()
        ws.Start ()
        let result = fn arg
        let timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff")
        logger.info $"{timestamp} Executed {id} (took {ws.ElapsedMilliseconds} ms)"
        result

