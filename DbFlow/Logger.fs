namespace DbFlow

type Logger = Logger of (string -> unit)
    with member x.info s = let (Logger f) = x in f s 

module Logger =
    let dummy = Logger (fun _ -> ())
    
    let create f = Logger f
    
    /// C# friendly 
    let fromFunc (f : System.Action<string>) = create f.Invoke
    
    let decorate (Logger logger') f = Logger (fun m -> logger' (f m))
        
    let infoWithTime (message : string) (logger : Logger) =  
        let timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff")
        logger.info $"{timestamp} {message}" 
        
    let logTime (logger : Logger) (taskName : string) arg fn =
        let ws = System.Diagnostics.Stopwatch ()
        ws.Start ()
        let result = fn arg
        infoWithTime $"{taskName} (in {ws.ElapsedMilliseconds} ms)" logger
        result

    let logTimeIO (logger : Logger) (taskName : string) (io : IO<'a,'b>) =
        IO.delay
            (fun () -> 
                let ws = System.Diagnostics.Stopwatch ()
                ws.Start ()
                ws)
        |> IO.bind (fun ws -> io |> IO.map (fun result -> ws, result))
        |> IO.map 
            (fun (ws, result) ->
                infoWithTime $"{taskName} (in {ws.ElapsedMilliseconds} ms)" logger
                result)