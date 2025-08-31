module DbFlow.SqlServer.CopyData

open DbFlow
open DbFlow.SqlServer
open DbFlow.Readers

open Microsoft.Data.SqlClient
//open SqLucid


type DataKey = obj array

/// Reference to a number of rows of a given table
type DataReference = {
    Id : string 
    Table : Schema.Table
    KeyColumns : Schema.Column array
    DataKeys : DataKey list 
}

let bulkOptions = SqlBulkCopyOptions.CheckConstraints ||| SqlBulkCopyOptions.KeepIdentity

let tableName (t : Schema.Table) = $"[{t.Schema.Name}].[{t.Name}]"

let dataRefToTempTable' tableName (dataRef : DataReference) =
    DbTr (fun ctx ->
        let tr = match ctx.Transaction with Some t -> t | None -> null
        use bc = new SqlBulkCopy(ctx.Connection :?> SqlConnection, bulkOptions, tr :?> SqlTransaction)
        bc.DestinationTableName <- tableName
        bc.BulkCopyTimeout <- 60 * 60 

        for c in dataRef.KeyColumns do
            bc.ColumnMappings.Add (c.Name, c.Name) |> ignore

        use dataTable = new System.Data.DataTable()
        for c in dataRef.KeyColumns do
            dataTable.Columns.Add(new System.Data.DataColumn(c.Name))

        for d in dataRef.DataKeys do 
            let dataRow = dataTable.NewRow()
            dataRow.ItemArray <- d
            dataTable.Rows.Add(dataRow)

        bc.WriteToServer dataTable
        bc.RowsCopied)

let dataRefToTempTable (dataRef : DataReference) =
    let tempTableName = $"{System.Guid.NewGuid()}".Replace("-", "")
    let columnDefs = 
        dataRef.KeyColumns 
        |> Array.joinBy 
            ",\r\n" 
            (fun c -> 
                let dataType = c.Datatype
                let typeStr = Schema.Datatype.typeStr dataType
                $"{c.Name} {typeStr}")
    let createTempTable = DbTr.nonQuery $"CREATE TABLE #{tempTableName} ({columnDefs});" []
    let insertKeys = dataRefToTempTable' $"#{tempTableName}" dataRef

    DbTr.zip createTempTable insertKeys
    |> DbTr.map (fun (_, nCopied) -> nCopied, tempTableName)
    

let readTableKeys (logger : Logger) (columns : Schema.Column array) (dataRef : DataReference) : DbTr<DataKey list> =
    let tableName = $"[{dataRef.Table.Schema.Name}].[{dataRef.Table.Name}]"
    let columnsStr = columns |> Array.joinBy ", " (fun c -> $"source.[{c.Name}]")
    let joinCondition = 
        dataRef.KeyColumns 
        |> Array.joinBy " AND " (fun c -> $"source.[{c.Name}] = keys.[{c.Name}]")
    let whereCondition = 
        columns 
        |> Array.joinBy " AND " (fun c -> $"source.[{c.Name}] IS NOT NULL")
        |> function "" -> "" | c -> $"\r\nWHERE {c}"
    dataRefToTempTable dataRef
    |> DbTr.bind
        (fun (_nCopied, tempTableName) ->
            let cmdText = 
                $"SELECT {columnsStr} 
                  FROM #{tempTableName} keys
                  INNER JOIN {tableName} source ON {joinCondition}{whereCondition}"
            DbTr.readList cmdText []
                (fun r -> columns |> Array.map (fun c -> readObject c.Name r)))

let copyTableData' (dataRef : DataReference) (sourceConnection : System.Data.IDbConnection) (targetConnection : System.Data.IDbConnection) =
    let tableName = $"[{dataRef.Table.Schema.Name}].[{dataRef.Table.Name}]"
    let allColumnsStr = dataRef.Table.Columns |> Array.joinBy ", " (fun c -> $"source.[{c.Name}]")
    let joinCondition = 
        dataRef.KeyColumns 
        |> Array.joinBy " AND " (fun c -> $"source.[{c.Name}] = keys.[{c.Name}]")
    dataRefToTempTable dataRef
    |> DbTr.bind
        (fun (_nCopied, tempTableName) ->
            DbTr.reader'
                $"SELECT {allColumnsStr}
                  FROM #{tempTableName} keys
                  INNER JOIN {tableName} source ON {joinCondition}"
                []
                (fun dataReader ->            
                    // Ensure SET ANSI_NULLS ON 
                    DbTr.nonQuery "SET ANSI_NULLS ON" [] |> DbTr.exe targetConnection
                    DbTr.nonQuery "SET QUOTED_IDENTIFIER ON" [] |> DbTr.exe targetConnection

                    // Now use the reader to pass data to the bulk insert 
                    use bc = new SqlBulkCopy(targetConnection :?> SqlConnection, bulkOptions, null) // NOT part of the source transaction
                    bc.DestinationTableName <- tableName
                    bc.BulkCopyTimeout <- 60 * 60 

                    for c in dataRef.Table.Columns do
                        bc.ColumnMappings.Add (c.Name, c.Name) |> ignore

                    bc.WriteToServer dataReader
                    bc.RowsCopied))
    |> DbTr.commit_ sourceConnection

