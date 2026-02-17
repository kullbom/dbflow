namespace DbFlow

open System.Threading.Tasks

// A few helpful abstractions on Tasks

module Task =
    let inline map (f : 'a -> 'b) (t : Task<'a>) : Task<'b> = 
        t.ContinueWith(fun (t' : Task<'a>) -> f (t'.Result))

    let inline bind (f : 'a -> Task<'b>) (t : Task<'a>) : Task<'b> = 
        t.ContinueWith(fun (x: Task<_>) -> f x.Result).Unwrap ()

    let inline ret (x : 'a) : Task<'a> = Task.FromResult x 


    /// Convert an untyped Task to a typed (unit) Task
    let toUnit (t : Task) : Task<unit> = t.ContinueWith<unit>(fun t -> if t.IsFaulted then raise t.Exception else ())

    /// "Parallell join" of two Tasks
    let join f t0 t1 =
        [| t0 |> map Choice1Of2; t1 |> map Choice2Of2 |]
        |> Task.WhenAll
        |> map
            (function 
                | [|Choice1Of2 r0; Choice2Of2 r1|] -> f r0 r1
                | _ -> failwith "err")

    let inline map2 f t0 t1 = join f t0 t1

    let zip t0 t1 = map2 (fun x0 x1 -> x0, x1) t0 t1

