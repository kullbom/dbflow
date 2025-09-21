module DbFlow.SqlServer.CopyData

open DbFlow
open DbFlow.SqlServer
open DbFlow.SqlServer.Schema
open DbFlow.Readers

open Microsoft.Data.SqlClient

type CopyMethod = 
    /// Plain insert of all source rows to target. Conflicts will fail with exception.
    | InsertCopy 
    /// Insert missing rows and updates existing rows. Slower. 
    | UpsertCopy

type DataKey = obj array

/// Reference to a number of rows of a given table
type DataReference = {
    Id : string 
    Table : Table
    KeyColumns : Column array
    DataKeys : DataKey list 
}

module Internal =

    let bulkOptions = SqlBulkCopyOptions.CheckConstraints ||| SqlBulkCopyOptions.KeepIdentity
    
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
    
    let createTempTable (allTypes : Map<int, Datatype>) tempTableName (columns : Column array) = 
        let columnDefs = 
            columns 
            |> Array.joinBy 
                ",\r\n" 
                (fun c -> 
                    let dataType = c.Datatype
                    let nullStr = if c.Datatype.Parameter.IsNullable then "NULL" else "NOT NULL"
                    let typeStr = 
                        match dataType.DatatypeSpec with
                        // Temp tables can not contain user defined types
                        | UserDefined ->  
                            let baseType = allTypes |> Map.find (int dataType.SystemTypeId)
                            Schema.Datatype.typeStr' baseType.Name baseType.DatatypeSpec dataType.Parameter
                        | _ ->  Schema.Datatype.typeStr dataType
                    $"{c.Name} {typeStr} {nullStr}")
        DbTr.nonQuery $"CREATE TABLE #{tempTableName} ({columnDefs});" []
        
    let dataRefToTempTable allTypes (dataRef : DataReference) =
        let tempTableName = $"{System.Guid.NewGuid()}".Replace("-", "")
        let createTempTableTr = createTempTable allTypes tempTableName dataRef.KeyColumns
        let insertKeysTr = dataRefToTempTable' $"#{tempTableName}" dataRef
    
        DbTr.zip createTempTableTr insertKeysTr
        |> DbTr.map (fun (_, nCopied) -> nCopied, tempTableName)
        
    
    let readTableKeys (logger : Logger) allTypes (columns : Schema.Column array) (dataRef : DataReference) : DbTr<DataKey list> =
        let tableName = $"[{dataRef.Table.Schema.Name}].[{dataRef.Table.Name}]"
        let columnsStr = columns |> Array.joinBy ", " (fun c -> $"source.[{c.Name}]")
        let joinCondition = 
            dataRef.KeyColumns 
            |> Array.joinBy " AND " (fun c -> $"source.[{c.Name}] = keys.[{c.Name}]")
        let whereCondition = 
            columns 
            |> Array.joinBy " AND " (fun c -> $"source.[{c.Name}] IS NOT NULL")
            |> function "" -> "" | c -> $"\r\nWHERE {c}"
        dataRefToTempTable allTypes dataRef
        |> DbTr.bind
            (fun (_nCopied, tempTableName) ->
                let cmdText = 
                    $"SELECT {columnsStr} 
                      FROM #{tempTableName} keys
                      INNER JOIN {tableName} source ON {joinCondition}{whereCondition}"
                DbTr.readList cmdText []
                    (fun r -> columns |> Array.map (fun c -> readObject c.Name r)))
    
    let copyTableData'' allTypes (dataRef : DataReference) (onSource : DbTr<'a> -> 'a) (targetTableName : string) (onTarget : DbTr<int> -> 'a) =
        let sourceTableName = $"[{dataRef.Table.Schema.Name}].[{dataRef.Table.Name}]"
        let allColumns = 
            // Exclude computed columns
            dataRef.Table.Columns |> Array.filter (fun c -> c.ComputedDefinition.IsNone)
        let allColumnsStr = allColumns |> Array.joinBy ", " (fun c -> $"source.[{c.Name}]")
        let joinCondition = 
            dataRef.KeyColumns 
            |> Array.joinBy " AND " (fun c -> $"source.[{c.Name}] = keys.[{c.Name}]")
        dataRefToTempTable allTypes dataRef
        |> DbTr.bind
            (fun (_nCopied, tempTableName) ->
                DbTr.reader'
                    $"SELECT {allColumnsStr}
                      FROM #{tempTableName} keys
                      INNER JOIN {sourceTableName} source ON {joinCondition}"
                    []
                    (fun dataReader ->            
                        // Now use the reader to pass data to the bulk insert 
                        // NOT part of the source transaction
                        let bulkInsertTransaction =
                            DbTr (fun c ->
                                let dbTransaction =
                                    match c.Transaction with
                                    | None -> null 
                                    | Some tr -> tr :?> SqlTransaction
                                use bc = new SqlBulkCopy(c.Connection :?> SqlConnection, bulkOptions, dbTransaction)
                                bc.DestinationTableName <- targetTableName
                                bc.BulkCopyTimeout <- 60 * 60 
    
                                for c in allColumns do
                                    bc.ColumnMappings.Add (c.Name, c.Name) |> ignore
    
                                bc.WriteToServer dataReader
                                bc.RowsCopied)
                        bulkInsertTransaction 
                        |> onTarget))
        |> onSource
    
    let copyTableData' allTypes (dataRef : DataReference) (copyMethod : CopyMethod) (sourceConnection : System.Data.IDbConnection) (targetConnection : System.Data.IDbConnection) =
        match copyMethod with
        | InsertCopy ->
            // Plain insert to the target table
            let targetTableName = $"[{dataRef.Table.Schema.Name}].[{dataRef.Table.Name}]"
            // Ensure SET ANSI_NULLS ON 
            DbTr.nonQuery "SET ANSI_NULLS ON" [] |> DbTr.exe targetConnection
            DbTr.nonQuery "SET QUOTED_IDENTIFIER ON" [] |> DbTr.exe targetConnection
            copyTableData'' allTypes dataRef (DbTr.commit_ sourceConnection) targetTableName (DbTr.commit_ targetConnection)
        | UpsertCopy ->
            // Upsert
            let targetTableName = $"[{dataRef.Table.Schema.Name}].[{dataRef.Table.Name}]"
            let tempTableName = $"{System.Guid.NewGuid()}".Replace("-", "")
            copyTableData'' allTypes dataRef 
                (DbTr.commit_ sourceConnection)
                tempTableName
                (fun copyTr -> 
                    DbTr.builder {
                        // 1. Create temp table
                        let columns = 
                            // Exclude computed columns
                            dataRef.Table.Columns |> Array.filter (fun c -> c.ComputedDefinition.IsNone)
                        do! createTempTable allTypes tempTableName columns
                        // 2. Copy to temp table
                        let! nRowsCopied = copyTr
                        // 3. Update existing rows
                        let keyColumns = dataRef.KeyColumns |> Array.map (fun c -> c.Name) |> Set.ofArray
                        let joinCondition =
                            dataRef.KeyColumns
                            |> Array.joinBy " AND " (fun c -> $"temp.[{c.Name}] = target.[{c.Name}]") 
                        let setColumnsTarget = 
                            columns 
                            |> Array.filter (fun c -> not <| Set.contains c.Name keyColumns)
                            |> Array.joinBy ", " (fun c -> $"[{c.Name}] = temp.[{c.Name}]")
                        do! DbTr.nonQuery 
                                $"UPDATE target SET {setColumnsTarget}
                                  FROM #{tempTableName} temp
                                  INNER JOIN {targetTableName} target ON {joinCondition}"
                                []
                        // 4. Insert new rows
                        let allColumnsTarget = columns |> Array.joinBy ", " (fun c -> $"[{c.Name}]")
                        let allColumnsSelect = columns |> Array.joinBy ", " (fun c -> $"temp.[{c.Name}]")
                        do! DbTr.nonQuery 
                                $"SET IDENTITY_INSERT {targetTableName} ON;
                                  
                                  INSERT INTO {targetTableName} ({allColumnsTarget})
                                  SELECT {allColumnsSelect} 
                                  FROM #{tempTableName} temp
                                  LEFT OUTER JOIN {targetTableName} target ON {joinCondition}
                                  WHERE target.[{dataRef.KeyColumns.[0].Name}] IS NULL
                                  
                                  SET IDENTITY_INSERT {targetTableName} OFF;"
                                []
                        return nRowsCopied
                    }
                    |> DbTr.commit_ targetConnection)
    
    let rec collectDataRefs (logger : Logger) allTypes indent collected allTables (dataRef : DataReference) (sourceConnection : System.Data.IDbConnection) =
        let table = dataRef.Table
        table.ForeignKeys
        |> Array.fold 
            (fun collected' fk ->
                let fkColumns = fk.Columns |> Array.map (fun c -> c.ParentColumn)
                let sw = System.Diagnostics.Stopwatch ()
                sw.Start ()
                let dataKeys' = 
                    readTableKeys logger allTypes fkColumns dataRef
                    |> DbTr.commit_ sourceConnection
                let dataKeys = dataKeys' |> List.distinct
                
                logger.info $"{indent}{Table.fullName table} depends on {Table.fullName (allTables fk.Referenced.ObjectId)} : {dataKeys'.Length} ({dataKeys.Length} unique) keys found (in {sw.ElapsedMilliseconds} ms)"
                
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
                    collectDataRefs logger allTypes (indent + "   ") collected' allTables fkDataRef sourceConnection)
            (dataRef :: collected)
        
/// Copies all data referenced by {origDataRef} from one database to another
let copyData (logger : Logger) (schema : Schema.DatabaseSchema) (origDataRef : DataReference) copyMethod (sourceConnection : System.Data.IDbConnection) (targetConnection : System.Data.IDbConnection) =
    let allTables =
        schema.Tables
        |> List.map (fun t -> t.Object.ObjectId, t)
        |> Map.ofList
        |> fun tables object_id -> 
            match Map.tryFind object_id tables with
            | Some t -> t
            | None -> failwith $"Table with object_id {object_id} not found in schema."
    
    let allTypes = 
        schema.Types
        |> List.map (fun t -> t.UserTypeId, t)
        |> Map.ofList

    // Collect all data that needs to be copied following foreign keys of the tables
    logger.info $"Collect data dependencies of {Table.fullName origDataRef.Table}..."
    let allDataRefs =
        Internal.collectDataRefs logger allTypes "   " [] allTables origDataRef sourceConnection
    
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
        logger.info $"Copy {allDataKeys.Length} rows from {Schema.Table.fullName dataRef.Table} ..."
        let n = Internal.copyTableData' allTypes dataRef copyMethod sourceConnection targetConnection
        logger.info $"Copied {n} rows from {Table.fullName dataRef.Table} in {sw.ElapsedMilliseconds} ms"


/// Creates a DataReference for TOP {topN} rows of the given table (by the primary key)  
let TopN (table : Schema.Table) (topN : int) =
    let pk = 
        table.Indexes |> Array.tryFind (fun ix -> ix.IsPrimaryKey)
        |> function Some pk -> pk | None -> failwith "A primary key is required for {tableName table}"
        
    let keyColumnNames = pk.Columns |> Array.map (fun c -> c.Column.Name)
    let keyColumnsStr = keyColumnNames |> Array.joinBy ", " (fun columnName -> $"[{columnName}]")
    
    DbTr.readList 
        $"SELECT TOP ({topN}) {keyColumnsStr} FROM {Table.fullName table}" []
        (fun r -> keyColumnNames |> Array.map (fun columnName -> readObject columnName r))
    |> DbTr.map (fun dataKeys -> 
        {
            Id = $"TOP {topN} FROM {Table.fullName table}"
            Table = table
            KeyColumns = pk.Columns |> Array.map (fun c -> c.Column)
            DataKeys = dataKeys
        })