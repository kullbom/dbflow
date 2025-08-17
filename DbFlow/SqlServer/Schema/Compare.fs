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
        | OBJECT_TYPE.FOREIGN_KEY_CONSTRAINT, OBJECT_TYPE.FOREIGN_KEY_CONSTRAINT 
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
        

        
// Generate diff collectors (to avoid missing a properety on changes


[<RequireQualifiedAccess>]
type PropType =
    Gen
    | List
    | Array
    | Option
    | Special of (string -> string)

module Generator =
    let listType = typeof<list<_>>.GetGenericTypeDefinition()
    let optionType = typeof<option<_>>.GetGenericTypeDefinition()
    let mapType = typeof<Map<_,_>>.GetGenericTypeDefinition()
    
    let propertyType (parent : System.Type) (pName : string) (pType :System.Type) =
        match pName with
        | "dependencies" when parent = typeof<DATABASE> -> PropType.Special (fun pname -> "")
        | "schema_id" when parent = typeof<SCHEMA> -> PropType.Special (fun pname -> "")
        | "object_id" when parent = typeof<OBJECT> -> PropType.Special (fun pname -> "")
        | "parent_object_id" when parent = typeof<OBJECT> -> PropType.Special (fun pname -> "")
        | "column_id" when parent = typeof<COLUMN> -> PropType.Special (fun pname -> "")
        | "create_date" -> PropType.Special (fun pname -> "")
        | "modify_date" -> PropType.Special (fun pname -> "")
        | "all_objects" -> PropType.Special (fun pname -> "")
        | "orig_definition" -> PropType.Special (fun pname -> "")
        
        // TODO: This should be part of the generated scripts
        | "ms_description" -> PropType.Special (fun pname -> "")
    
        | "name" when parent = typeof<OBJECT> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.object_name (x0, x1, path, diff')") 
        
        | "name" when parent = typeof<INDEX> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.index_name (x0, x1, path, diff')") 
        | "object" when parent = typeof<INDEX> -> PropType.Special (fun _ -> "")
        | "is_system_named" when parent = typeof<INDEX> -> PropType.Special (fun _ -> "")
        
        | "name" when parent = typeof<FOREIGN_KEY> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.foreign_key_name (x0, x1, path, diff')") 
        | "object" when parent = typeof<FOREIGN_KEY> -> PropType.Special (fun _ -> "")
        | "is_system_named" when parent = typeof<FOREIGN_KEY> -> PropType.Special (fun _ -> "")
    
        | "name" when parent = typeof<CHECK_CONSTRAINT> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.check_constraint_name (x0, x1, path, diff')") 
        | "object" when parent = typeof<CHECK_CONSTRAINT> -> PropType.Special (fun _ -> "")
        | "is_system_named" when parent = typeof<CHECK_CONSTRAINT> -> PropType.Special (fun _ -> "")
    
        | "name" when parent = typeof<DEFAULT_CONSTRAINT> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.default_constraint_name (x0, x1, path, diff')") 
        | "object" when parent = typeof<DEFAULT_CONSTRAINT> -> PropType.Special (fun _ -> "")
        | "is_system_named" when parent = typeof<DEFAULT_CONSTRAINT> -> PropType.Special (fun _ -> "")
    
        | "sequence_definition" when parent = typeof<SEQUENCE> -> PropType.Special (fun pname -> $"|> fun diff' -> Compare.sequence_definition (x0.{pname}, x1.{pname}, \"{pname}\" :: path, diff')")
       
        | "identity_definition" when parent = typeof<COLUMN> -> PropType.Special (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.identity_definition (\"{pname}\" :: path)")
        | "sys_datatype"      when parent = typeof<DATATYPE> -> PropType.Special (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.equalCollector (\"{pname}\" :: path)")
        
        | "object_type" when pType = typeof<OBJECT_TYPE> -> PropType.Special (fun pname -> $"|> fun diff' -> Compare.equalCollector (x0.{pname}, x1.{pname}, (\"{pname}\" :: path), diff')")
        | "index_type" when pType = typeof<INDEX_TYPE> -> PropType.Special (fun pname -> $"|> fun diff' -> Compare.equalCollector (x0.{pname}, x1.{pname}, (\"{pname}\" :: path), diff')")
        | _ ->
            if pType.IsArray
            then PropType.Array
            else if pType.IsGenericType
            then 
                match pType.GetGenericTypeDefinition() with
                | t when t = listType -> PropType.List
                | t when t = optionType -> PropType.Option
                | t -> failwithf "Unhandled type %A" t
            else PropType.Gen
    
    let rec foldTypes (t : System.Type) f (visited, seed) =
        if Set.contains t.FullName visited
        then
            visited, seed
        else
            let visited' = Set.add t.FullName visited
            let name = t.Name 
            if t.Namespace = "DbFlow.SqlServer.Schema"
            then
                if t.IsArray
                then foldTypes (t.GetElementType ()) f (visited', seed)
                else 
                    let properties' = t.GetProperties ()
                    let properties =
                        properties'
                        |> Array.map (fun p -> propertyType t p.Name p.PropertyType, p.Name)
                    
                    let seed' = f seed name (Some properties)
                    
                    properties'
                    |> Array.fold (fun (visited'', acc) p ->
                        let pt = p.PropertyType 
                        if pt.IsGenericType
                        then 
                            match pt.GetGenericTypeDefinition(), pt.GenericTypeArguments with
                            | t, [| t' |] when t = listType -> foldTypes t' f (visited'', acc)
                            | t, [| t' |] when t = optionType -> foldTypes t' f (visited'', acc)
                            | t, [| kt; vt |] when t = mapType -> foldTypes kt f (visited'', acc) |> foldTypes vt f 
                            | t', _ -> failwithf "Unhandled type %A" t'
                        else foldTypes pt f (visited'', acc))
                        (visited', seed')
            else visited', f seed name None

    let generate () =
        let sb = System.Text.StringBuilder ()
        let append (s : string) = sb.AppendLine s |> ignore
        append """namespace DbFlow.SqlServer.Schema

open DbFlow

type CompareGen = CompareGenCase
    with"""

        
        foldTypes typeof<DATABASE>
            (fun () (ty : string) fields -> 
                if ty.StartsWith "<>" || ty.StartsWith "FSharpList" || ty = "Object" 
                    || ty = "OBJECT_TYPE" || ty = "SYS_DATATYPE" || ty = "INDEX_TYPE" || ty = "IDENTITY_DEFINITION" || ty = "SEQUENCE_DEFINITION"
                then ()
                else
                    append ""
                    match fields with
                    | None -> 
                        append $"        static member Collect (x0 : System.{ty}, x1 : System.{ty}, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)"
                    | Some fs ->
                        append $"        static member Collect (x0 : {ty}, x1 : {ty}, path, diffs) ="
                        append $"                    diffs"
                        fs
                        |> Array.choose 
                            (fun (pt, pname) -> 
                                match pt with
                                | PropType.Gen -> $"|> fun diffs' -> CompareGen.Collect (x0.{pname}, x1.{pname}, \"{pname}\" :: path, diffs')"
                                | PropType.Option -> $"|> Compare.collectOption x0.{pname} x1.{pname} CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.List -> $"|> Compare.collectList x0.{pname} x1.{pname} SortOrder.orderBy Sequence.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Array -> $"|> Compare.collectArray x0.{pname} x1.{pname} SortOrder.orderBy Sequence.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Special special -> special pname
                                |> function "" -> None | s -> Some s 
                            )
                        |> Array.iter
                            (fun s -> append $"                    {s}"))
            (Set.empty, ())
        |> ignore

        sb.ToString()
