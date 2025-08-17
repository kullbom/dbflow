module DbFlow.SqlServer.Scripts.Generate

open DbFlow
open DbFlow.Dependencies
open DbFlow.SqlServer.Schema

type Script = { 
    directory_name : string; 
    filename : string; 
    content : string; 
}

type ScriptObjects =
    | SchemaDefinition
    | ObjectDefinitions of {| contains_objects : int list; depends_on : int list |}
    | UserDefinedTypeDefinition
    | XmlSchemaCollectionDefinition


let columnDefinitionStr (opt : Options) allTypes isTableType (columnInlineDefaults : Map<int, DEFAULT_CONSTRAINT>) (column : COLUMN) =
    let columnDefStr =
        match column.computed_definition with
        | Some computed ->
            let persistStr = if computed.is_persisted then " PERSISTED" else ""
            $"AS {computed.computed_definition}{persistStr}"
        | None -> 
            let nullStr = if column.data_type.parameter.is_nullable then "NULL" else "NOT NULL"
            let maskedStr =
                if opt.SchemazenCompatibility
                then ""
                else 
                    match column.masking_function with
                    | Some m -> $" MASKED WITH ( FUNCTION = '{m}' )"
                    | None -> ""
            let identityStr = 
                if opt.SchemazenCompatibility && isTableType
                then ""
                else
                    match column.identity_definition with
                    | Some def -> $"\r\n      IDENTITY ({def.seed_value},{def.increment_value})"
                    | None -> ""
            let checkStr = 
                match Map.tryFind column.column_id columnInlineDefaults with
                | Some dc -> $"\r\n       DEFAULT {dc.definition}"
                | None -> ""
            let rowGuidStr =
                if column.is_rowguidcol
                then " ROWGUIDCOL "
                else ""
            let typeStr = 
                if opt.SchemazenCompatibility 
                    && (column.data_type.is_user_defined || column.data_type.name = "sysname")
                then 
                    DATATYPE.typeStr opt.SchemazenCompatibility false 
                        { (Map.find (int column.data_type.system_type_id) allTypes) with parameter = column.data_type.parameter }
                else DATATYPE.typeStr opt.SchemazenCompatibility false column.data_type
            $"{typeStr}{maskedStr} {nullStr}{identityStr}{checkStr}{rowGuidStr}"
    $"[{column.column_name}] {columnDefStr}"

let separateBy f xs =
    xs 
    |> Array.fold (fun (xs, ys) x -> if f x then x :: xs, ys else xs, x :: ys) ([], [])
    |> fun (xs, ys) -> xs |> List.rev |> List.toArray, ys |> List.rev |> List.toArray
    
let generateSchemaScript (w : System.IO.StreamWriter) (opt : Options) (schema : SCHEMA) =
    if opt.SchemazenCompatibility
    then w.WriteLine $"create schema [{schema.name}] authorization [{schema.principal_name}]"
    else w.WriteLine $"CREATE SCHEMA [{schema.name}] AUTHORIZATION [{schema.principal_name}]"
    w.WriteLine "GO"
    SchemaDefinition

let commaSeparated (w : System.IO.StreamWriter) (indentionStr : string) xs (formatter : _ -> string) =
    let n = xs |> Array.length
    xs 
    |> Array.iteri
        (fun i x -> 
            let commaStr = if i = n - 1 then "" else ","
            w.WriteLine $"{indentionStr}{formatter x}{commaStr}")

let indexColumnsStr (columns : INDEX_COLUMN array) =
    columns
    |> Array.joinBy 
        ", "
        (fun c -> 
            let descStr = if c.is_descending_key then " DESC" else ""
            $"[{c.column.column_name}]{descStr}")

let indexWithSettings (index : INDEX) =
    [|
        match index.fill_factor with 0uy -> "" | ff -> $"FILLFACTOR = {ff}"
    |]
    |> Array.filter (fun s -> s.Length > 0)
    |> Array.joinBy ", " id
    |> function "" -> "" | s -> $"\r\n    WITH( {s} )"

