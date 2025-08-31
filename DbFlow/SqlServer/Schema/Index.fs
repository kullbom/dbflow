namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-index-columns-transact-sql?view=sql-server-ver17

type IndexColumn = {
    Object : OBJECT
    IndexId : int
    IndexColumnId : int
    Column : Column
    KeyOrdinal : byte option
    PartitionOrdinal : byte option
    IsDescendingKey : bool
    IsIncludedColumn : bool
}

module IndexColumn =
    let readAll' objects columns connection =
        DbTr.reader 
            "SELECT 
                 ic.object_id, ic.index_id, ic.index_column_id, ic.column_id, ic.key_ordinal, ic.partition_ordinal,
                 ic.is_descending_key, ic.is_included_column
             FROM sys.index_columns ic"
            []
            (fun acc r -> 
                let objectId = readInt32 "object_id" r
                let columnId = readInt32 "column_id" r
                let object : OBJECT = RCMap.pick objectId objects
                match object.ObjectType with
                | ObjectType.InternalTable 
                | ObjectType.SystemTable
                    -> acc
                | _ ->
                    let column = RCMap.pick (objectId, columnId) columns
                    {
                        Object = object
                        IndexId = readInt32 "index_id" r
                        IndexColumnId = readInt32 "index_column_id" r
                        Column = column
                        KeyOrdinal = match readByte "key_ordinal" r with 0uy -> None | o -> Some o
                        PartitionOrdinal = match readByte "partition_ordinal" r with 0uy -> None | o -> Some o
                        IsDescendingKey = readBool "is_descending_key" r
                        IsIncludedColumn = readBool "is_included_column" r
                    } :: acc)
            []
        |> DbTr.commit_ connection
       
    let readAll objects columns connection =
        let indexColumns = readAll' objects columns connection
        let indexColumnsByIndex =
            indexColumns
            |> List.groupBy (fun c -> c.Object.ObjectId, c.IndexId)
            |> List.map 
                (fun (key, cs) -> 
                    key, 
                    cs 
                    |> List.sortBy 
                        (fun c -> 
                            match c.KeyOrdinal, c.IsIncludedColumn with
                            | Some ordinal, false -> 0, int ordinal
                            | _ -> 1, c.IndexColumnId)
                    |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        indexColumnsByIndex
        


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-indexes-transact-sql?view=sql-server-ver17
// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-key-constraints-transact-sql?view=sql-server-ver17
// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-xml-indexes-transact-sql?view=sql-server-ver17

[<RequireQualifiedAccess>]
type IndexType =
    | Heap
    | Clustered
    | Nonclustered
    | Xml
    | Spatial
    | ClusteredColumnstore // Applies to: SQL Server 2014 (12.x) and later.
    | NonclusteredColumnstore // Applies to: SQL Server 2012 (11.x) and later.
    //  NONCLUSTERED HASH indexes are supported only on memory-optimized tables. The sys.hash_indexes view shows the current hash indexes 
    // and the hash properties. For more information, see sys.hash_indexes (Transact-SQL). 
    | NonclusteredHash // Applies to: SQL Server 2014 (12.x) and later.
    | Json // Applies to: SQL Server 2025 (17.x) Preview

module IndexType =
    let findIndexType index_type_code =
        match index_type_code with 
        | 0uy -> IndexType.Heap
        | 1uy -> IndexType.Clustered
        | 2uy -> IndexType.Nonclustered
        | 3uy -> IndexType.Xml
        | 4uy -> IndexType.Spatial
        | 5uy -> IndexType.ClusteredColumnstore
        | 6uy -> IndexType.NonclusteredColumnstore
        | 7uy -> IndexType.NonclusteredHash
        | 9uy -> IndexType.Json
        | c -> failwithf "Unknwon index type: %i" c

type XmlIndexSecondaryType = { PrimaryIndexName : string; SecondaryType : string }

type Index = {
    Parent : OBJECT // The object to which this index belongs
    Object : OBJECT option

    Name : string option
    IndexId : int
    IndexType : IndexType
    IsUnique : bool
    DataSpaceId : int
    IgnoreDupKey : bool
    IsPrimaryKey : bool
    IsSystemNamed : bool // not part of sys.indexes
    IsUniqueConstraint : bool
    FillFactor : byte
    IsPadded : bool
    IsDisabled : bool 
    IsHypothetical : bool 
    //is_ignored_in_optimization : bool // Introduced in 2017
    AllowRowLocks : bool
    AllowPageLocks : bool
    Filter : string option 
    //suppress_dup_key_messages : bool  // Introduced in 2017
    //auto_created : bool // Introduced in 2017
    
    XmlIndexSecondaryType : XmlIndexSecondaryType option
    
    Columns : IndexColumn array

    XProperties : Map<string, string>
}

module Index =
    let readAll' objects indexColumnsByIndex xProperties connection =
        DbTr.reader
            "SELECT 
                i.object_id parent_object_id, kc.object_id index_object_id, 
                i.name, i.index_id, i.type index_type, i.is_unique, i.data_space_id, i.ignore_dup_key, i.is_primary_key, i.is_unique_constraint,
                ISNULL(kc.is_system_named, 0) is_system_named,
                i.fill_factor, i.is_padded, i.is_disabled, i.is_hypothetical, /* i.is_ignored_in_optimization, */
                i.allow_row_locks, i.allow_page_locks, i.has_filter, i.filter_definition, i.compression_delay, /*, i.suppress_dup_key_messages, i.auto_created */
                xi.secondary_type_desc AS xml_index_secondary_type_desc,
                pi.name AS xml_index_primary_index_name
             FROM sys.indexes i
             LEFT OUTER JOIN sys.key_constraints kc ON i.object_id = kc.parent_object_id AND i.name = kc.name
             LEFT OUTER JOIN sys.xml_indexes xi ON i.object_id = xi.object_id AND i.index_id = xi.index_id
             LEFT OUTER JOIN sys.indexes pi ON pi.object_id = xi.object_id AND pi.index_id = xi.using_xml_index_id"
            []
            (fun acc r -> 
                let index_object_id = nullable "index_object_id" readInt32 r
                let parent_object_id = readInt32 "parent_object_id" r
                let index_id = readInt32 "index_id" r
                let parent = RCMap.pick parent_object_id objects
                let object = 
                    match index_object_id with
                    | Some id -> RCMap.pick id objects |> Some
                    | None -> None
                let columns = 
                    match RCMap.tryPick (parent_object_id, index_id) indexColumnsByIndex with
                    | Some cs -> cs 
                    | None -> [||]
                let xProperties =
                    let x0 = XProperty.getXProperties (XPropertyClass.Index, parent_object_id, index_id) xProperties
                    match index_object_id with
                    | None -> x0
                    | Some id -> 
                        Map.fold (fun m k v -> Map.add k v m) x0
                            (XProperty.getXProperties (XPropertyClass.ObjectOrColumn, id, 0) xProperties)
                {
                    Parent = parent
                    Object = object
                
                    Name = nullable "name" readString r
                    IndexId = index_id
                    IndexType = IndexType.findIndexType (readByte "index_type" r)
                    IsUnique = readBool "is_unique" r
                    DataSpaceId = readInt32 "data_space_id" r
                    IgnoreDupKey = readBool "ignore_dup_key" r
                    IsPrimaryKey = readBool "is_primary_key" r
                    IsSystemNamed = readBool "is_system_named" r
                    IsUniqueConstraint = readBool "is_unique_constraint" r
                    FillFactor = readByte "fill_factor" r
                    IsPadded = readBool "is_padded" r
                    IsDisabled = readBool "is_disabled" r 
                    IsHypothetical = readBool "is_hypothetical" r
                    //is_ignored_in_optimization = readBool "is_ignored_in_optimization" r
                    AllowRowLocks = readBool "allow_row_locks" r
                    AllowPageLocks = readBool "allow_page_locks" r
                    Filter =
                        match readBool "has_filter" r with
                        | true -> readString "filter_definition" r |> Some
                        | false -> None
                    //suppress_dup_key_messages = readBool "suppress_dup_key_messages" r
                    //auto_created = readBool "auto_created" r
                    XmlIndexSecondaryType = 
                        match nullable "xml_index_primary_index_name" readString r with
                        | None -> None
                        | Some piName ->
                            let sType = readString "xml_index_secondary_type_desc" r 
                            Some { SecondaryType = sType; PrimaryIndexName = piName }
                    Columns = columns

                    XProperties = xProperties
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll objects indexColumnsByIndex xProperties connection =
        let indexes = readAll' objects indexColumnsByIndex xProperties connection 
        let indexesByParent =
            indexes
            |> List.groupBy (fun i -> i.Parent.ObjectId)
            |> List.map (fun (objectId, is) -> objectId, is |> List.sortBy (fun i -> i.IndexId) |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        indexesByParent