let rec collectDataRefs (logger : Logger) indent collected allTables (dataRef : DataReference) (sourceConnection : System.Data.IDbConnection) =
    let table = dataRef.Table
    table.ForeignKeys
    |> Array.fold 
        (fun collected' fk ->
            let fkColumns = fk.Columns |> Array.map (fun c -> c.ParentColumn)
            let sw = System.Diagnostics.Stopwatch ()
            sw.Start ()
            let dataKeys' = 
                readTableKeys logger fkColumns dataRef
                |> DbTr.commit_ sourceConnection
            let dataKeys = dataKeys' |> List.distinct
            
            logger.info $"{indent}{tableName table} depends on {tableName (allTables fk.Referenced.ObjectId)} : {dataKeys'.Length} ({dataKeys.Length} unique) keys found (in {sw.ElapsedMilliseconds} ms)"
            
            match dataKeys with
            | [] -> collected'
            | _ ->
                let fkDataRef =
                    {
                        Id = $"{dataRef.Id}>{fk.Name}"
                        Table = allTables fk.Referenced.ObjectId
                        KeyColumns = fk.Columns |> Array.map (fun c -> c.ReferencedColumn)
                        DataKeys = dataKeys
                    }
                collectDataRefs logger (indent + "   ") collected' allTables fkDataRef sourceConnection)
        (dataRef :: collected)
    
let copyData (logger : Logger) (schema : Schema.DatabaseSchema) (origDataRef : DataReference) (sourceConnection : System.Data.IDbConnection) (targetConnection : System.Data.IDbConnection) =
    let allTables =
        schema.Tables
        |> List.map (fun t -> t.Object.ObjectId, t)
        |> Map.ofList
        |> fun tables object_id -> 
            match Map.tryFind object_id tables with
            | Some t -> t
            | None -> failwith $"Table with object_id {object_id} not found in schema."
    
    // Collect all data that needs to be copied following foreign keys of the tables
    logger.info $"Collect data dependencies of {tableName origDataRef.Table}..."
    let allDataRefs =
        collectDataRefs logger "   " [] allTables origDataRef sourceConnection
    
    logger.info $"Resolving order dependency of {allDataRefs.Length} set(s) of data..."
    let dependentDataRefs = 
        allDataRefs
        |> List.groupBy (fun dr -> dr.Table.Object.ObjectId)
        |> List.map 
            (fun (table_object_id, drs) -> 
                let contains = [table_object_id]
                let dependsOn = 
                    drs 
                    |> List.fold 
                        (fun acc dr -> 
                            dr.Table.ForeignKeys 
                            |> Array.fold (fun acc' fk -> fk.Referenced.ObjectId :: acc') acc)
                        []
                Dependent.create (table_object_id, drs) contains dependsOn 0)
        |> Dependent.resolveOrder (fun d -> fst d.Content) Map.empty 
            
    for x in dependentDataRefs do
        let sw = System.Diagnostics.Stopwatch ()
        sw.Start ()
        let (df0, dfAll) = 
            match x.Content with
            | _, (df0 :: _ as dfAll) -> df0, dfAll
            | _ -> failwithf "Empty group - shoult not be possible"
        // PROBLEM: It is _possible_ that the KeyColumns is NOT the same here... 
        //   If that is the case the data is rejected for now
        let allDataKeys =
            dfAll
            |> List.collect
                (fun df -> 
                    if df.KeyColumns <> df0.KeyColumns
                    then failwithf "Key columns does not match"
                    df.DataKeys)
            |> List.distinct
        let dataRef = { Id = "Syntetic"; Table = df0.Table; KeyColumns = df0.KeyColumns; DataKeys = allDataKeys } 
        logger.info $"Copy {allDataKeys.Length} rows from {tableName dataRef.Table} ..."
        let n = copyTableData' dataRef sourceConnection targetConnection
        logger.info $"Copied {n} rows from {tableName dataRef.Table} in {sw.ElapsedMilliseconds} ms"


let TopN (table : Schema.Table) (topN : int) =
    let pk = 
        table.Indexes |> Array.tryFind (fun ix -> ix.IsPrimaryKey)
        |> function Some pk -> pk | None -> failwith "A primary key is required for {tableName table}"
        
    let keyColumnNames = pk.Columns |> Array.map (fun c -> c.Column.Name)
    let keyColumnsStr = keyColumnNames |> Array.joinBy ", " (fun columnName -> $"[{columnName}]")
    
    DbTr.readList 
        $"SELECT TOP ({topN}) {keyColumnsStr} FROM {tableName table}" []
        (fun r -> keyColumnNames |> Array.map (fun columnName -> readObject columnName r))
    |> DbTr.map (fun dataKeys -> 
        {
            Id = $"TOP {topN} FROM {tableName table}"
            Table = table
            KeyColumns = pk.Columns |> Array.map (fun c -> c.Column)
            DataKeys = dataKeys
        })