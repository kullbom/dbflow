namespace DbFlow

open System
open System.Data
open System.Data.Common
open System.Threading.Tasks


module Task =
    let inline map (f : 'a -> 'b) (t : Task<'a>) : Task<'b> = 
        t.ContinueWith(fun (t' : Task<'a>) -> f (t'.Result))

    let inline bind (f : 'a -> Task<'b>) (t : Task<'a>) : Task<'b> = 
        t.ContinueWith(fun (x: Task<_>) -> f x.Result).Unwrap ()

    let inline enter (x : 'a) : Task<'a> = 
        task { return x } 

    let toUnit (t : Task) : Task<unit> = t.ContinueWith<unit>(fun t -> if t.IsFaulted then raise t.Exception else ())

module Readers =
    let readObject n (r : System.Data.IDataReader) = r.GetValue (r.GetOrdinal n)
    let readString n (r : System.Data.IDataReader) = r.GetString (r.GetOrdinal n) 
    let readBool n (r : System.Data.IDataReader) = r.GetBoolean (r.GetOrdinal n)
    let readByte n (r : System.Data.IDataReader) = r.GetByte (r.GetOrdinal n)
    let readInt16 n (r : System.Data.IDataReader) = r.GetInt16 (r.GetOrdinal n)
    let readInt32 n (r : System.Data.IDataReader) = r.GetInt32 (r.GetOrdinal n)
    let readDateTime n (r : System.Data.IDataReader) = r.GetDateTime (r.GetOrdinal n)
    let nullable n f (r : System.Data.IDataReader) = if r.IsDBNull (r.GetOrdinal n) then None else Some (f n r)

// A microscopic db library
type DbContext = { Connection : DbConnection; Transaction : DbTransaction option }
type 'a DbTr = { Execute : DbContext -> 'a; ExecuteAsync : DbContext -> Task<'a> } 

module DbTr =
    let create execute executeAsync = { Execute = execute; ExecuteAsync = executeAsync }

    let map f (t : 'a DbTr) = 
        create
            (fun ctx -> ctx |> t.Execute |> f)
            (fun ctx -> ctx |> t.ExecuteAsync |> Task.map f)
    
    let bind (f : 'a -> 'b DbTr) (t : 'a DbTr) = 
        create
            (fun ctx -> (f (t.Execute ctx)).Execute ctx)
            (fun ctx -> ctx |> t.ExecuteAsync |> Task.bind (fun x -> (f x).ExecuteAsync ctx))
    
    let ret x = 
        create (fun _ctx -> x) (fun _ctx -> Task.FromResult x) 


    let map2 f t0 t1 = t0 |> bind (fun x0 -> t1 |> map (f x0))
    
    let zip t0 t1 = map2 (fun x0 x1 -> x0, x1) t0 t1

    let map3 f t0 t1 t2 = zip t0 t1 |> bind (fun (x0, x1) -> t2 |> map (f x0 x1))

    let zip3 t0 t1 t2 = map3 (fun x0 x1 x2 -> x0, x1, x2) t0 t1 t2

    let map4 f t0 t1 t2 t3 = zip3 t0 t1 t2 |> bind (fun (x0, x1, x2) -> t3 |> map (f x0 x1 x2))

    let zip4 t0 t1 t2 t3 = map4 (fun x0 x1 x2 x3 -> x0, x1, x2, x3) t0 t1 t2 t3


    let sequence_ (ts : DbTr<unit> list)  =
        let fns = ts |> List.map (fun t -> t.Execute)
        let fnsAsync = ts |> List.map (fun t -> t.ExecuteAsync)
        create 
            (fun ctx -> for fn in fns do fn ctx)
            (fun ctx -> 
                fnsAsync 
                |> List.fold 
                    (fun a fn -> a |> Task.bind (fun () -> fn ctx))
                    (Task.enter ()))
        

    let nonQuery cmdText parameters =
        create  
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
        create 
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
        reader cmdText parameters (fun acc r -> f r :: acc) [] |> map List.rev

    let readArray cmdText parameters f =
        readList cmdText parameters f |> map List.toArray
    
    let readMap cmdText parameters f =
        reader cmdText parameters (fun m r -> let (k, v) = f r in Map.add k v m) Map.empty

    let readSet cmdText parameters f =
        reader cmdText parameters (fun s r -> Set.add (f r) s) Set.empty

    /// Commit a transaction
    let commit (c : DbConnection) (i : System.Data.IsolationLevel) (t : 'a DbTr) =
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
    let exe (c : DbConnection) (t : 'a DbTr) =
        t.Execute { Connection = c; Transaction = None }


    /// Commit a transaction asyncronously
    let commitAsync (c : DbConnection) (i : System.Data.IsolationLevel) (t : 'a DbTr) =
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
    let exeAsync (c : DbConnection) (t : 'a DbTr) =
        t.ExecuteAsync { Connection = c; Transaction = None }

    type CompExprBuilder() =
        member inline b.Return (x)        = ret x
        member inline b.Bind (p, rest)    = p |> bind rest
        member inline b.Let (p, rest)     = rest p
        member inline b.ReturnFrom (expr) = expr
        
    let builder = new CompExprBuilder()
    




