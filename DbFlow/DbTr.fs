namespace DbFlow

open System
open System.Data

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
type DbContext = { Connection : IDbConnection; Transaction : IDbTransaction option }
type 'a DbTr = DbTr of (DbContext -> 'a)

module DbTr =
    let map f (DbTr t) = DbTr (fun ctx -> f (t ctx))

    let bind f (DbTr t) = DbTr (fun ctx -> match f (t ctx) with DbTr t' -> t' ctx)
    
    let ret x = DbTr (fun _ctx -> x)


    let map2 f t0 t1 = t0 |> bind (fun x0 -> t1 |> map (f x0))
    
    let zip t0 t1 = map2 (fun x0 x1 -> x0, x1) t0 t1

    let map3 f t0 t1 t2 = zip t0 t1 |> bind (fun (x0, x1) -> t2 |> map (f x0 x1))

    let zip3 t0 t1 t2 = map3 (fun x0 x1 x2 -> x0, x1, x2) t0 t1 t2

    let map4 f t0 t1 t2 t3 = zip3 t0 t1 t2 |> bind (fun (x0, x1, x2) -> t3 |> map (f x0 x1 x2))

    let zip4 t0 t1 t2 t3 = map4 (fun x0 x1 x2 x3 -> x0, x1, x2, x3) t0 t1 t2 t3

    let sequence_ ts =
        let fns = ts |> List.map (fun (DbTr f) -> f)
        DbTr (fun ctx -> for fn in fns do fn ctx)

    let nonQuery cmdText parameters =
        DbTr
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

    /// Custom reader - traversing the data reader is up to the user    
    let reader' cmdText parameters f =
        DbTr
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

    let commit (c : IDbConnection) (i : System.Data.IsolationLevel) (DbTr t) =
        let tr = c.BeginTransaction(i)
        let mutable success = false
        try 
            let res = t { Connection = c; Transaction = Some tr }
            tr.Commit ()
            success <- true
            res
        finally
            if not success
            then tr.Rollback ()
        
    let commit_ c t = commit c System.Data.IsolationLevel.ReadCommitted t

    let exe (c : IDbConnection) (DbTr t) =
        t { Connection = c; Transaction = None }

        
        
    




