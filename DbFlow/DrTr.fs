namespace DbFlow

open System

// A small db library on top of the IO abstraction

module Readers =
    let readObject n (r : System.Data.IDataReader) = r.GetValue (r.GetOrdinal n)
    let readString n (r : System.Data.IDataReader) = r.GetString (r.GetOrdinal n) 
    let readBool n (r : System.Data.IDataReader) = r.GetBoolean (r.GetOrdinal n)
    let readByte n (r : System.Data.IDataReader) = r.GetByte (r.GetOrdinal n)
    let readInt16 n (r : System.Data.IDataReader) = r.GetInt16 (r.GetOrdinal n)
    let readInt32 n (r : System.Data.IDataReader) = r.GetInt32 (r.GetOrdinal n)
    let readDateTime n (r : System.Data.IDataReader) = r.GetDateTime (r.GetOrdinal n)
    let nullable n f (r : System.Data.IDataReader) = if r.IsDBNull (r.GetOrdinal n) then None else Some (f n r)


type DbContext = { 
    Connection : System.Data.Common.DbConnection; 
    Transaction : System.Data.Common.DbTransaction option 
}


type DbTr<'a> = IO<DbContext,'a>

module DbTr =
    let internal nonQuery' cmdText parameters executor ctx =
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
            executor cmd
        with e ->
            raise (Exception($"Batch failed: {cmdText}", e))
            
    let nonQuery cmdText parameters =
        IO.create  
            (nonQuery' cmdText parameters (fun cmd -> cmd.ExecuteNonQuery() |> ignore))
            (nonQuery' cmdText parameters (fun cmd -> cmd.ExecuteNonQueryAsync() |> Task.map ignore))
        
    let internal reader'' cmdText parameters executor ctx = 
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
        executor cmd

    /// Custom reader - traversing the data reader is up to the user    
    let reader' cmdText parameters f =
        IO.create 
            (reader'' cmdText parameters 
                (fun cmd -> use dataReader = cmd.ExecuteReader() in f dataReader))
            (reader'' cmdText parameters 
                (fun cmd -> 
                    task {
                        use! dataReader = cmd.ExecuteReaderAsync() 
                        return f dataReader
                    }))
        

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
    let commit (c : System.Data.Common.DbConnection) (i : System.Data.IsolationLevel) (t : DbTr<'a>) =
        IO.create 
            (fun () ->
                let tr = c.BeginTransaction(i)
                let mutable success = false
                try 
                    let res = IO.run' t { Connection = c; Transaction = Some tr }
                    tr.Commit ()
                    success <- true
                    res
                finally
                    if not success
                    then tr.Rollback ())
            (fun () -> 
                task {
                    let! tr = c.BeginTransactionAsync(i)
                    let mutable success = false
                    try 
                        let! res = IO.runAsync' t { Connection = c; Transaction = Some tr }
                        do! tr.CommitAsync ()
                        success <- true
                        return res
                    finally
                        if not success
                        then tr.Rollback ()
                })

    /// Commit a transaction with isolation level READ COMMITED
    let commit_ c t = commit c System.Data.IsolationLevel.ReadCommitted t

    /// Execute a transaction {t} without an actual database transaction
    let exe (c : System.Data.Common.DbConnection) (t : DbTr<'a>) : IO<unit, 'a> =
        IO.create
            (fun () -> IO.run' t { Connection = c; Transaction = None })
            (fun () -> IO.runAsync' t { Connection = c; Transaction = None })