let primaryKeyStr (opt : Options) isTableType (pk : INDEX) =
    let pkName = 
        match pk.name with Some n -> n | None -> failwithf "Can not handle PK without name"
    let clusteredStr = 
        match pk.index_type with
        | INDEX_TYPE.CLUSTERED -> "CLUSTERED"
        | INDEX_TYPE.NONCLUSTERED -> "NONCLUSTERED"
        | iType -> failwithf "Unhandled index type %A of primary key %s" iType pkName
    let pkColumnsStr = indexColumnsStr pk.columns 
    if opt.SchemazenCompatibility
    then 
        if isTableType 
        then $"PRIMARY KEY {clusteredStr} ({pkColumnsStr})"
        else $"CONSTRAINT [{pkName}] PRIMARY KEY {clusteredStr} ({pkColumnsStr})"
    else    
        let withSettings = indexWithSettings pk
        if isTableType || pk.is_system_named
        then $"PRIMARY KEY {clusteredStr} ({pkColumnsStr}){withSettings}"
        else $"CONSTRAINT [{pkName}] PRIMARY KEY {clusteredStr} ({pkColumnsStr}){withSettings}"
        
let uniqueKeyStr (opt : Options) (i : INDEX) =
    let iName = 
        match i.name with Some n -> n | None -> failwithf "Can not handle UNIQUE CONSTRAINTS without name"
    let clusteredStr = 
        match i.index_type with
        | INDEX_TYPE.CLUSTERED -> "CLUSTERED"
        | INDEX_TYPE.NONCLUSTERED -> "NONCLUSTERED"
        | iType -> failwithf "Unhandled index type %A of primary key %s" iType iName
    let indexColumnsStr = indexColumnsStr i.columns
    if (not i.is_system_named) || opt.SchemazenCompatibility
    then $"CONSTRAINT [{iName}] UNIQUE {clusteredStr} ({indexColumnsStr})"
    else $"UNIQUE {clusteredStr} ({indexColumnsStr})"

let generateStandardIndexScript (opt : Options) (index : INDEX) (parentName : string) (indexName : string) (indexTypeStr : string) =
    let (includeColumns, keyColumns) =
        separateBy (fun c -> c.is_included_column) index.columns

    let keyColumnsStr = indexColumnsStr keyColumns
    let includeStr = 
        if Array.isEmpty includeColumns
        then ""
        else 
            includeColumns
            |> Array.joinBy ", " (fun c -> $"[{c.column.column_name}]") 
            |> fun s -> $" INCLUDE ({s})"
    let filterStr =
        match index.filter with
        | None -> ""
        | Some filter -> $" WHERE {filter}"
    
    let withSettings = 
        if opt.SchemazenCompatibility
        then ""
        else indexWithSettings index
            

    $"CREATE {indexTypeStr} INDEX [{indexName}] ON {parentName} ({keyColumnsStr}){includeStr}{filterStr}{withSettings}"

let generateXMLIndexScript (opt : Options) (index : INDEX) (parentName : string) (indexName : string) =
    let (includeColumns, keyColumns) =
        separateBy (fun c -> c.is_included_column) index.columns

    let keyColumnsStr = indexColumnsStr keyColumns
    
    if includeColumns |> Array.isEmpty |> not
    then failwithf "XML index with included colunms not supported (index: %s)" indexName
    
    if opt.SchemazenCompatibility
    then $"CREATE XML INDEX [{indexName}] ON {parentName} ({keyColumnsStr})"
    else $"CREATE PRIMARY XML INDEX [{indexName}] ON {parentName} ({keyColumnsStr})"

let objectFilename (schema_name :string) (object_name : string) =
    match schema_name with 
    | "dbo" -> $"{object_name}.sql"
    | _ -> $"{schema_name}.{object_name}.sql"

    
