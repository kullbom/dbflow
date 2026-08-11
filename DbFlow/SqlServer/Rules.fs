namespace DbFlow.SqlServer

open DbFlow

type ColumnKey = { schema : string; parent : string ; name : string }
type ObjectKey = { schema : string; name : string }

type RuleExclusion = { Columns : ColumnKey Set; Objects : ObjectKey Set }

module RuleExclusion =
    let none = { Columns = Set.empty; Objects = Set.empty }
    let create (ex : string list) =
        ex
        |> List.fold 
            (fun (columns, objects) (s : string) ->
                match s.Split(".") with
                | [| schemaName; objectName |] ->
                    columns, Set.add { schema = schemaName; name = objectName } objects
                | [| schemaName; objectName; columnName |] ->
                    Set.add { schema = schemaName; parent = objectName; name = columnName } columns, objects
                | _ -> failwithf "Illegal exclude: %s" s)
            (Set.empty, Set.empty)
        |> fun (columns, objects) ->
            { Columns = columns; Objects = objects }

type Rule = { Title : string; Description : string; Reference : string option; CheckRule : Schema.DatabaseSchema -> Result<unit, string> }


module Helpers =
    let foldAllColumns (exclude : RuleExclusion) (db : Schema.DatabaseSchema) (f : 'a -> Schema.Column -> 'a) (seed : 'a) =
        let excludeObject (o : Schema.OBJECT) =
            Set.contains { schema = o.Schema.Name; name = o.Name } exclude.Objects
        
        let foldColumns f seed (columns : Schema.Column array) =
            columns
            |> Array.fold 
                (fun acc c -> 
                    if Set.contains { schema = c.Object.Schema.Name; parent = c.Object.Name; name = c.Name } exclude.Columns
                    then acc
                    else f acc c)
                seed

        let acc = 
            db.Tables |> List.fold (fun acc' t -> if excludeObject t.Object then acc' else t.Columns |> foldColumns f acc') seed
        let acc = db.Views |> List.fold (fun acc' v -> if excludeObject v.Object then acc' else v.Columns |> foldColumns f acc') acc
        let acc = db.TableTypes |> Map.fold (fun acc' _ tt -> if excludeObject tt.Object then acc' else tt.Columns |> foldColumns f acc') acc
        let acc = db.Procedures |> List.fold (fun acc' p -> if excludeObject p.Object then acc' else p.Columns |> foldColumns f acc') acc
        acc

module Rule =
    let ``DATETIME2 - not DATETIME`` exclude = 
        { 
            Title = "`DATETIME2` - not `DATETIME`"
            Description = "`DATETIME2` should be prefered over `DATETIME`."
            Reference = None
            CheckRule = 
                (fun db ->
                    Helpers.foldAllColumns exclude db
                        (fun acc c -> 
                            match c.Datatype.DatatypeSpec with
                            | Schema.SystemType Schema.SystemDatatype.DATETIME ->
                                $"{c.Object.Schema.Name}.{c.Object.Name}.{c.Name}" :: acc
                            | _ -> acc)
                        []
                    |> function 
                        | [] -> Ok () 
                        | errors -> 
                            errors |> List.toArray |> Array.joinBy ", " id
                            |> Error)
        }

    let ``Postfix datetime with 'Utc'`` exclude = 
        { 
            Title = "Postfix datetime with 'Utc'"
            Description = "Postfix datetime columns with 'Utc' removes the ambiguity of what is actually stored."
            Reference = None
            CheckRule = 
                (fun db ->
                    Helpers.foldAllColumns exclude db
                        (fun acc c -> 
                            match c.Datatype.DatatypeSpec with
                            | Schema.SystemType Schema.SystemDatatype.DATETIME when not (c.Name.ToUpperInvariant().EndsWith "UTC") ->
                                $"{c.Object.Schema.Name}.{c.Object.Name}.{c.Name}" :: acc
                            | Schema.SystemType Schema.SystemDatatype.DATETIME2 when not (c.Name.ToUpperInvariant().EndsWith "UTC") -> 
                                $"{c.Object.Schema.Name}.{c.Object.Name}.{c.Name}" :: acc
                            | _ -> acc)
                        []
                    |> function 
                        | [] -> Ok () 
                        | errors -> 
                            errors |> List.toArray |> Array.joinBy ", " id
                            |> Error)
        }

    let ``No ansi padding OFF - THIS IS PROBABLY NOT HOW TO DO IT...`` exclude = 
        { 
            Title = "No ansi padding OFF"
            Description = "SET ANSI_PADDING OFF, and the ANSI_PADDING OFF database option, are deprecated."
            Reference = Some "https://learn.microsoft.com/en-us/sql/t-sql/statements/set-ansi-padding-transact-sql?view=sql-server-ver17"
            CheckRule = 
                (fun db ->
                    Helpers.foldAllColumns exclude db
                        (fun acc c -> 
                            if c.IsAnsiPadded 
                            then acc
                            else $"{c.Object.Schema.Name}.{c.Object.Name}.{c.Name}" :: acc)
                        []
                    |> function 
                        | [] -> Ok () 
                        | errors -> 
                            errors |> List.toArray |> Array.joinBy ", " id
                            |> Error)
        }

    let ``No system named objects`` (exclude : RuleExclusion) =
        { 
            Title = "No system named objects"
            Description = "Using system named objects makes it hard to adjust or remove them later on."
            Reference = None
            CheckRule = 
                (fun db ->
                    let addObject (o : Schema.OBJECT) acc =
                        let oKey = { schema = o.Schema.Name; name = o.Name.[0..o.Name.LastIndexOf "_"] }
                        if Set.contains oKey exclude.Objects
                        then acc
                        else Schema.OBJECT.fullName o :: acc
                    let addIndex (i : Schema.Index) acc =
                        match i.Object with
                        | Some o -> addObject o acc
                        | None -> 
                            match i.Name with
                            | Some n -> n :: acc
                            | None -> $"Index {i.IndexId} of {Schema.OBJECT.fullName i.Parent}" :: acc
                        
                    db.Tables 
                    |> List.fold 
                        (fun acc0 t -> 
                            let acc1 =
                                t.DefaultConstraints |> Array.fold (fun acc' dc -> if dc.IsSystemNamed then addObject dc.Object acc' else acc') acc0
                            let acc2 =
                                t.CheckConstraints |> Array.fold (fun acc' cc -> if cc.IsSystemNamed then addObject cc.Object acc' else acc') acc1
                            let acc3 =
                                t.ForeignKeys |> Array.fold (fun acc' fk -> if fk.IsSystemNamed then addObject fk.Object acc' else acc') acc2
                            t.Indexes 
                            |> Array.fold (fun acc' i -> if i.IsSystemNamed then addIndex i acc' else acc') acc3)
                        []
                    |> function
                        | [] -> Ok ()
                        | systemNamedObjects -> 
                            systemNamedObjects |> List.toArray |> Array.joinBy ", " id
                            |> Error)
            }

    let ALL (exclusions : RuleExclusion) =
        let exclusions' = { exclusions with Objects = Set.add { schema = "dbo"; name = "SchemaVersions" } exclusions.Objects }
        [
            ``DATETIME2 - not DATETIME`` exclusions'
            ``Postfix datetime with 'Utc'`` exclusions'
            //``No ansi padding OFF`` exclusions'
            ``No system named objects`` exclusions'
        ]
