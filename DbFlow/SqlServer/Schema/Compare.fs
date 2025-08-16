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


module Compare =
    let optionIsSame (o0 : 'a option) (o1 : 'a option) (isSame : ('a * 'a) -> bool) =
        match o0, o1 with
        | None, None -> true
        | Some x, Some y -> isSame (x,y)
        | _ -> false
    
    let listIsSame (l0 : 'a list) (l1 : 'a list) (orderBy : 'a -> 'k) (isSame : ('a * 'a) -> bool) =
        List.forall2 (fun x y -> isSame (x,y)) (l0 |> List.sortBy orderBy) (l1 |> List.sortBy orderBy)
    
    let arrayIsSame (l0 : 'a array) (l1 : 'a array) (orderBy : 'a -> 'k) (isSame : ('a * 'a) -> bool) =
        Array.forall2 (fun x y -> isSame (x,y)) (l0 |> Array.sortBy orderBy) (l1 |> Array.sortBy orderBy)

    
    // A few manual implementations
    let object_type (x0 : OBJECT_TYPE) (x1 : OBJECT_TYPE) = x0 = x1
    
    let index_type (x0 : INDEX_TYPE) (x1 : INDEX_TYPE) = x0 = x1
    
    let sys_datatype (x0 : SYS_DATATYPE, x1 : SYS_DATATYPE) = x0 = x1

    let index_name (x0 : INDEX) (x1 : INDEX) =
        match x0.is_system_named, x0.name, x1.is_system_named, x1.name with
        | true, _, true, _ -> true
        | false, Some n0, false, Some n1 when n0 = n1 -> true
        | _ -> false

    let foreign_key_name (x0 : FOREIGN_KEY) (x1 : FOREIGN_KEY) =
        match x0.is_system_named, x0.name, x1.is_system_named, x1.name with
        | true, _, true, _ -> true
        | false, n0, false, n1 when n0 = n1 -> true
        | _ -> false

    let check_constraint_name (x0 : CHECK_CONSTRAINT) (x1 : CHECK_CONSTRAINT) =
        match x0.is_system_named, x0.object.name, x1.is_system_named, x1.object.name with
        | true, _, true, _ -> true
        | false, n0, false, n1 when n0 = n1 -> true
        | _ -> false
       
    let default_constraint_name (x0 : DEFAULT_CONSTRAINT) (x1 : DEFAULT_CONSTRAINT) =
        match x0.is_system_named, x0.object.name, x1.is_system_named, x1.object.name with
        | true, _, true, _ -> true
        | false, n0, false, n1 when n0 = n1 -> true
        | _ -> false    

    
    let sequence_definition (x0 : SEQUENCE_DEFINITION) (x1 : SEQUENCE_DEFINITION) =
        x0.increment = x1.increment
        && optionIsSame x0.minimum_value x1.minimum_value (fun (v0,v1) -> v0 = v1)
        && optionIsSame x0.maximum_value x1.maximum_value (fun (v0,v1) -> v0 = v1)
    
    let identity_definition (x0 : IDENTITY_DEFINITION, x1 : IDENTITY_DEFINITION) =
        x0.increment_value = x1.increment_value
    
    let schema_id (_x0 : _) (_x1 : _) = true
    let object_id (_x0 : _) (_x1 : _) = true
    let create_date (_x0 : _) (_x1 : _) = true
    let modify_date (_x0 : _) (_x1 : _) = true
    
    let dependencies (_x0 : _) (_x1 : _) = true


            
