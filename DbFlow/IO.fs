namespace DbFlow

open System
open System.Data
open System.Data.Common
open System.Threading.Tasks

// A small db/IO library

module Task =
    let inline map (f : 'a -> 'b) (t : Task<'a>) : Task<'b> = 
        t.ContinueWith(fun (t' : Task<'a>) -> f (t'.Result))

    let inline bind (f : 'a -> Task<'b>) (t : Task<'a>) : Task<'b> = 
        t.ContinueWith(fun (x: Task<_>) -> f x.Result).Unwrap ()

    let inline enter (x : 'a) : Task<'a> = 
        task { return x } 

    let toUnit (t : Task) : Task<unit> = t.ContinueWith<unit>(fun t -> if t.IsFaulted then raise t.Exception else ())

    let map2 f t0 t1 =
        [| t0 |> map Choice1Of2; t1 |> map Choice2Of2 |]
        |> Task.WhenAll
        |> map
            (function 
                | [|Choice1Of2 r0; Choice2Of2 r1|] -> f r0 r1
                | _ -> failwith "err")

    let zip t0 t1 = map2 (fun x0 x1 -> x0, x1) t0 t1


type IO<'context, 'a> = { Execute : 'context -> 'a; ExecuteAsync : 'context -> Task<'a> } 

module IO =
    let create execute executeAsync = { Execute = execute; ExecuteAsync = executeAsync }

    let map f (t : IO<'context, 'a>) = 
        create
            (fun ctx -> ctx |> t.Execute |> f)
            (fun ctx -> ctx |> t.ExecuteAsync |> Task.map f)
    
    let bind (f : 'a -> IO<'context, 'b>) (t : IO<'context, 'a>) = 
        create
            (fun ctx -> (f (t.Execute ctx)).Execute ctx)
            (fun ctx -> ctx |> t.ExecuteAsync |> Task.bind (fun x -> (f x).ExecuteAsync ctx))
    
    let ret x = 
        create (fun _ctx -> x) (fun _ctx -> Task.FromResult x) 

    let delay f = create (fun _ctx -> f ()) (fun _ctx -> Task.FromResult (f ()))


    let map2 f t0 t1 = 
        create
            (fun ctx -> 
                f (t0.Execute ctx) (t1.Execute ctx))
            (fun ctx ->
                Task.map2 f (t0.ExecuteAsync ctx) (t1.ExecuteAsync ctx))
                
    let zip t0 t1 = map2 (fun x0 x1 -> x0, x1) t0 t1

    let map3 f t0 t1 t2 = map2 (fun (x0, x1) x2 -> f x0 x1 x2) (zip t0 t1) t2

    let zip3 t0 t1 t2 = map3 (fun x0 x1 x2 -> x0, x1, x2) t0 t1 t2

    let map4 f t0 t1 t2 t3 = map2 (fun (x0, x1) (x2, x3) -> f x0 x1 x2 x3) (zip t0 t1) (zip t2 t3)
    
    let zip4 t0 t1 t2 t3 = map4 (fun x0 x1 x2 x3 -> x0, x1, x2, x3) t0 t1 t2 t3


    let sequence_ (ts : IO<'context, unit> list)  =
        let fns = ts |> List.map (fun t -> t.Execute)
        let fnsAsync = ts |> List.map (fun t -> t.ExecuteAsync)
        create 
            (fun ctx -> for fn in fns do fn ctx)
            (fun ctx -> 
                fnsAsync 
                |> List.fold 
                    (fun a fn -> a |> Task.bind (fun () -> fn ctx))
                    (Task.enter ()))

    type CompExprBuilder() =
        member inline b.Return (x)        = ret x
        member inline b.Bind (p, rest)    = p |> bind rest
        member inline b.Let (p, rest)     = rest p
        member inline b.ReturnFrom (expr) = expr
        
    let builder = new CompExprBuilder()
        

module FileSystem =
    let writeAllText path (fileContent : string) =
        IO.create
            (fun () -> System.IO.File.WriteAllText(path, fileContent))
            (fun () -> System.IO.File.WriteAllTextAsync(path, fileContent) |> Task.toUnit)


module Readers =
    let readObject n (r : System.Data.IDataReader) = r.GetValue (r.GetOrdinal n)
    let readString n (r : System.Data.IDataReader) = r.GetString (r.GetOrdinal n) 
    let readBool n (r : System.Data.IDataReader) = r.GetBoolean (r.GetOrdinal n)
    let readByte n (r : System.Data.IDataReader) = r.GetByte (r.GetOrdinal n)
    let readInt16 n (r : System.Data.IDataReader) = r.GetInt16 (r.GetOrdinal n)
    let readInt32 n (r : System.Data.IDataReader) = r.GetInt32 (r.GetOrdinal n)
    let readDateTime n (r : System.Data.IDataReader) = r.GetDateTime (r.GetOrdinal n)
    let nullable n f (r : System.Data.IDataReader) = if r.IsDBNull (r.GetOrdinal n) then None else Some (f n r)


type DbContext = { Connection : DbConnection; Transaction : DbTransaction option }


type DbTr<'a> = IO<DbContext,'a>

module DbTr =
    let nonQuery cmdText parameters =
        IO.create  
            (fun ctx -> 
                let cmd = ctx.Connection.CreateCommand()
                cmd.CommandText <- cmdText
                match ctx.Transaction with 
                | Some t -> cmd.Transaction <- t
                | None -> ()
                for (parameterName : string, parameterValue : obj) in parameters do
                    let p = cmd.CreateParameter()
                    p.ParameterName <- parameterName
                    p.Value <- parameterValue
                    cmd.Parameters.Add(p) |> ignore
                try
                    cmd.ExecuteNonQuery() |> ignore
                with e ->
                    raise (Exception($"Batch failed: {cmdText}", e)))
            (fun ctx -> 
                let cmd = ctx.Connection.CreateCommand()
                cmd.CommandText <- cmdText
                match ctx.Transaction with 
                | Some t -> cmd.Transaction <- t
                | None -> ()
                for (parameterName : string, parameterValue : obj) in parameters do
                    let p = cmd.CreateParameter()
                    p.ParameterName <- parameterName
                    p.Value <- parameterValue
                    cmd.Parameters.Add(p) |> ignore
                try
                    cmd.ExecuteNonQueryAsync() |> Task.map ignore
                with e ->
                    raise (Exception($"Batch failed: {cmdText}", e)))                
        

    /// Custom reader - traversing the data reader is up to the user    
    let reader' cmdText parameters f =
        IO.create 
            (fun ctx -> 
                let cmd = ctx.Connection.CreateCommand()
                cmd.CommandText <- cmdText
                match ctx.Transaction with 
                | Some t -> cmd.Transaction <- t
                | None -> ()
                for (parameterName : string, parameterValue : obj) in parameters do
                    let p = cmd.CreateParameter()
                    p.ParameterName <- parameterName
                    p.Value <- parameterValue
                    cmd.Parameters.Add(p) |> ignore
                use dataReader = cmd.ExecuteReader() 
                f dataReader)
            (fun ctx -> 
                let cmd = ctx.Connection.CreateCommand() 
                cmd.CommandText <- cmdText
                match ctx.Transaction with 
                | Some t -> cmd.Transaction <- t
                | None -> ()
                for (parameterName : string, parameterValue : obj) in parameters do
                    let p = cmd.CreateParameter()
                    p.ParameterName <- parameterName
                    p.Value <- parameterValue
                    cmd.Parameters.Add(p) |> ignore
                task {
                    use! dataReader = cmd.ExecuteReaderAsync() 
                    return f dataReader
                })
        

    /// Reader - folds that data of the data reader calling the fold function once per row of data
    let reader cmdText parameters f seed =
        reader' cmdText parameters
            (fun dataReader ->
                let mutable acc = seed
                let f' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
                while (dataReader.Read()) do
                    acc <- f'.Invoke(acc, dataReader)
                acc)

    let readList cmdText parameters f =
        reader cmdText parameters (fun acc r -> f r :: acc) [] |> IO.map List.rev

    let readArray cmdText parameters f =
        readList cmdText parameters f |> IO.map List.toArray
    
    let readMap cmdText parameters f =
        reader cmdText parameters (fun m r -> let (k, v) = f r in Map.add k v m) Map.empty

    let readSet cmdText parameters f =
        reader cmdText parameters (fun s r -> Set.add (f r) s) Set.empty

    /// Commit a transaction
    let commit (c : DbConnection) (i : System.Data.IsolationLevel) (t : IO<DbContext, 'a>) =
        let tr = c.BeginTransaction(i)
        let mutable success = false
        try 
            let res = t.Execute { Connection = c; Transaction = Some tr }
            tr.Commit ()
            success <- true
            res
        finally
            if not success
            then tr.Rollback ()

    /// Commit a transaction with isolation level READ COMMITED
    let commit_ c t = commit c System.Data.IsolationLevel.ReadCommitted t

    /// Execute a transaction {t} without an actual database transaction
    let exe (c : DbConnection) (t : IO<DbContext, 'a>) =
        t.Execute { Connection = c; Transaction = None }


    /// Commit a transaction asyncronously
    let commitAsync (c : DbConnection) (i : System.Data.IsolationLevel) (t : IO<DbContext, 'a>) =
        task {
            let! tr = c.BeginTransactionAsync(i)
            let mutable success = false
            try 
                let! res = t.ExecuteAsync { Connection = c; Transaction = Some tr }
                do! tr.CommitAsync ()
                success <- true
                return res
            finally
                if not success
                then tr.Rollback ()
        }
        
    /// Commit a transaction asyncronously with isolation level READ COMMITED
    let commitAsync_ c t = commitAsync c System.Data.IsolationLevel.ReadCommitted t

    /// Execute a transaction {t} asyncronously without an actual database transaction
    let exeAsync (c : DbConnection) (t : IO<DbContext, 'a>) =
        t.ExecuteAsync { Connection = c; Transaction = None }





