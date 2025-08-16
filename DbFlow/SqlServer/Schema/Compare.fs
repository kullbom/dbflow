namespace DbFlow.SqlServer.Schema

open DbFlow

// In order to compare sequences we need a deterministic order of the elements
type SortOrder = SortOrder
    with 
        static member orderBy (x : SCHEMA) = x.name 
        static member orderBy (x : TABLE) = SortOrder.orderBy x.object 
        static member orderBy (x : VIEW) = SortOrder.orderBy x.object 
        static member orderBy (x : INDEX) = SortOrder.orderBy x.parent, x.index_id
        static member orderBy (x : COLUMN) = SortOrder.orderBy x.object, x.column_id 
        static member orderBy (x : FOREIGN_KEY) = SortOrder.orderBy x.object 
        static member orderBy (x : TRIGGER) = SortOrder.orderBy x.object, x.trigger_name 
        static member orderBy (x : PARAMETER) = SortOrder.orderBy x.object, x.name, x.parameter_id 
        static member orderBy (x : DEFAULT_CONSTRAINT) = SortOrder.orderBy x.object 
        static member orderBy (x : SYNONYM) = SortOrder.orderBy x.object
        static member orderBy (x : DATATYPE) = x.name
        static member orderBy (x : TABLE_TYPE) = x.type_name
        static member orderBy (x : PROCEDURE) = x.name 
        static member orderBy (x : XML_SCHEMA_COLLECTION) = SortOrder.orderBy x.schema, x.name 
        static member orderBy (x : DATABASE_TRIGGER) = x.trigger_name
        static member orderBy (x : CHECK_CONSTRAINT) = SortOrder.orderBy x.object
        static member orderBy (x : FOREIGN_KEY_COLUMN) = SortOrder.orderBy x.parent_column, SortOrder.orderBy x.referenced_column 
        static member orderBy (x : INDEX_COLUMN) = SortOrder.orderBy x.object, x.index_id, x.index_column_id
        static member orderBy (x : OBJECT) = SortOrder.orderBy x.schema, x.name, x.object_type
        static member orderBy (x : SEQUENCE) = SortOrder.orderBy x.object

type Sequence = Sequence
    with 
        static member elementId (x : TABLE) = x.table_name
        static member elementId (x : SCHEMA) = x.name 
        static member elementId (x : VIEW) = x.view_name
        static member elementId (x : INDEX) = match x.name with Some n -> n | None -> "???"
        static member elementId (x : COLUMN) = x.column_name
        static member elementId (x : FOREIGN_KEY) = x.name
        static member elementId (x : TRIGGER) = x.trigger_name
        static member elementId (x : PARAMETER) = x.name
        static member elementId (x : DEFAULT_CONSTRAINT) = "DF???" + x.column.column_name
        static member elementId (x : SYNONYM) = x.object.name
        static member elementId (x : DATATYPE) = x.name
        static member elementId (x : TABLE_TYPE) = x.type_name
        static member elementId (x : PROCEDURE) = x.name 
        static member elementId (x : XML_SCHEMA_COLLECTION) = x.name 
        static member elementId (x : DATABASE_TRIGGER) = x.trigger_name
        static member elementId (x : CHECK_CONSTRAINT) = x.object.name
        static member elementId (x : FOREIGN_KEY_COLUMN) = $"{x.parent_column.column_name} -> {x.referenced_column.column_name}"
        static member elementId (x : INDEX_COLUMN) = x.column.column_name
        static member elementId (x : OBJECT) = x.name
        static member elementId (x : SEQUENCE) = x.object.name

type Diff = { Message : string; Path : string }
    with static member create message path = { Message = message; Path = path |> List.rev |> List.toArray |> Array.joinBy "/" id }

module Compare =
    let collectOption (o0 : 'a option) (o1 : 'a option) (collector : 'a * 'a * _ * _ -> _) path diffs =
        match o0, o1 with
        | None, None -> diffs
        | Some x, Some y -> collector (x, y, path, diffs)
        | x, y -> Diff.create $"{x} != {y}" path :: diffs
    
    let collectList (l0 : 'a list) (l1 : 'a list) (orderBy : 'a -> 'k) (elementId : 'a -> string) (collector : 'a * 'a * _ * _ -> _) path diffs =
        if l0.Length <> l1.Length
        then Diff.create $"different length ({l0.Length} != {l1.Length})" path :: diffs
        else
            List.fold2 
                (fun diffs' x y -> collector (x, y, elementId x :: path, diffs'))
                diffs
                (l0 |> List.sortBy orderBy) 
                (l1 |> List.sortBy orderBy)
    
    let collectArray (l0 : 'a array) (l1 : 'a array) (orderBy : 'a -> 'k) (elementId : 'a -> string) (collector : 'a * 'a * _ * _ -> _) path diffs =
        if l0.Length <> l1.Length
        then Diff.create $"different length ({l0.Length} != {l1.Length})" path :: diffs
        else
            Array.fold2 
                (fun diffs' x y -> collector (x, y, elementId x :: path, diffs'))
                diffs
                (l0 |> Array.sortBy orderBy) 
                (l1 |> Array.sortBy orderBy)

    let equalCollector (x0 : _, x1 : _, path, diff) = 
        if x0 = x1 then diff else Diff.create $"{x0} != {x1}" path :: diff

    // A few manual implementations

    let object_name (x0 : OBJECT, x1 : OBJECT, path, diffs) =
        match x0.object_type, x1.object_type with
        | OBJECT_TYPE.TYPE_TABLE, OBJECT_TYPE.TYPE_TABLE ->
            let i = x0.name.LastIndexOf '_'
            equalCollector (x0.name.Substring(0, i), x1.name.Substring(0, i), "name" :: path, diffs)
        | _ -> equalCollector (x0.name, x1.name, "name" :: path, diffs)
    
    let sys_datatype (x0 : SYS_DATATYPE, x1 : SYS_DATATYPE, path, diff) = equalCollector (x0, x1, path, diff)

    let index_name (x0 : INDEX, x1 : INDEX, path, diff) =
        match x0.is_system_named, x0.name, x1.is_system_named, x1.name with
        | true, _, true, _ -> diff
        | false, Some n0, false, Some n1 when n0 = n1 -> diff
        | false, None, false, None -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path :: diff
        

    let foreign_key_name (x0 : FOREIGN_KEY, x1 : FOREIGN_KEY, path, diff) =
        match x0.is_system_named, x0.name, x1.is_system_named, x1.name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path :: diff

    let check_constraint_name (x0 : CHECK_CONSTRAINT, x1 : CHECK_CONSTRAINT, path, diff) =
        match x0.is_system_named, x0.object.name, x1.is_system_named, x1.object.name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path :: diff
       
    let default_constraint_name (x0 : DEFAULT_CONSTRAINT, x1 : DEFAULT_CONSTRAINT, path, diff) =
        match x0.is_system_named, x0.object.name, x1.is_system_named, x1.object.name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path :: diff

    
    let sequence_definition (x0 : SEQUENCE_DEFINITION, x1 : SEQUENCE_DEFINITION, path, diff) =
        diff
        |> fun diff' -> equalCollector (x0.increment, x1.increment, "increment" :: path, diff')
        |> collectOption x0.minimum_value x1.minimum_value equalCollector ("minimum_value" :: path)
        |> collectOption x0.maximum_value x1.maximum_value equalCollector ("maximum_value" :: path)

    let identity_definition (x0 : IDENTITY_DEFINITION, x1 : IDENTITY_DEFINITION, path, diff) =
        equalCollector (x0.increment_value, x1.increment_value, path, diff)
        

        