let getIndexDefinitionStr (opt : Options) parentName (index : INDEX) =
    match index.name, index.is_unique, index.index_type with
    | Some n, false, INDEX_TYPE.CLUSTERED -> 
        Some <| generateStandardIndexScript opt index parentName n "CLUSTERED"
    | Some n, false, INDEX_TYPE.NONCLUSTERED -> 
        Some <| generateStandardIndexScript opt index parentName n "NONCLUSTERED" 
    | Some n, true, INDEX_TYPE.CLUSTERED -> 
        Some <| generateStandardIndexScript opt index parentName n "UNIQUE CLUSTERED"
    | Some n, true, INDEX_TYPE.NONCLUSTERED -> 
        Some <| generateStandardIndexScript opt index parentName n "UNIQUE NONCLUSTERED"
    | Some n, false, INDEX_TYPE.XML ->
        Some <| generateXMLIndexScript opt index parentName n  
    // Heap indexes without name is ignored
    | None, false, INDEX_TYPE.HEAP -> None
    | iType -> 
        failwithf "Unhandled index type %A" iType

    
let generateTableBody (w : System.IO.StreamWriter) (opt : Options) allTypes isTableType columns tableInlineIndexes tableInlineChecks columnInlineDefaults =
    commaSeparated w "   " columns (columnDefinitionStr opt allTypes isTableType columnInlineDefaults)

    let indexDefs =
        tableInlineIndexes
        |> List.map 
            (fun (inlineIndex : INDEX) ->
                match inlineIndex.is_system_named, inlineIndex.is_primary_key, inlineIndex.is_unique_constraint with 
                | _, true, false -> $"   ,{primaryKeyStr opt isTableType inlineIndex}"
                | _, false, true -> $"   ,{uniqueKeyStr opt inlineIndex}"
                | _ -> failwithf "Unknown inline index %A" inlineIndex)
        |> List.rev
        
    let checkDefs =
        tableInlineChecks
        |> List.map 
            (fun (inlineCheck : CHECK_CONSTRAINT) ->
                if inlineCheck.is_system_named
                then 
                    let extraSpace = if opt.SchemazenCompatibility then " " else ""
                    $"   ,CHECK {extraSpace}{inlineCheck.definition}"
                else $"   ,CONSTRAINT [{inlineCheck.object.name}] CHECK ({inlineCheck.definition})")

        
    match indexDefs, checkDefs with
    | [], [] -> ()
    | _ -> 
        w.WriteLine ""
        checkDefs |> List.iter w.WriteLine
        indexDefs |> List.iter w.WriteLine


                
