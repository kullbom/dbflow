namespace DbFlow

open System.Threading.Tasks

type IO<'context, 'a> = { Run : 'context -> 'a; RunAsync : 'context -> Task<'a> }

type IndependentIO<'a> = IO<unit, 'a>

module IO =
    let create execute executeAsync = { Run = execute; RunAsync = executeAsync }

    let run' (t : IO<'context, 'a>) ctx = t.Run ctx
    let runAsync' (t : IO<'context, 'a>) ctx = t.RunAsync ctx

    let run (t : IndependentIO<'a>) = run' t ()
    let runAsync (t : IndependentIO<'a>) = runAsync' t ()


    let map f (t : IO<'context, 'a>) = 
        create (run' t >> f) (runAsync' t >> Task.map f)
    
    let bind (f : 'a -> IO<'context, 'b>) (t : IO<'context, 'a>) = 
        create
            (fun ctx -> run' (f (run' t ctx)) ctx)
            (fun ctx -> ctx |> runAsync' t |> Task.bind (fun x -> runAsync' (f x) ctx))
    
    let ret x = 
        create (fun _ctx -> x) (fun _ctx -> Task.ret x) 

    let delay f = create (fun _ctx -> f ()) (fun _ctx -> Task.ret (f ()))


    let map2 f t0 t1 = 
        create
            (fun ctx -> 
                f (t0.Run ctx) (t1.Run ctx))
            (fun ctx ->
                Task.map2 f (t0.RunAsync ctx) (t1.RunAsync ctx))
                
    let zip t0 t1 = map2 (fun x0 x1 -> x0, x1) t0 t1

    let map3 f t0 t1 t2 = map2 (fun (x0, x1) x2 -> f x0 x1 x2) (zip t0 t1) t2

    let zip3 t0 t1 t2 = map3 (fun x0 x1 x2 -> x0, x1, x2) t0 t1 t2

    let map4 f t0 t1 t2 t3 = map2 (fun (x0, x1) (x2, x3) -> f x0 x1 x2 x3) (zip t0 t1) (zip t2 t3)
    
    let zip4 t0 t1 t2 t3 = map4 (fun x0 x1 x2 x3 -> x0, x1, x2, x3) t0 t1 t2 t3


    let sequence_ (ts : IO<'context, unit> list)  =
        let fns = ts |> List.map (fun t -> t.Run)
        let fnsAsync = ts |> List.map (fun t -> t.RunAsync)
        create 
            (fun ctx -> for fn in fns do fn ctx)
            (fun ctx -> 
                fnsAsync 
                |> List.fold 
                    (fun a fn -> a |> Task.bind (fun () -> fn ctx))
                    (Task.ret ()))

    
    type IOBuilder() =
        member inline b.Return (x)        = ret x
        member inline b.Bind (p, rest)    = p |> bind rest
        member inline b.Let (p, rest)     = rest p
        member inline b.ReturnFrom (expr) = expr
        member inline b.Using (x, f)      = use x' = x in f x'
        
    let io = new IOBuilder()
        
