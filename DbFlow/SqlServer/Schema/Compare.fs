namespace DbFlow.SqlServer.Schema

open DbFlow

// In order to compare sequences we need a deterministic order of the elements
type SortOrder = SortOrder
    with 
        static member orderBy (x : OBJECT) = SortOrder.orderBy x.Schema, x.Name, x.ObjectType
        static member orderBy (x : Schema) = x.Name 
        static member orderBy (x : TABLE) = SortOrder.orderBy x.Object 
        static member orderBy (x : VIEW) = SortOrder.orderBy x.Object 
        static member orderBy (x : INDEX) = match x.name with Some n -> n | None -> x.columns |> Array.joinBy "," (fun c -> c.column.column_name)
        static member orderBy (x : COLUMN) = SortOrder.orderBy x.object, x.column_name
        static member orderBy (x : FOREIGN_KEY) =
            if x.IsSystemNamed
            then
                let parentColumns = x.columns |> Array.joinBy "," (fun c -> c.parent_column.column_name)
                let refsColumns = x.columns |> Array.joinBy "," (fun c -> c.referenced_column.column_name)
                $"[{x.parent.Schema.Name}].[{x.parent.Name}]({parentColumns})>[{x.referenced.Schema.Name}].[{x.referenced.Name}]({refsColumns})"
            else x.name
        static member orderBy (x : TRIGGER) = SortOrder.orderBy x.object, x.trigger_name 
        static member orderBy (x : PARAMETER) = SortOrder.orderBy x.object, x.name, x.parameter_id 
        static member orderBy (x : DEFAULT_CONSTRAINT) = x.Column.column_name
        static member orderBy (x : SYNONYM) = SortOrder.orderBy x.object
        static member orderBy (x : Datatype) = x.name
        static member orderBy (x : TableType) = x.Name
        static member orderBy (x : PROCEDURE) = x.name 
        static member orderBy (x : XmlSchemaCollection) = SortOrder.orderBy x.Schema, x.Name 
        static member orderBy (x : DATABASE_TRIGGER) = x.trigger_name
        static member orderBy (x : CHECK_CONSTRAINT) = match x.column with Some c -> c.column_name | None -> x.definition
        static member orderBy (x : FOREIGN_KEY_COLUMN) = SortOrder.orderBy x.parent_column
        static member orderBy (x : INDEX_COLUMN) = SortOrder.orderBy x.column
        static member orderBy (x : SEQUENCE) = SortOrder.orderBy x.object

type Sequence = Sequence
    with 
        static member elementId (x : TABLE) = x.Name
        static member elementId (x : Schema) = x.Name 
        static member elementId (x : VIEW) = x.Name
        static member elementId (x : INDEX) = match x.name with Some n -> n | None -> "???"
        static member elementId (x : COLUMN) = x.column_name
        static member elementId (x : FOREIGN_KEY) = x.name
        static member elementId (x : TRIGGER) = x.trigger_name
        static member elementId (x : PARAMETER) = x.name
        static member elementId (x : DEFAULT_CONSTRAINT) = "DF???" + x.Column.column_name
        static member elementId (x : SYNONYM) = x.object.Name
        static member elementId (x : Datatype) = x.name
        static member elementId (x : TableType) = x.Name
        static member elementId (x : PROCEDURE) = x.name 
        static member elementId (x : XmlSchemaCollection) = x.Name 
        static member elementId (x : DATABASE_TRIGGER) = x.trigger_name
        static member elementId (x : CHECK_CONSTRAINT) = x.object.Name
        static member elementId (x : FOREIGN_KEY_COLUMN) = $"{x.parent_column.column_name} -> {x.referenced_column.column_name}"
        static member elementId (x : INDEX_COLUMN) = x.column.column_name
        static member elementId (x : OBJECT) = x.Name
        static member elementId (x : SEQUENCE) = x.object.Name

type Diff = { Message : string; Path : string; Data0 : obj; Data1 : obj }
    with static member create message path data0 data1 = 
            { 
                Message = message; 
                Path = path |> List.rev |> List.toArray |> Array.joinBy "/" id 
                Data0 = data0
                Data1 = data1
            }