let generateTableScript' (w : System.IO.StreamWriter) (opt : Options) allTypes isTableType parentName columns indexes checkConstraints (defaultConstraints : DEFAULT_CONSTRAINT array) =
    let (tableInlineIndexes, standaloneIndexes) =
        indexes
        |> Array.fold 
            (fun (tableInlineIndexes', standaloneIndexes') index ->
                match index.is_primary_key || index.is_unique_constraint with
                | true -> index :: tableInlineIndexes', standaloneIndexes'
                | false -> tableInlineIndexes', index :: standaloneIndexes')
            ([],[])

    let tableInlineChecks =
        checkConstraints
        |> Array.fold 
            (fun (tableInlineChecks') cc ->
                // Schemazen inlines checks for table types
                if opt.SchemazenCompatibility && isTableType
                then cc :: tableInlineChecks'
                else tableInlineChecks')
            []

    let columnInlineDefaults =
        defaultConstraints
        |> Array.fold
            (fun tableInlineDefaults' dc ->
                 if isTableType
                 then Map.add dc.column.column_id dc tableInlineDefaults'
                 else tableInlineDefaults')
            Map.empty


    generateTableBody w opt allTypes isTableType columns tableInlineIndexes tableInlineChecks columnInlineDefaults

    w.WriteLine $")"

    w.WriteLine ""
    for index in standaloneIndexes |> List.sortBy (fun i -> i.index_id) do
        let indexStr = getIndexDefinitionStr opt parentName index
        match indexStr with
        | Some s -> w.WriteLine $"{s}"
        | None -> ()

    w.WriteLine ""
    w.WriteLine "GO"

    []
    |> (fun acc -> columns |> Array.fold (fun acc' c -> c.object.object_id :: acc') acc)
    |> (fun acc -> indexes |> Array.fold (fun acc' i -> match i.object with Some o -> o.object_id :: acc' | None -> acc') acc)
    |> (fun acc -> columnInlineDefaults |> Map.fold (fun acc' _ dc -> dc.object.object_id :: acc') acc)
    

let generateTableScript allTypes (w : System.IO.StreamWriter) (opt : Options) (t : TABLE) =
    let tableName = $"[{t.schema.name}].[{t.table_name}]"
    w.WriteLine $"CREATE TABLE {tableName} ("
    
    let object_ids =
        generateTableScript' w opt allTypes false tableName 
            t.columns t.indexes t.checkConstraints t.defaultConstraints
    
    ObjectDefinitions {| contains_objects = t.object.object_id :: object_ids; depends_on = [] |}

let generateTableTypeScript allTypes (w : System.IO.StreamWriter) (opt : Options) (t : TABLE_TYPE) =
    let tName = $"[{t.schema.name}].[{t.type_name}]"
    w.WriteLine $"CREATE TYPE {tName} AS TABLE ("
    
    let object_ids =
        generateTableScript' w opt allTypes true tName 
            t.columns t.indexes t.checkConstraints t.defaultConstraints
    ObjectDefinitions {| contains_objects = t.object.object_id :: object_ids; depends_on = [] |}


let generateViewScript (w : System.IO.StreamWriter) (opt : Options) (view : VIEW)=
    [
        "SET QUOTED_IDENTIFIER ON "; "GO"; "SET ANSI_NULLS ON "; "GO"
        view.definition
        "GO"; "SET QUOTED_IDENTIFIER OFF "; "GO"; "SET ANSI_NULLS OFF "; "GO"; ""; "GO"
    ]
    |> List.iter w.WriteLine

    ObjectDefinitions {| contains_objects = [view.object.object_id]; depends_on = [] |}

let generateCheckConstraintsScript (w : System.IO.StreamWriter) (opt : Options) (table : TABLE, ccs : CHECK_CONSTRAINT array) =
    let object_ids =
        ccs
        |> Array.fold 
            (fun acc cc ->
                let tableName = $"[{table.schema.name}].[{table.table_name}]"
                let additionalSpace = if opt.SchemazenCompatibility then " " else ""
                $"ALTER TABLE {tableName} WITH CHECK ADD CONSTRAINT [{cc.object.name}] CHECK{additionalSpace} {cc.definition}"
                |> w.WriteLine 
                "GO" |> w.WriteLine
                cc.object.object_id :: acc)
            []
    ObjectDefinitions {| contains_objects = object_ids; depends_on = [table.object.object_id] |}

let generateDefaultConstraintsScript (w : System.IO.StreamWriter) (opt : Options) (table : TABLE, dcs : DEFAULT_CONSTRAINT array) =
    let object_ids =
        dcs 
        |> Array.sortBy (fun dc -> dc.object.object_id)
        |> Array.fold
            (fun acc dc ->
                let tableName = $"[{table.schema.name}].[{table.table_name}]"
                if dc.is_system_named
                then 
                    if opt.SchemazenCompatibility
                    then $"ALTER TABLE {tableName} ADD  DEFAULT {dc.definition} FOR [{dc.column.column_name}]"
                    else $"ALTER TABLE {tableName} ADD DEFAULT {dc.definition} FOR [{dc.column.column_name}]"
                else $"ALTER TABLE {tableName} ADD CONSTRAINT [{dc.object.name}] DEFAULT {dc.definition} FOR [{dc.column.column_name}]"
                |> w.WriteLine
                "GO" |> w.WriteLine
                
                dc.object.object_id :: acc)
            []
    ObjectDefinitions {| contains_objects = object_ids; depends_on = [table.object.object_id] |}

let generateForeignKeysScript (w : System.IO.StreamWriter) (opt : Options) (table : TABLE, fks : FOREIGN_KEY array) =
    let (object_ids, depends_on) =
        fks
        |> Array.fold 
            (fun (object_ids, depends_on) fk ->
                let columnsStr = fk.columns |> Array.joinBy ", " (fun c -> $"[{c.parent_column.column_name}]")
                let refColumnsStr = fk.columns |> Array.joinBy ", " (fun c -> $"[{c.referenced_column.column_name}]")
                let name = if fk.is_system_named then "" else $"CONSTRAINT [{fk.name}]"
                w.WriteLine $"ALTER TABLE [{table.schema.name}].[{table.table_name}] WITH CHECK ADD {name}"
                w.WriteLine $"   FOREIGN KEY({columnsStr}) REFERENCES [{fk.referenced.schema.name}].[{fk.referenced.name}] ({refColumnsStr})"
                match fk.update_referential_action with
                | REFERENTIAL_ACTION.No_action -> () 
                | REFERENTIAL_ACTION.Cascade -> w.WriteLine "   ON UPDATE CASCADE"
                | REFERENTIAL_ACTION.Set_null -> w.WriteLine "   ON UPDATE SET NULL"
                | REFERENTIAL_ACTION.Set_default -> w.WriteLine "   ON UPDATE SET DEFAULT"
                match fk.delete_referential_action with
                | REFERENTIAL_ACTION.No_action -> () 
                | REFERENTIAL_ACTION.Cascade -> w.WriteLine "   ON DELETE CASCADE"
                | REFERENTIAL_ACTION.Set_null -> w.WriteLine "   ON DELETE SET NULL"
                | REFERENTIAL_ACTION.Set_default -> w.WriteLine "   ON DELETE SET DEFAULT"
                w.WriteLine ""
                w.WriteLine "GO"

                fk.object.object_id :: object_ids,
                fk.parent.object_id :: fk.referenced.object_id :: depends_on)
            ([], [])
    ObjectDefinitions {| contains_objects = object_ids; depends_on = depends_on |}

let generateTriggerScript (w : System.IO.StreamWriter) (opt : Options) (trigger : TRIGGER) =
    [
        "SET QUOTED_IDENTIFIER ON "; "GO"; "SET ANSI_NULLS ON "; "GO"
        if opt.SchemazenCompatibility then trigger.orig_definition else trigger.definition
        "GO"; "SET QUOTED_IDENTIFIER OFF "; "GO"; "SET ANSI_NULLS OFF "; "GO"; ""; 
        $"ENABLE TRIGGER [{trigger.object.schema.name}].[{trigger.object.name}] ON [{trigger.parent.schema.name}].[{trigger.parent.name}]"
        "GO"; ""; "GO"
    ]
    |> List.iter w.WriteLine

    ObjectDefinitions 
        {| 
            contains_objects = [trigger.object.object_id]
            depends_on = [trigger.parent.object_id]
        |}

let generateProcedureScript (w : System.IO.StreamWriter) (opt : Options) (p : PROCEDURE) =
    [
        "SET QUOTED_IDENTIFIER ON "; "GO"; "SET ANSI_NULLS ON "; "GO"
        if opt.SchemazenCompatibility then p.orig_definition else p.definition
        "GO"; "SET QUOTED_IDENTIFIER OFF "; "GO"; "SET ANSI_NULLS OFF "; "GO"; ""; "GO"
    ]
    |> List.iter w.WriteLine
    ObjectDefinitions {| contains_objects = [p.object.object_id]; depends_on = [] |}

let generateSynonymScript (w : System.IO.StreamWriter) (opt : Options) (synonym : SYNONYM) =
    w.WriteLine $"CREATE SYNONYM [{synonym.object.schema.name}].[{synonym.object.name}] FOR {synonym.base_object_name}"
    w.WriteLine "GO"
    ObjectDefinitions {| contains_objects = [synonym.object.object_id]; depends_on = [] |}


let generateXmlSchemaCollectionScript (w : System.IO.StreamWriter) (opt : Options) (s : XML_SCHEMA_COLLECTION) =
    let name =
        if opt.SchemazenCompatibility
        then $"{s.schema.name}.{s.name}"
        else $"[{s.schema.name}].[{s.name}]"
    w.WriteLine $"CREATE XML SCHEMA COLLECTION {name} AS N'{s.definition}'"
    w.WriteLine "GO"
    XmlSchemaCollectionDefinition

(*
CREATE SEQUENCE [schema_name . ] sequence_name  
    [ AS [ built_in_integer_type | user-defined_integer_type ] ]  
    [ START WITH <constant> ]  
    [ INCREMENT BY <constant> ]  
    [ { MINVALUE [ <constant> ] } | { NO MINVALUE } ]  
    [ { MAXVALUE [ <constant> ] } | { NO MAXVALUE } ]  
    [ CYCLE | { NO CYCLE } ]  
    [ { CACHE [ <constant> ] } | { NO CACHE } ]  
    [ ; ]  
*)

let generateSequenceScript (w : System.IO.StreamWriter) (opt : Options) (s : SEQUENCE) =
    let name = $"[{s.object.schema.name}].[{s.object.name}]"
    let typeStr = DATATYPE.typeStr opt.SchemazenCompatibility false s.data_type
    let startWith = match s.sequence_definition.start_value with Some v -> $" START WITH {v}" | None -> ""
    let incrementBy = $" INCREMENT BY {s.sequence_definition.increment}"
    let minValue = match s.sequence_definition.minimum_value with Some v -> $" MINVALUE {v}" | None -> " NO MINVALUE"
    let maxValue = match s.sequence_definition.maximum_value with Some v -> $" MAXVALUE {v}" | None -> " NO MAXVALUE"
    let cycle = if s.is_cycling then " CYCLE" else " NO CYCLE"
    let cache = match s.cache_size with Some s -> $" CACHE {s}" | None -> if s.is_cached then "" else " NO CACHE"
    w.WriteLine $"CREATE SEQUENCE {name} AS {typeStr}{startWith}{incrementBy}{minValue}{maxValue}{cycle}{cache}"

    ObjectDefinitions {| contains_objects = [s.object.object_id]; depends_on = [] |}

(*
CREATE TYPE [ schema_name. ] type_name
{
      FROM base_type
      [ ( precision [ , scale ] ) ]
      [ NULL | NOT NULL ]
    | EXTERNAL NAME assembly_name [ .class_name ]
    | AS TABLE ( { <column_definition> | <computed_column_definition> [ , ...n ]
      [ <table_constraint> ] [ , ...n ]
      [ <table_index> ] [ , ...n ] } )
} [ ; ]
*)

let generateUserDefinedTypeScript (types : Map<int, DATATYPE>) (w : System.IO.StreamWriter) (opt : Options) (t : DATATYPE) =
    let tDef =
        let base_type = types |> Map.find (int t.system_type_id)
        DATATYPE.typeStr' opt.SchemazenCompatibility true base_type.name base_type.sys_datatype t.parameter
    let nullStr = if t.parameter.is_nullable then "NULL" else "NOT NULL"
    w.WriteLine $"CREATE TYPE [{t.schema.name}].[{t.name}] FROM {tDef} {nullStr}"
    w.WriteLine "GO"
    UserDefinedTypeDefinition


let generateScripts (opt : Options) (schema : DATABASE) scriptConsumer =
    let db = schema
    let dataForFolder subfolderName (nameFn : 'a -> string) f (xs : 'a list) =
        if xs |> List.isEmpty |> not 
        then
            for x in xs do
                let script =
                    use ms = new System.IO.MemoryStream()
                    let scriptObjects =
                        use w = new System.IO.StreamWriter(ms)
                        f w opt x
                    let (priority, contains_objects, depends_on) =
                        match scriptObjects with
                        | SchemaDefinition -> 1, [],[]
                        | ObjectDefinitions x -> 3, x.contains_objects, x.depends_on
                        | UserDefinedTypeDefinition -> 2,  [],[]
                        | XmlSchemaCollectionDefinition -> 4, [], []
                        
                    {
                        contains_objects = Set.ofList contains_objects
                        depends_on = Set.ofList depends_on
                        priority = priority
                        
                        content  = { 
                            directory_name = subfolderName; 
                            filename = nameFn x; 
                            content = ms.ToArray () |> System.Text.Encoding.UTF8.GetString
                        }
                    }
                scriptConsumer script
    
    let allTypes = 
        db.TYPES
        |> List.map (fun t -> t.user_type_id, t)
        |> Map.ofList
    
    db.SCHEMAS |> List.filter (fun s -> not s.is_system_schema)
    |> dataForFolder "schemas" (fun s -> $"{s.name}.sql") generateSchemaScript

    db.TYPES
    |> List.filter (fun t -> t.is_user_defined && t.table_datatype.IsNone)
    |> dataForFolder "user_defined_types" (fun t -> objectFilename t.schema.name t.name) (generateUserDefinedTypeScript allTypes)

    db.TABLE_TYPES
    |> dataForFolder "table_types" (fun t -> $"TYPE_{t.type_name}.sql") (generateTableTypeScript allTypes)

    db.TABLES |> dataForFolder "tables" (fun t -> objectFilename t.schema.name t.table_name) (generateTableScript allTypes)
    
    db.VIEWS 
    |> dataForFolder "views" (fun v -> objectFilename v.schema.name v.view_name) generateViewScript
    db.VIEWS 
    |> List.collect 
        (fun v -> 
            v.indexes 
            |> Array.choose 
                (fun i -> 
                    match i.name, getIndexDefinitionStr opt $"[{v.schema.name}].[{v.view_name}]" i with 
                    | Some n, Some def -> Some (n, v, i, def) 
                    | _ -> None) 
            |> Array.toList) 
    |> dataForFolder "views" (fun (name,_view,_index,_def) -> $"{name}.sql") 
        (fun w _o (_name,view,index,def)-> 
            w.WriteLine def
            w.WriteLine "GO"
            ObjectDefinitions 
                {| 
                    contains_objects = match index.object with Some o -> [o.object_id] | None -> []; 
                    depends_on = [view.object.object_id] 
                |})
    
    db.PROCEDURES |> List.filter  (fun p -> p.object.object_type <> OBJECT_TYPE.SQL_STORED_PROCEDURE)
    |> dataForFolder "functions" (fun p -> objectFilename p.object.schema.name p.name) generateProcedureScript

    db.TABLES |> List.choose (fun t -> match t.checkConstraints |> Array.filter (fun cc -> not cc.is_system_named) with [||] -> None | ccs -> Some (t, ccs))
    |> dataForFolder "check_constraints" (fun (t, _) -> objectFilename t.schema.name t.table_name) generateCheckConstraintsScript

    db.TABLES |> List.choose (fun t -> match t.defaultConstraints with [||] -> None | dcs -> Some (t, dcs))
    |> dataForFolder "defaults" (fun (t, _) -> objectFilename t.schema.name t.table_name) generateDefaultConstraintsScript

    db.TABLES 
    |> List.collect (fun t -> t.triggers |> Array.toList)
    |> dataForFolder "triggers" (fun tr -> objectFilename tr.object.schema.name tr.trigger_name) generateTriggerScript

    db.TABLES |> List.choose (fun t -> match t.foreignKeys with [||] -> None | fks -> Some (t, fks))
    |> dataForFolder "foreign_keys" (fun (t, _) -> objectFilename t.schema.name t.table_name) generateForeignKeysScript

    db.PROCEDURES |> List.filter  (fun p -> p.object.object_type = OBJECT_TYPE.SQL_STORED_PROCEDURE)
    |> dataForFolder "procedures" (fun p -> objectFilename p.object.schema.name p.name) generateProcedureScript

    db.SYNONYMS
    |> dataForFolder "synonyms" (fun s -> objectFilename s.object.schema.name s.object.name) generateSynonymScript

    db.XML_SCHEMA_COLLECTIONS
    |> dataForFolder "xmlschemacollections" (fun s -> objectFilename s.schema.name s.name) generateXmlSchemaCollectionScript

    if not (opt.SchemazenCompatibility)
    then 
        db.SEQUENCES
        |> dataForFolder "sequences" (fun s -> objectFilename s.object.schema.name s.object.name) generateSequenceScript    

