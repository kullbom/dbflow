namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-index-columns-transact-sql?view=sql-server-ver17

type INDEX_COLUMN = {
    object : OBJECT
    index_id : int
    index_column_id : int
    column : COLUMN
    key_ordinal : byte option
    partition_ordinal : byte option
    is_descending_key : bool
    is_included_column : bool
}

module INDEX_COLUMN =
    let readAll' objects columns connection =
        DbTr.reader 
            "SELECT 
                 ic.object_id, ic.index_id, ic.index_column_id, ic.column_id, ic.key_ordinal, ic.partition_ordinal,
                 ic.is_descending_key, ic.is_included_column
             FROM sys.index_columns ic"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                let column_id = readInt32 "column_id" r
                let object : OBJECT = RCMap.pick object_id objects
                match object.ObjectType with
                | ObjectType.InternalTable 
                | ObjectType.SystemTable
                    -> acc
                | _ ->
                    let column = RCMap.pick (object_id, column_id) columns
                    {
                        object = object
                        index_id = readInt32 "index_id" r
                        index_column_id = readInt32 "index_column_id" r
                        column = column
                        key_ordinal = match readByte "key_ordinal" r with 0uy -> None | o -> Some o
                        partition_ordinal = match readByte "partition_ordinal" r with 0uy -> None | o -> Some o
                        is_descending_key = readBool "is_descending_key" r
                        is_included_column = readBool "is_included_column" r
                    } :: acc)
            []
        |> DbTr.commit_ connection
       
    let readAll objects columns connection =
        let indexColumns = readAll' objects columns connection
        let indexColumnsByIndex =
            indexColumns
            |> List.groupBy (fun c -> c.object.ObjectId, c.index_id)
            |> List.map 
                (fun (key, cs) -> 
                    key, 
                    cs 
                    |> List.sortBy 
                        (fun c -> 
                            match c.key_ordinal, c.is_included_column with
                            | Some ordinal, false -> 0, int ordinal
                            | _ -> 1, c.index_column_id)
                    |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        indexColumnsByIndex
        


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-indexes-transact-sql?view=sql-server-ver17

[<RequireQualifiedAccess>]
type INDEX_TYPE =
    | HEAP
    | CLUSTERED
    | NONCLUSTERED
    | XML
    | SPATIAL
    | CLUSTERED_COLUMNSTORE // Applies to: SQL Server 2014 (12.x) and later.
    | NONCLUSTERED_COLUMNSTORE // Applies to: SQL Server 2012 (11.x) and later.
    //  NONCLUSTERED HASH indexes are supported only on memory-optimized tables. The sys.hash_indexes view shows the current hash indexes 
    // and the hash properties. For more information, see sys.hash_indexes (Transact-SQL). 
    | NONCLUSTERED_HASH // Applies to: SQL Server 2014 (12.x) and later.
    | JSON // Applies to: SQL Server 2025 (17.x) Preview

module INDEX_TYPE =
    let findIndexType index_type_code =
        match index_type_code with 
        | 0uy -> INDEX_TYPE.HEAP
        | 1uy -> INDEX_TYPE.CLUSTERED
        | 2uy -> INDEX_TYPE.NONCLUSTERED
        | 3uy -> INDEX_TYPE.XML
        | 4uy -> INDEX_TYPE.SPATIAL
        | 5uy -> INDEX_TYPE.CLUSTERED_COLUMNSTORE
        | 6uy -> INDEX_TYPE.NONCLUSTERED_COLUMNSTORE
        | 7uy -> INDEX_TYPE.NONCLUSTERED_HASH
        | 9uy -> INDEX_TYPE.JSON
        | c -> failwithf "Unknwon index type: %i" c


type INDEX = {
    parent : OBJECT // The object to which this index belongs
    object : OBJECT option

    name : string option
    index_id : int
    index_type : INDEX_TYPE
    is_unique : bool
    data_space_id : int
    ignore_dup_key : bool
    is_primary_key : bool
    IsSystemNamed : bool // not part of sys.indexes
    is_unique_constraint : bool
    fill_factor : byte
    is_padded : bool
    is_disabled : bool 
    is_hypothetical : bool 
    //is_ignored_in_optimization : bool // Introduced in 2017
    allow_row_locks : bool
    allow_page_locks : bool
    filter : string option 
    //suppress_dup_key_messages : bool  // Introduced in 2017
    //auto_created : bool // Introduced in 2017

    columns : INDEX_COLUMN array

    ms_description : string option
}

module INDEX =
    let readAll' objects indexColumnsByIndex ms_descriptions connection =
        DbTr.reader
            "SELECT 
                i.object_id parent_object_id, kc.object_id index_object_id, 
                i.name, i.index_id, i.type index_type, i.is_unique, i.data_space_id, i.ignore_dup_key, i.is_primary_key, i.is_unique_constraint,
                ISNULL(kc.is_system_named, 0) is_system_named,
                i.fill_factor, i.is_padded, i.is_disabled, i.is_hypothetical, /* i.is_ignored_in_optimization, */
                i.allow_row_locks, i.allow_page_locks, i.has_filter, i.filter_definition, i.compression_delay /*, i.suppress_dup_key_messages, i.auto_created */
             FROM sys.indexes i
             LEFT OUTER JOIN sys.key_constraints kc ON i.object_id = kc.parent_object_id AND i.name = kc.name"
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
                let ms_description =
                    match index_object_id, RCMap.tryPick (XPropertyClass.Index, parent_object_id, index_id) ms_descriptions with
                    | _, Some d -> Some d
                    | Some id, None -> RCMap.tryPick (XPropertyClass.ObjectOrColumn, id, 0) ms_descriptions
                    | _ -> None 
                {
                    parent = parent
                    object = object
                
                    name = nullable "name" readString r
                    index_id = index_id
                    index_type = INDEX_TYPE.findIndexType (readByte "index_type" r)
                    is_unique = readBool "is_unique" r
                    data_space_id = readInt32 "data_space_id" r
                    ignore_dup_key = readBool "ignore_dup_key" r
                    is_primary_key = readBool "is_primary_key" r
                    IsSystemNamed = readBool "is_system_named" r
                    is_unique_constraint = readBool "is_unique_constraint" r
                    fill_factor = readByte "fill_factor" r
                    is_padded = readBool "is_padded" r
                    is_disabled = readBool "is_disabled" r 
                    is_hypothetical = readBool "is_hypothetical" r
                    //is_ignored_in_optimization = readBool "is_ignored_in_optimization" r
                    allow_row_locks = readBool "allow_row_locks" r
                    allow_page_locks = readBool "allow_page_locks" r
                    filter =
                        match readBool "has_filter" r with
                        | true -> readString "filter_definition" r |> Some
                        | false -> None
                    //suppress_dup_key_messages = readBool "suppress_dup_key_messages" r
                    //auto_created = readBool "auto_created" r

                    columns = columns

                    ms_description = ms_description
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll objects indexColumnsByIndex ms_descriptions connection =
        let indexes = readAll' objects indexColumnsByIndex ms_descriptions connection 
        let indexesByParent =
            indexes
            |> List.groupBy (fun i -> i.parent.ObjectId)
            |> List.map (fun (objectId, is) -> objectId, is |> List.sortBy (fun i -> i.index_id) |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        indexesByParent