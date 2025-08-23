namespace DbFlow.SqlServer.Schema

open DbFlow

// In order to compare sequences we need a deterministic order of the elements
type SortOrder = SortOrder
    with 
        static member orderBy (x : OBJECT) = SortOrder.orderBy x.Schema, x.Name, x.ObjectType
        static member orderBy (x : Schema) = x.Name 
        static member orderBy (x : Table) = SortOrder.orderBy x.Object 
        static member orderBy (x : View) = SortOrder.orderBy x.Object 
        static member orderBy (x : Index) = match x.Name with Some n -> n | None -> x.Columns |> Array.joinBy "," (fun c -> c.Column.Name)
        static member orderBy (x : Column) = SortOrder.orderBy x.Object, x.Name
        static member orderBy (x : ForeignKey) =
            if x.IsSystemNamed
            then
                let parentColumns = x.Columns |> Array.joinBy "," (fun c -> c.ParentColumn.Name)
                let refsColumns = x.Columns |> Array.joinBy "," (fun c -> c.ReferencedColumn.Name)
                $"[{x.Parent.Schema.Name}].[{x.Parent.Name}]({parentColumns})>[{x.Referenced.Schema.Name}].[{x.Referenced.Name}]({refsColumns})"
            else x.Name
        static member orderBy (x : Trigger) = SortOrder.orderBy x.Object, x.Name 
        static member orderBy (x : Parameter) = SortOrder.orderBy x.Object, x.Name, x.ParameterId 
        static member orderBy (x : DefaultConstraint) = x.Column.Name
        static member orderBy (x : Synonym) = SortOrder.orderBy x.Object
        static member orderBy (x : Datatype) = x.Name
        static member orderBy (x : TableType) = x.Name
        static member orderBy (x : Procedure) = x.Name 
        static member orderBy (x : XmlSchemaCollection) = SortOrder.orderBy x.Schema, x.Name 
        static member orderBy (x : DatabaseTrigger) = x.Name
        static member orderBy (x : CheckConstraint) = match x.Column with Some c -> c.Name | None -> x.Definition
        static member orderBy (x : ForeignKeycolumn) = SortOrder.orderBy x.ParentColumn
        static member orderBy (x : IndexColumn) = SortOrder.orderBy x.Column
        static member orderBy (x : Sequence) = SortOrder.orderBy x.Object