module Compare =
    let collectOption (o0 : 'a option) (o1 : 'a option) (collector : 'a * 'a -> _ -> _ -> _) path diffs =
        match o0, o1 with
        | None, None -> diffs
        | Some x, Some y -> collector (x, y) path diffs
        | x, y -> Diff.create $"{x} != {y}" path x y :: diffs
    
    let collectList (l0 : 'a list) (l1 : 'a list) (orderBy : 'a -> 'k) (elementId : 'a -> string) (collector : 'a * 'a -> _ -> _ -> _) path diffs =
        if l0.Length <> l1.Length
        then Diff.create $"different length ({l0.Length} != {l1.Length})" path l0 l1 :: diffs
        else
            List.fold2 
                (fun diffs' x y -> collector (x, y) (elementId x :: path) diffs')
                diffs
                (l0 |> List.sortBy orderBy) 
                (l1 |> List.sortBy orderBy)
    
    let collectArray (l0 : 'a array) (l1 : 'a array) (orderBy : 'a -> 'k) (elementId : 'a -> string) (collector : 'a * 'a -> _ -> _ -> _) path diffs =
        if l0.Length <> l1.Length
        then Diff.create $"different length ({l0.Length} != {l1.Length})" path l0 l1 :: diffs
        else
            Array.fold2 
                (fun diffs' x y -> collector (x, y) (elementId x :: path) diffs')
                diffs
                (l0 |> Array.sortBy orderBy) 
                (l1 |> Array.sortBy orderBy)

    let equalCollector (x0 : _, x1 : _) path diff = 
        if x0 = x1 then diff else Diff.create $"{x0} != {x1}" path x0 x1 :: diff

    // A few manual implementations

    let object_name (x0 : OBJECT, x1 : OBJECT) path diffs =
        match x0.ObjectType, x1.ObjectType with
        | ObjectType.FOREIGN_KEY_CONSTRAINT, ObjectType.FOREIGN_KEY_CONSTRAINT 
        | ObjectType.TYPE_TABLE, ObjectType.TYPE_TABLE ->
            // Ignore these since they can be system named 
            diffs
        | _ -> equalCollector (x0.Name, x1.Name) ("name" :: path) diffs
    
    //let sys_datatype (x0 : SYS_DATATYPE, x1 : SYS_DATATYPE, path, diff) = equalCollector (x0, x1) path diff

    let index_name (x0 : INDEX, x1 : INDEX) path diff =
        match x0.IsSystemNamed, x0.name, x1.IsSystemNamed, x1.name with
        | true, _, true, _ -> diff
        | false, Some n0, false, Some n1 when n0 = n1 -> diff
        | false, None, false, None -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path x0 x1 :: diff
        

    let foreign_key_name (x0 : FOREIGN_KEY, x1 : FOREIGN_KEY) path diff =
        match x0.IsSystemNamed, x0.name, x1.IsSystemNamed, x1.name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path x0 x1 :: diff

    let check_constraint_name (x0 : CHECK_CONSTRAINT, x1 : CHECK_CONSTRAINT) path diff =
        match x0.IsSystemNamed, x0.object.Name, x1.IsSystemNamed, x1.object.Name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path n0 n1 :: diff
       
    let default_constraint_name (x0 : DEFAULT_CONSTRAINT, x1 : DEFAULT_CONSTRAINT) path diff =
        match x0.IsSystemNamed, x0.Object.Name, x1.IsSystemNamed, x1.Object.Name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path x0 x1 :: diff

    
    let sequence_definition (x0 : SEQUENCE_DEFINITION, x1 : SEQUENCE_DEFINITION) path diff =
        diff
        |> equalCollector (x0.increment, x1.increment) ("increment" :: path)
        |> collectOption x0.minimum_value x1.minimum_value equalCollector ("minimum_value" :: path)
        |> collectOption x0.maximum_value x1.maximum_value equalCollector ("maximum_value" :: path)

    let identity_definition (x0 : IDENTITY_DEFINITION, x1 : IDENTITY_DEFINITION) path diff =
        equalCollector (x0.increment_value, x1.increment_value) path diff
        

        
// Generate diff collectors (to avoid missing a properety on changes


[<RequireQualifiedAccess>]
type PropType =
    Gen
    | List
    | Array
    | Option
    | Special of string

module Generator =
    let listType = typeof<list<_>>.GetGenericTypeDefinition()
    let optionType = typeof<option<_>>.GetGenericTypeDefinition()
    let mapType = typeof<Map<_,_>>.GetGenericTypeDefinition()
    
    module Special =
        let sDefNoneT<'a> pname = (pname, typeof<'a>.FullName), fun (_ : string) -> ""
        let sDefT<'a> pname (def : string -> string) = (pname, typeof<'a>.FullName), def
        
        let equalCollector pname = $"|> Compare.equalCollector (x0.{pname}, x1.{pname}) (\"{pname}\" :: path)"

        
        let specialDefinitionsT = 
            [
                // Skip these
                sDefNoneT<DatabaseSchema> "Dependencies"
                sDefNoneT<Schema> "SchemaId"
                sDefNoneT<OBJECT> "ObjectId"
                sDefNoneT<OBJECT> "ParentObjectId"
                sDefNoneT<CHECK_CONSTRAINT> "parent_column_id"
                sDefNoneT<FOREIGN_KEY> "key_index_id"
                sDefNoneT<COLUMN> "column_id"
                sDefNoneT<INDEX> "index_id"
                sDefNoneT<INDEX_COLUMN> "index_id"
                sDefNoneT<Datatype> "user_type_id"
                
                sDefT<FOREIGN_KEY> "delete_referential_action" equalCollector
                sDefT<FOREIGN_KEY> "update_referential_action" equalCollector

                sDefT<OBJECT> "Name" (fun _ -> $"|> Compare.object_name (x0, x1) path") 
                sDefT<OBJECT> "ObjectType" equalCollector
                
                sDefT<INDEX> "name" (fun _ -> $"|> Compare.index_name (x0, x1) path")
                sDefNoneT<INDEX> "object"
                sDefNoneT<INDEX> "IsSystemNamed"
                sDefT<INDEX> "index_type" equalCollector
                
                sDefT<FOREIGN_KEY> "name" (fun _ -> $"|> Compare.foreign_key_name (x0, x1) path")
                sDefNoneT<FOREIGN_KEY> "object"
                sDefNoneT<FOREIGN_KEY> "IsSystemNamed"

                sDefT<CHECK_CONSTRAINT> "object" (fun _ -> $"|> Compare.check_constraint_name (x0, x1) path")
                sDefNoneT<CHECK_CONSTRAINT> "IsSystemNamed"

                sDefT<DEFAULT_CONSTRAINT> "Object" (fun _ -> $"|> Compare.default_constraint_name (x0, x1) path")
                sDefNoneT<DEFAULT_CONSTRAINT> "IsSystemNamed"

                sDefT<SEQUENCE> "sequence_definition" (fun pname -> $"|> Compare.sequence_definition (x0.{pname}, x1.{pname}) (\"{pname}\" :: path)")
   
                sDefT<COLUMN> "identity_definition" (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.identity_definition (\"{pname}\" :: path)")
                sDefT<Datatype> "sys_datatype" (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.equalCollector (\"{pname}\" :: path)")
                
            ] |> Map.ofList

        let sDefNone pname = pname, fun (_ : string) -> ""
        let sDef pname def = pname, def 
        
        let specialDefinitions = 
            [
                sDefNone "CreateDate"
                sDefNone "ModifyDate"
                sDefNone "all_objects"
                sDefNone "origDefinition"
                
                // TODO: This should be part of the generated scripts
                sDefNone "MSDescription"
                sDefNone "ms_description"
            ] |> Map.ofList

        let getSpecialDef pname (parentType : System.Type) (propertyType : System.Type) =
            match Map.tryFind pname specialDefinitions with
            | Some d -> Some (d pname)
            | None ->
                match Map.tryFind (pname, parentType.FullName) specialDefinitionsT with
                | Some d -> Some (d pname)
                | None -> None


    let propertyType (parent : System.Type) (pname : string) (pType :System.Type) =
        match Special.getSpecialDef pname parent pType with
        | Some d -> PropType.Special d 
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

        let skipObjects =
            [ 
                "Object"
                "REFERENTIAL_ACTION"; "ObjectType"; "SysDatatype"; "INDEX_TYPE"; 
                "IDENTITY_DEFINITION"; "SEQUENCE_DEFINITION"
            ]
            |> Set.ofList

        foldTypes typeof<DatabaseSchema>
            (fun () (ty : string) fields -> 
                if ty.StartsWith "<>" || ty.StartsWith "FSharpList" || Set.contains ty skipObjects
                then ()
                else
                    append ""
                    match fields with
                    | None -> 
                        append $"        static member Collect (x0 : System.{ty}, x1 : System.{ty}) = Compare.equalCollector (x0, x1)"
                    | Some fs ->
                        append $"        static member Collect (x0 : {ty}, x1 : {ty}) ="
                        append $"                    fun path diffs ->"
                        append $"                       diffs"
                        fs
                        |> Array.choose 
                            (fun (pt, pname) -> 
                                match pt with
                                | PropType.Gen -> $"|> CompareGen.Collect (x0.{pname}, x1.{pname}) (\"{pname}\" :: path)"
                                | PropType.Option -> $"|> Compare.collectOption x0.{pname} x1.{pname} CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.List -> $"|> Compare.collectList x0.{pname} x1.{pname} SortOrder.orderBy Sequence.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Array -> $"|> Compare.collectArray x0.{pname} x1.{pname} SortOrder.orderBy Sequence.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Special special -> special
                                |> function "" -> None | s -> Some s 
                            )
                        |> Array.iter
                            (fun s -> append $"                       {s}"))
            (Set.empty, ())
        |> ignore

        sb.ToString()
