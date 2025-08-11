namespace DbFlow.SqlServer

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

type Rule = { Title : string; Description : string; CheckRule : Schema.DATABASE -> Result<unit, string> }


module Helpers =
    let foldAllColumns (exclude : RuleExclusion) (db : Schema.DATABASE) (f : 'a -> Schema.COLUMN -> 'a) (seed : 'a) =
        let excludeObject (o : Schema.OBJECT) =
            Set.contains { schema = o.schema.name; name = o.name } exclude.Objects
        
        let foldColumns f seed (columns : Schema.COLUMN array) =
            columns
            |> Array.fold 
                (fun acc c -> 
                    if Set.contains { schema = c.object.schema.name; parent = c.object.name; name = c.column_name } exclude.Columns
                    then acc
                    else f acc c)
                seed

        let acc = 
            db.TABLES |> List.fold (fun acc' t -> if excludeObject t.object then acc' else t.columns |> foldColumns f acc') seed
        let acc = db.VIEWS |> List.fold (fun acc' v -> if excludeObject v.object then acc' else v.columns |> foldColumns f acc') acc
        let acc = db.TABLE_TYPES |> List.fold (fun acc' tt -> if excludeObject tt.object then acc' else tt.columns |> foldColumns f acc') acc
        let acc = db.PROCEDURES |> List.fold (fun acc' p -> if excludeObject p.object then acc' else p.columns |> foldColumns f acc') acc
        acc

module Rule =
    let ``DATETIME2 - not DATETIME`` exclude = 
        { 
            Title = "DATETIME2 - not DATETIME"
            Description = "DATETIME2 should be prefered over DATETIME."
            CheckRule = 
                (fun db ->
                    Helpers.foldAllColumns exclude db
                        (fun acc c -> 
                            match c.data_type.sys_datatype with
                            | Some Schema.SYS_DATATYPE.DATETIME ->
                                $"{c.object.schema.name}.{c.object.name}.{c.column_name}" :: acc
                            | _ -> acc)
                        []
                    |> function 
                        | [] -> Ok () 
                        | errors -> 
                            let errStr = System.String.Join(", ", errors |> List.toArray) 
                            $"{errStr}" |> Error)
        }

    let ``Postfix datetime with 'Utc'`` exclude = 
        { 
            Title = "Postfix datetime with 'Utc'"
            Description = "Postfix datetime columns with 'Utc' removes the ambiguity of what is actually stored."
            CheckRule = 
                (fun db ->
                    Helpers.foldAllColumns exclude db
                        (fun acc c -> 
                            match c.data_type.sys_datatype with
                            | Some Schema.SYS_DATATYPE.DATETIME when not (c.column_name.ToUpperInvariant().EndsWith "UTC") ->
                                $"{c.object.schema.name}.{c.object.name}.{c.column_name}" :: acc
                            | Some Schema.SYS_DATATYPE.DATETIME2 when not (c.column_name.ToUpperInvariant().EndsWith "UTC") -> 
                                $"{c.object.schema.name}.{c.object.name}.{c.column_name}" :: acc
                            | _ -> acc)
                        []
                    |> function 
                        | [] -> Ok () 
                        | errors -> 
                            let errStr = System.String.Join(", ", errors |> List.toArray) 
                            $"{errStr}" |> Error)
        }

    let ALL (exclusions : RuleExclusion) =
        let exclusions' = { exclusions with Objects = Set.add { schema = "dbo"; name = "SchemaVersions" } exclusions.Objects }
        [
            ``DATETIME2 - not DATETIME`` exclusions'
            ``Postfix datetime with 'Utc'`` exclusions'
        ]