type ElementId = ElementId
    with 
        static member elementId (x : Table) = x.Name
        static member elementId (x : Schema) = x.Name 
        static member elementId (x : View) = x.Name
        static member elementId (x : Index) = match x.Name with Some n -> n | None -> "???"
        static member elementId (x : Column) = x.Name
        static member elementId (x : ForeignKey) = x.Name
        static member elementId (x : Trigger) = x.Name
        static member elementId (x : Parameter) = x.Name
        static member elementId (x : DefaultConstraint) = "DF???" + x.Column.Name
        static member elementId (x : Synonym) = x.Object.Name
        static member elementId (x : Datatype) = x.Name
        static member elementId (x : TableType) = x.Name
        static member elementId (x : Procedure) = x.Name 
        static member elementId (x : XmlSchemaCollection) = x.Name 
        static member elementId (x : DatabaseTrigger) = x.Name
        static member elementId (x : CheckConstraint) = x.Object.Name
        static member elementId (x : ForeignKeycolumn) = $"{x.ParentColumn.Name} -> {x.ReferencedColumn.Name}"
        static member elementId (x : IndexColumn) = x.Column.Name
        static member elementId (x : OBJECT) = x.Name
        static member elementId (x : Sequence) = x.Object.Name

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

    let objectName (x0 : OBJECT, x1 : OBJECT) path diffs =
        match x0.ObjectType, x1.ObjectType with
        | ObjectType.ForeignKeyConstraint, ObjectType.ForeignKeyConstraint 
        | ObjectType.TypeTable, ObjectType.TypeTable ->
            // Ignore these since they can be system named 
            diffs
        | _ -> equalCollector (x0.Name, x1.Name) ("name" :: path) diffs
    
    //let sys_datatype (x0 : SYS_DATATYPE, x1 : SYS_DATATYPE, path, diff) = equalCollector (x0, x1) path diff

    let indexName (x0 : Index, x1 : Index) path diff =
        match x0.IsSystemNamed, x0.Name, x1.IsSystemNamed, x1.Name with
        | true, _, true, _ -> diff
        | false, Some n0, false, Some n1 when n0 = n1 -> diff
        | false, None, false, None -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path x0 x1 :: diff
        

    let foreignKeyName (x0 : ForeignKey, x1 : ForeignKey) path diff =
        match x0.IsSystemNamed, x0.Name, x1.IsSystemNamed, x1.Name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path x0 x1 :: diff

    let checkConstraintName (x0 : CheckConstraint, x1 : CheckConstraint) path diff =
        match x0.IsSystemNamed, x0.Object.Name, x1.IsSystemNamed, x1.Object.Name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path n0 n1 :: diff
       
    let defaultConstraintName (x0 : DefaultConstraint, x1 : DefaultConstraint) path diff =
        match x0.IsSystemNamed, x0.Object.Name, x1.IsSystemNamed, x1.Object.Name with
        | true, _, true, _ -> diff
        | false, n0, false, n1 when n0 = n1 -> diff
        | _,n0,_,n1 -> Diff.create $"names does not match ({n0} != {n1})" path x0 x1 :: diff

    
    let sequenceDefinition (x0 : SequenceDefinition, x1 : SequenceDefinition) path diff =
        diff
        |> equalCollector (x0.Increment, x1.Increment) ("increment" :: path)
        |> collectOption x0.MinimumValue x1.MinimumValue equalCollector ("minimum_value" :: path)
        |> collectOption x0.MaximumValue x1.MaximumValue equalCollector ("maximum_value" :: path)

    let identityDefinition (x0 : IdentityDefinition, x1 : IdentityDefinition) path diff =
        equalCollector (x0.IncrementValue, x1.IncrementValue) path diff
        

        
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
                sDefNoneT<CheckConstraint> "ParentColumnId"
                sDefNoneT<ForeignKey> "KeyIndexId"
                sDefNoneT<Column> "ColumnId"
                sDefNoneT<Index> "IndexId"
                sDefNoneT<IndexColumn> "IndexId"
                sDefNoneT<Datatype> "UserTypeId"
                
                sDefT<ForeignKey> "DeleteReferentialAction" equalCollector
                sDefT<ForeignKey> "UpdateReferentialAction" equalCollector

                sDefT<OBJECT> "Name" (fun _ -> $"|> Compare.objectName (x0, x1) path") 
                sDefT<OBJECT> "ObjectType" equalCollector
                
                sDefT<Index> "Name" (fun _ -> $"|> Compare.indexName (x0, x1) path")
                sDefNoneT<Index> "Object"
                sDefNoneT<Index> "IsSystemNamed"
                sDefT<Index> "IndexType" equalCollector
                
                sDefT<ForeignKey> "Name" (fun _ -> $"|> Compare.foreignKeyName (x0, x1) path")
                sDefNoneT<ForeignKey> "Object"
                sDefNoneT<ForeignKey> "IsSystemNamed"

                sDefT<CheckConstraint> "Object" (fun _ -> $"|> Compare.checkConstraintName (x0, x1) path")
                sDefNoneT<CheckConstraint> "IsSystemNamed"

                sDefT<DefaultConstraint> "Object" (fun _ -> $"|> Compare.defaultConstraintName (x0, x1) path")
                sDefNoneT<DefaultConstraint> "IsSystemNamed"

                sDefT<Sequence> "SequenceDefinition" (fun pname -> $"|> Compare.sequenceDefinition (x0.{pname}, x1.{pname}) (\"{pname}\" :: path)")
   
                sDefT<Column> "IdentityDefinition" (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.identityDefinition (\"{pname}\" :: path)")
                sDefT<Datatype> "SystemDatatype" (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.equalCollector (\"{pname}\" :: path)")
                
            ] |> Map.ofList

        let sDefNone pname = pname, fun (_ : string) -> ""
        let sDef pname def = pname, def 
        
        let specialDefinitions = 
            [
                sDefNone "CreateDate"
                sDefNone "ModifyDate"
                sDefNone "OrigDefinition"

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
                "ReferentialAction"; "ObjectType"; "SystemDatatype"; "IndexType"; 
                "IdentityDefinition"; "SequenceDefinition"
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
                                | PropType.List -> $"|> Compare.collectList x0.{pname} x1.{pname} SortOrder.orderBy ElementId.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Array -> $"|> Compare.collectArray x0.{pname} x1.{pname} SortOrder.orderBy ElementId.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Special special -> special
                                |> function "" -> None | s -> Some s 
                            )
                        |> Array.iter
                            (fun s -> append $"                       {s}"))
            (Set.empty, ())
        |> ignore

        sb.ToString()
