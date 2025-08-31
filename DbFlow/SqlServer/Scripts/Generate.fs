module DbFlow.SqlServer.Scripts.Generate

open DbFlow
open DbFlow.SqlServer.Schema

type Script = { 
    Subdirectory : string option; 
    Filename : string; 
    Content : string; 
}

type SchemaScriptPart =
    | DatabaseDefinition
    | SchemaDefinition
    | UserDefinedTypeDefinition
    | ObjectDefinitions of {| Contains : int list; DependsOn : int list |}
    | XmlSchemaCollectionDefinition

module XProperties =
    let xPropStr (propName : string) (propValue : string) keysAndValues =
        let escapedValue = propValue.Replace("'", "''")
        match keysAndValues with
        | [key0, value0] ->
            $"EXECUTE [sys].[sp_addextendedproperty] N'{propName}', N'{escapedValue}', {key0}, {value0};"
        | [key0, value0; key1, value1] ->
            $"EXECUTE [sys].[sp_addextendedproperty] N'{propName}', N'{escapedValue}', {key0}, {value0}, {key1}, {value1};"
        | [key0, value0; key1, value1; key2, value2] ->
            $"EXECUTE [sys].[sp_addextendedproperty] N'{propName}', N'{escapedValue}', {key0}, {value0}, {key1}, {value1}, {key2}, {value2};"
        
        | _ -> failwithf "Can not generate documentation script for %A" keysAndValues

    let xProps (xProperties : Map<string, string>) keysAndValues f seed =
        xProperties
        |> Map.fold (fun acc k v -> f acc (xPropStr k v keysAndValues)) seed
    
    let formatXProps wl (xProperties : Map<string, string>) keysAndValues =
        if not xProperties.IsEmpty
        then wl ""
        xProps xProperties keysAndValues (fun () xp -> wl xp; ()) ()
    
    let database wl (xProperties : Map<string, string>) =
        formatXProps wl xProperties ["NULL", "NULL";]
    
    let xmlSchemaCollection wl (xmlSchema : XmlSchemaCollection) =
        formatXProps wl xmlSchema.XProperties 
            ["N'SCHEMA'", $"[{xmlSchema.Schema.Name}]"; "N'XML SCHEMA COLLECTION'", $"[{xmlSchema.Name}]";]

    let schema wl (s : Schema) =
        formatXProps wl s.XProperties
            ["N'SCHEMA'", $"[{s.Name}]";]
    
    let procedure wl (p : Procedure) =
        let pType =
            match p.Object.ObjectType with
            | ObjectType.SqlScalarFunction 
            | ObjectType.SqlInlineTableValuedFunction
            | ObjectType.SqlTableValuedFunction  -> "FUNCTION"
            | ObjectType.SqlStoredProcedure -> "PROCEDURE"
            | t -> failwithf "Unsupported object type %A" t
                
        p.Parameters
        |> Array.fold 
            (fun acc para ->
                xProps para.XProperties
                    ["N'SCHEMA'", $"[{p.Object.Schema.Name}]"; $"N'{pType}'", $"[{p.Name}]"; "N'PARAMETER'", $"'{para.Name}'"]
                    (fun acc' x -> x :: acc')
                    acc)
            (xProps p.XProperties 
                ["N'SCHEMA'", $"[{p.Object.Schema.Name}]"; $"N'{pType}'", $"[{p.Name}]"]
                (fun acc x -> x :: acc)
                [])
        |> function
            | [] -> ()
            | ds ->
                wl ""
                ds |> List.iter wl

    let constraint' wl (schemaName : string) (tableName : string) constraintName xProperties =
        xProps xProperties ["N'SCHEMA'", $"[{schemaName}]"; "N'TABLE'", $"[{tableName}]"; "N'CONSTRAINT'", $"[{constraintName}]"]
            (fun acc x -> x :: acc)
            []
        |> function 
            | [] -> ()
            | ds -> 
                wl ""
                ds |> List.iter wl 
                wl ""
                wl "GO"
    
    let foreignKey wl (t : Table) (fk : ForeignKey) = constraint' wl t.Schema.Name t.Name fk.Name fk.XProperties 
    let checkConstraint wl (schemaName : string) (tableName : string) (cc : CheckConstraint) = 
        constraint' wl schemaName tableName cc.Object.Name cc.XProperties
    let defaultConstraint wl (schemaName : string) (tableName : string) (dc : DefaultConstraint) = 
        constraint' wl schemaName tableName dc.Object.Name dc.XProperties 
            
    let trigger wl (tr : Trigger) =
        formatXProps wl tr.XProperties
            ["N'SCHEMA'", $"[{tr.Parent.Schema.Name}]"; "N'TABLE'", $"[{tr.Parent.Name}]"; "N'TRIGGER'", $"[{tr.Name}]"]
    
        
    let index wl cType (i : Index) =
        match i.Name with
        | Some iName ->
            formatXProps wl i.XProperties 
                ["N'SCHEMA'", $"[{i.Parent.Schema.Name}]"; cType, $"[{i.Parent.Name}]"; "N'INDEX'", $"[{iName}]"]
        | None -> ()
    
    let containerAndColumns wl schemaName cType cName cXProperties (columns : Column array) =
        columns
        |> Array.fold 
            (fun acc c ->
                xProps c.XProperties
                    ["N'SCHEMA'", $"[{schemaName}]"; cType, $"[{cName}]"; "N'COLUMN'", $"[{c.Name}]"]
                    (fun acc' x -> x :: acc')
                    acc)
            (xProps cXProperties
                ["N'SCHEMA'", $"[{schemaName}]"; cType, $"[{cName}]";]
                (fun acc x -> x :: acc)
                [])
        |> function 
            | [] -> () 
            | docs ->
                wl ""
                docs |> List.iter wl

    let table wl (t : Table) =
        containerAndColumns wl t.Schema.Name "N'TABLE'" t.Name t.XProperties t.Columns
        t.Indexes
        |> Array.iter (index wl "N'TABLE'")

    let viewAndColumns wl (v : View) =
        containerAndColumns wl v.Schema.Name "N'VIEW'" v.Name v.XProperties v.Columns

    let typeProps wl (s : Schema) (typeName : string) (xProps : Map<string, string>) =
        formatXProps wl xProps  ["N'SCHEMA'", $"[{s.Name}]"; "N'TYPE'", $"[{typeName}]";]

        
let columnDefinitionStr (opt : Options) (dbProps : DatabaseProperties) allTypes isTableType (columnInlineDefaults : Map<int, DefaultConstraint>) (column : Column) =
    let columnDefStr =
        match column.ComputedDefinition with
        | Some computed ->
            let persistStr = 
                if computed.IsPersisted 
                then 
                    if column.Datatype.Parameter.IsNullable 
                    then " PERSISTED" 
                    else " PERSISTED NOT NULL"
                else ""
            $"AS {computed.ComputedDefinition}{persistStr}"
        | None -> 
            let collateStr = 
                match column.Datatype.Parameter.CollationName with
                | Some c when c <> dbProps.collation_name -> $" COLLATE {c}"
                | _ -> ""
            let nullStr = if column.Datatype.Parameter.IsNullable then "NULL" else "NOT NULL"
            let maskedStr =
                match column.MaskingFunction with
                | Some m -> $" MASKED WITH ( FUNCTION = '{m}' )"
                | None -> ""
            let identityStr = 
                match column.IdentityDefinition with
                | Some def -> $"\r\n      IDENTITY ({def.SeedValue},{def.IncrementValue})"
                | None -> ""
            let checkStr = 
                match Map.tryFind column.ColumnId columnInlineDefaults with
                | Some dc -> $"\r\n       DEFAULT {dc.Definition}"
                | None -> ""
            let rowGuidStr =
                if column.IsRowguidcol
                then " ROWGUIDCOL "
                else ""
            let typeStr = 
                Datatype.typeStr column.Datatype
            $"{typeStr}{collateStr}{maskedStr} {nullStr}{identityStr}{checkStr}{rowGuidStr}"
    $"[{column.Name}] {columnDefStr}"

let separateBy f xs =
    xs 
    |> Array.fold (fun (xs, ys) x -> if f x then x :: xs, ys else xs, x :: ys) ([], [])
    |> fun (xs, ys) -> xs |> List.rev |> List.toArray, ys |> List.rev |> List.toArray
    
let generateSettingsScript (w : System.IO.StreamWriter) (opt : Options) (schema : DatabaseSchema) =
    let wl : string -> unit = w.WriteLine
    let s = schema.Properties
    let onOff isSet = if isSet then "ON" else "OFF"
    let recoveryModel = match s.recovery_model with 1uy -> "FULL" | 2uy -> "BULK_LOGGED" | 3uy -> "SIMPLE" | rm -> failwithf "Unknown recovery model %i" rm
    let parameterization = if s.is_parameterization_forced then "FORCED" else "SIMPLE"
    let cursorDefault = if s.is_local_cursor_default then "LOCAL" else "GLOBAL"
    let allowSnapshotIsolation = match s.snapshot_isolation_state with 0uy | 2uy -> "OFF" | 1uy | 3uy -> "ON" | sis -> failwithf "Unknown snapshot isolation state %i" sis
    let pageVerify = match s.page_verify_option with 0uy -> "NONE" | 1uy -> "TORN_PAGE_DETECTION" | 2uy -> "CHECKSUM" | sis -> failwithf "Unknown page verify option %i" sis
    wl "DECLARE @DB VARCHAR(255)"
    wl "SET @DB = DB_NAME()"
    wl $"EXEC dbo.sp_dbcmptlevel @DB, {s.compatibility_level}"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] COLLATE {s.collation_name}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_CLOSE {onOff s.is_auto_close_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_SHRINK {onOff s.is_auto_shrink_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET ALLOW_SNAPSHOT_ISOLATION {allowSnapshotIsolation}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET READ_COMMITTED_SNAPSHOT {onOff s.is_read_committed_snapshot_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET RECOVERY {recoveryModel}')" 
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET PAGE_VERIFY {pageVerify}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_CREATE_STATISTICS {onOff s.is_auto_create_stats_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_UPDATE_STATISTICS {onOff s.is_auto_update_stats_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_UPDATE_STATISTICS_ASYNC {onOff s.is_auto_update_stats_async_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_NULL_DEFAULT {onOff s.is_ansi_null_default_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_NULLS {onOff s.is_ansi_nulls_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_PADDING {onOff s.is_ansi_padding_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_WARNINGS {onOff s.is_ansi_warnings_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET ARITHABORT {onOff s.is_arithabort_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET CONCAT_NULL_YIELDS_NULL {onOff s.is_concat_null_yields_null_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET NUMERIC_ROUNDABORT {onOff s.is_numeric_roundabort_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET QUOTED_IDENTIFIER {onOff s.is_quoted_identifier_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET RECURSIVE_TRIGGERS {onOff s.is_recursive_triggers_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET CURSOR_CLOSE_ON_COMMIT {onOff s.is_cursor_close_on_commit_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET CURSOR_DEFAULT {cursorDefault}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET TRUSTWORTHY {onOff s.is_trustworthy_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET DB_CHAINING {onOff s.is_db_chaining_on}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET PARAMETERIZATION {parameterization}')"
    wl $"EXEC('ALTER DATABASE [' + @DB + '] SET DATE_CORRELATION_OPTIMIZATION {onOff s.is_date_correlation_on}')"
    wl "GO"
    
    XProperties.database wl schema.XProperties

    DatabaseDefinition


let generateSchemaScript (w : System.IO.StreamWriter) (opt : Options) (schema : Schema) =
    w.WriteLine $"CREATE SCHEMA [{schema.Name}] AUTHORIZATION [{schema.PrincipalName}]"
    w.WriteLine "GO"

    XProperties.schema w.WriteLine schema

    SchemaDefinition

let commaSeparated (w : System.IO.StreamWriter) (indentionStr : string) xs (formatter : _ -> string) =
    let n = xs |> Array.length
    xs 
    |> Array.iteri
        (fun i x -> 
            let commaStr = if i = n - 1 then "" else ","
            w.WriteLine $"{indentionStr}{formatter x}{commaStr}")

let indexColumnsStr (columns : IndexColumn array) =
    columns
    |> Array.joinBy 
        ", "
        (fun c -> 
            let descStr = if c.IsDescendingKey then " DESC" else ""
            $"[{c.Column.Name}]{descStr}")

let indexWithSettings parentIsView (index : Index) =
    [|
        match index.FillFactor with 0uy -> "" | ff -> $"FILLFACTOR = {ff}"
        if index.IgnoreDupKey then "IGNORE_DUP_KEY = ON" else ""
    |]
    |> Array.filter (fun s -> s.Length > 0)
    |> Array.joinBy ", " id
    |> function "" -> "" | s -> $"\r\n    WITH( {s} )"

let primaryKeyStr (opt : Options) isTableType (pk : Index) =
    let pkName = 
        match pk.Name with Some n -> n | None -> failwithf "Can not handle PK without name"
    let clusteredStr = 
        match pk.IndexType with
        | IndexType.Clustered -> "CLUSTERED"
        | IndexType.Nonclustered -> "NONCLUSTERED"
        | iType -> failwithf "Unhandled index type %A of primary key %s" iType pkName
    let pkColumnsStr = indexColumnsStr pk.Columns 
    let withSettings = indexWithSettings false pk
    if isTableType || pk.IsSystemNamed
    then $"PRIMARY KEY {clusteredStr} ({pkColumnsStr}){withSettings}"
    else $"CONSTRAINT [{pkName}] PRIMARY KEY {clusteredStr} ({pkColumnsStr}){withSettings}"
        
let uniqueKeyStr (opt : Options) (i : Index) =
    let iName = 
        match i.Name with Some n -> n | None -> failwithf "Can not handle UNIQUE CONSTRAINTS without name"
    let clusteredStr = 
        match i.IndexType with
        | IndexType.Clustered -> "CLUSTERED"
        | IndexType.Nonclustered -> "NONCLUSTERED"
        | iType -> failwithf "Unhandled index type %A of primary key %s" iType iName
    let indexColumnsStr = indexColumnsStr i.Columns
    if (not i.IsSystemNamed)
    then $"CONSTRAINT [{iName}] UNIQUE {clusteredStr} ({indexColumnsStr})"
    else $"UNIQUE {clusteredStr} ({indexColumnsStr})"

let generateStandardIndexScript (opt : Options) (index : Index) (parentName : string) parentIsView (indexName : string) (indexTypeStr : string) =
    let (includeColumns, keyColumns) =
        separateBy (fun c -> c.IsIncludedColumn) index.Columns

    let keyColumnsStr = indexColumnsStr keyColumns
    let includeStr = 
        if Array.isEmpty includeColumns
        then ""
        else 
            includeColumns
            |> Array.joinBy ", " (fun c -> $"[{c.Column.Name}]") 
            |> fun s -> $" INCLUDE ({s})"
    let filterStr =
        match index.Filter with
        | None -> ""
        | Some filter -> $" WHERE {filter}"
    
    let withSettings = 
        indexWithSettings parentIsView index

    $"CREATE {indexTypeStr} INDEX [{indexName}] ON {parentName} ({keyColumnsStr}){includeStr}{filterStr}{withSettings}"

    

let generateXMLIndexScript (opt : Options) (index : Index) (parentName : string) parentIsView (indexName : string) =
    let (includeColumns, keyColumns) =
        separateBy (fun c -> c.IsIncludedColumn) index.Columns

    let keyColumnsStr = indexColumnsStr keyColumns
    
    if includeColumns |> Array.isEmpty |> not
    then failwithf "XML index with included colunms not supported (index: %s)" indexName
    
    $"CREATE PRIMARY XML INDEX [{indexName}] ON {parentName} ({keyColumnsStr})"

let objectFilename (schema_name :string) (object_name : string) =
    match schema_name with 
    | "dbo" -> $"{object_name}.sql"
    | _ -> $"{schema_name}.{object_name}.sql"

    
let getIndexDefinitionStr (opt : Options) parentName parentIsView (index : Index) =
    match index.Name, index.IsUnique, index.IndexType with
    | Some n, false, IndexType.Clustered -> 
        Some <| generateStandardIndexScript opt index parentName parentIsView n "CLUSTERED"
    | Some n, false, IndexType.Nonclustered -> 
        Some <| generateStandardIndexScript opt index parentName parentIsView n "NONCLUSTERED" 
    | Some n, true, IndexType.Clustered -> 
        Some <| generateStandardIndexScript opt index parentName parentIsView n "UNIQUE CLUSTERED"
    | Some n, true, IndexType.Nonclustered -> 
        Some <| generateStandardIndexScript opt index parentName parentIsView n "UNIQUE NONCLUSTERED"
    | Some n, false, IndexType.Xml ->
        Some <| generateXMLIndexScript opt index parentName parentIsView n  
    // Heap indexes without name is ignored
    | None, false, IndexType.Heap -> None
    | iType -> 
        failwithf "Unhandled index type %A" iType

    
let generateTableBody (w : System.IO.StreamWriter) (opt : Options) ds allTypes isTableType columns tableInlineIndexes tableInlineChecks columnInlineDefaults =
    commaSeparated w "   " columns (columnDefinitionStr opt ds allTypes isTableType columnInlineDefaults)

    let indexDefs =
        tableInlineIndexes
        |> List.map 
            (fun (inlineIndex : Index) ->
                match inlineIndex.IsSystemNamed, inlineIndex.IsPrimaryKey, inlineIndex.IsUniqueConstraint with 
                | _, true, false -> $"   ,{primaryKeyStr opt isTableType inlineIndex}"
                | _, false, true -> $"   ,{uniqueKeyStr opt inlineIndex}"
                | _ -> failwithf "Unknown inline index %A" inlineIndex)
        |> List.rev
        
    let checkDefs =
        tableInlineChecks
        |> List.map 
            (fun (inlineCheck : CheckConstraint) ->
                if inlineCheck.IsSystemNamed
                then $"   ,CHECK {inlineCheck.Definition}"
                else $"   ,CONSTRAINT [{inlineCheck.Object.Name}] CHECK ({inlineCheck.Definition})")

        
    match indexDefs, checkDefs with
    | [], [] -> ()
    | _ -> 
        w.WriteLine ""
        checkDefs |> List.iter w.WriteLine
        indexDefs |> List.iter w.WriteLine


                
let generateTableScript' (w : System.IO.StreamWriter) (opt : Options) ds allTypes isTableType parentName columns indexes checkConstraints (defaultConstraints : DefaultConstraint array) =
    let (tableInlineIndexes, standaloneIndexes) =
        indexes
        |> Array.fold 
            (fun (tableInlineIndexes', standaloneIndexes') index ->
                match index.IsPrimaryKey || index.IsUniqueConstraint with
                | true -> index :: tableInlineIndexes', standaloneIndexes'
                | false -> tableInlineIndexes', index :: standaloneIndexes')
            ([],[])

    let tableInlineChecks =
        checkConstraints
        |> Array.fold 
            (fun (tableInlineChecks') cc ->
                if isTableType
                then cc :: tableInlineChecks'
                else tableInlineChecks')
            []

    let columnInlineDefaults =
        defaultConstraints
        |> Array.fold
            (fun tableInlineDefaults' dc ->
                 if isTableType
                 then Map.add dc.Column.ColumnId dc tableInlineDefaults'
                 else tableInlineDefaults')
            Map.empty


    generateTableBody w opt ds allTypes isTableType columns tableInlineIndexes tableInlineChecks columnInlineDefaults

    w.WriteLine $")"

    w.WriteLine ""
    for index in standaloneIndexes |> List.sortBy (fun i -> i.IndexId) do
        let indexStr = getIndexDefinitionStr opt parentName false index
        match indexStr with
        | Some s -> 
            w.WriteLine s
            match index.IsDisabled, index.Name with
            | true, Some iName -> 
                w.WriteLine $"ALTER INDEX {iName} ON {parentName} DISABLE"
            | _ -> ()
        | None -> ()

    w.WriteLine ""
    w.WriteLine "GO"

    []
    |> (fun acc -> columns |> Array.fold (fun acc' c -> c.Object.ObjectId :: acc') acc)
    |> (fun acc -> indexes |> Array.fold (fun acc' i -> match i.Object with Some o -> o.ObjectId :: acc' | None -> acc') acc)
    |> (fun acc -> columnInlineDefaults |> Map.fold (fun acc' _ dc -> dc.Object.ObjectId :: acc') acc)
    

let generateTableScript allTypes ds (w : System.IO.StreamWriter) (opt : Options) (t : Table) =
    let tableName = $"[{t.Schema.Name}].[{t.Name}]"
    w.WriteLine $"CREATE TABLE {tableName} ("
    
    let objectIds =
        generateTableScript' w opt ds allTypes false tableName 
            t.Columns t.Indexes t.CheckConstraints t.DefaultConstraints
    
    XProperties.table w.WriteLine t

    ObjectDefinitions {| Contains = t.Object.ObjectId :: objectIds; DependsOn = [] |}

let generateTableTypeScript allTypes ds (w : System.IO.StreamWriter) (opt : Options) (ty : Datatype, tt : TableType) =
    let tName = $"[{tt.Schema.Name}].[{tt.Name}]"
    w.WriteLine $"CREATE TYPE {tName} AS TABLE ("
    
    let object_ids =
        generateTableScript' w opt ds allTypes true tName 
            tt.Columns tt.Indexes tt.CheckConstraints tt.DefaultConstraints
    
    XProperties.typeProps w.WriteLine tt.Schema tt.Name ty.XProperties

    ObjectDefinitions {| Contains = tt.Object.ObjectId :: object_ids; DependsOn = [] |}


let generateViewScript (w : System.IO.StreamWriter) (opt : Options) (view : View)=
    [
        "SET QUOTED_IDENTIFIER ON "; "GO"; "SET ANSI_NULLS ON "; "GO"
        view.Definition
        "GO"; "SET QUOTED_IDENTIFIER OFF "; "GO"; "SET ANSI_NULLS OFF "; "GO"; ""; "GO"
    ]
    |> List.iter w.WriteLine
    
    XProperties.viewAndColumns w.WriteLine view

    ObjectDefinitions {| Contains = [view.Object.ObjectId]; DependsOn = [] |}


let generateCheckConstraintsScript (w : System.IO.StreamWriter) (opt : Options) 
                            (schemaName : string, tableName : string, table_object_id : int, ccs : CheckConstraint array) =
    let object_ids =
        ccs
        |> Array.fold 
            (fun acc cc ->
                let tableFullname = $"[{schemaName}].[{tableName}]"
                if cc.IsSystemNamed
                then w.WriteLine $"ALTER TABLE {tableFullname} WITH CHECK ADD CHECK {cc.Definition}"
                else 
                    let trustClause = if cc.IsNotTrusted then "NOCHECK" else "CHECK" 
                    w.WriteLine $"ALTER TABLE {tableFullname} WITH {trustClause} ADD CONSTRAINT [{cc.Object.Name}] CHECK {cc.Definition}"
                    if cc.IsDisabled
                    then w.WriteLine $"ALTER TABLE {tableFullname} NOCHECK CONSTRAINT [{cc.Object.Name}]"
                "GO" |> w.WriteLine

                XProperties.checkConstraint w.WriteLine schemaName tableName cc

                cc.Object.ObjectId :: acc)
            []
    ObjectDefinitions {| Contains = object_ids; DependsOn = [table_object_id] |}

let generateDefaultConstraintsScript (w : System.IO.StreamWriter) (opt : Options) (table : Table, dcs : DefaultConstraint array) =
    let object_ids =
        dcs 
        |> Array.sortBy (fun dc -> dc.Object.ObjectId)
        |> Array.fold
            (fun acc dc ->
                let tableName = $"[{table.Schema.Name}].[{table.Name}]"
                if dc.IsSystemNamed
                then $"ALTER TABLE {tableName} ADD DEFAULT {dc.Definition} FOR [{dc.Column.Name}]"
                else $"ALTER TABLE {tableName} ADD CONSTRAINT [{dc.Object.Name}] DEFAULT {dc.Definition} FOR [{dc.Column.Name}]"
                |> w.WriteLine
                "GO" |> w.WriteLine

                XProperties.defaultConstraint w.WriteLine table.Schema.Name table.Name dc
                
                dc.Object.ObjectId :: acc)
            []
    ObjectDefinitions {| Contains = object_ids; DependsOn = [table.Object.ObjectId] |}

let generateForeignKeysScript (w : System.IO.StreamWriter) (opt : Options) (table : Table, fks : ForeignKey array) =
    let (object_ids, depends_on) =
        fks
        |> Array.fold 
            (fun (object_ids, depends_on) fk ->
                let columnsStr = fk.Columns |> Array.joinBy ", " (fun c -> $"[{c.ParentColumn.Name}]")
                let refColumnsStr = fk.Columns |> Array.joinBy ", " (fun c -> $"[{c.ReferencedColumn.Name}]")
                let name = if fk.IsSystemNamed then "" else $"CONSTRAINT [{fk.Name}]"
                w.WriteLine $"ALTER TABLE [{table.Schema.Name}].[{table.Name}] WITH CHECK ADD {name}"
                w.WriteLine $"   FOREIGN KEY({columnsStr}) REFERENCES [{fk.Referenced.Schema.Name}].[{fk.Referenced.Name}] ({refColumnsStr})"
                match fk.UpdateReferentialAction with
                | ReferentialAction.NoAction -> () 
                | ReferentialAction.Cascade -> w.WriteLine "   ON UPDATE CASCADE"
                | ReferentialAction.SetNull -> w.WriteLine "   ON UPDATE SET NULL"
                | ReferentialAction.SetDefault -> w.WriteLine "   ON UPDATE SET DEFAULT"
                match fk.DeleteReferentialAction with
                | ReferentialAction.NoAction -> () 
                | ReferentialAction.Cascade -> w.WriteLine "   ON DELETE CASCADE"
                | ReferentialAction.SetNull -> w.WriteLine "   ON DELETE SET NULL"
                | ReferentialAction.SetDefault -> w.WriteLine "   ON DELETE SET DEFAULT"
                w.WriteLine ""
                w.WriteLine "GO"

                XProperties.foreignKey w.WriteLine table fk

                fk.Object.ObjectId :: object_ids,
                fk.Parent.ObjectId :: fk.Referenced.ObjectId :: depends_on)
            ([], [])
    ObjectDefinitions {| Contains = object_ids; DependsOn = depends_on |}

let generateTriggerScript (w : System.IO.StreamWriter) (opt : Options) (trigger : Trigger) =
    [
        "SET QUOTED_IDENTIFIER ON "; "GO"; "SET ANSI_NULLS ON "; "GO"
        trigger.Definition
        "GO"; "SET QUOTED_IDENTIFIER OFF "; "GO"; "SET ANSI_NULLS OFF "; "GO"; ""; 
        if trigger.IsDisabled
        then $"DISABLE TRIGGER [{trigger.Object.Schema.Name}].[{trigger.Object.Name}] ON [{trigger.Parent.Schema.Name}].[{trigger.Parent.Name}]"
        else $"ENABLE TRIGGER [{trigger.Object.Schema.Name}].[{trigger.Object.Name}] ON [{trigger.Parent.Schema.Name}].[{trigger.Parent.Name}]"
        "GO"; ""; "GO"
    ]
    |> List.iter w.WriteLine

    XProperties.trigger w.WriteLine trigger 

    ObjectDefinitions {| Contains = [trigger.Object.ObjectId]; DependsOn = [trigger.Parent.ObjectId] |}

let generateProcedureScript (w : System.IO.StreamWriter) (opt : Options) (p : Procedure) =
    [
        "SET QUOTED_IDENTIFIER ON "; "GO"; "SET ANSI_NULLS ON "; "GO"
        p.Definition
        "GO"; "SET QUOTED_IDENTIFIER OFF "; "GO"; "SET ANSI_NULLS OFF "; "GO"; ""; "GO"
    ]
    |> List.iter w.WriteLine

    XProperties.procedure w.WriteLine p

    ObjectDefinitions {| Contains = [p.Object.ObjectId]; DependsOn = [] |}

let generateSynonymScript (w : System.IO.StreamWriter) (opt : Options) (synonym : Synonym) =
    w.WriteLine $"CREATE SYNONYM [{synonym.Object.Schema.Name}].[{synonym.Object.Name}] FOR {synonym.BaseObjectName}"
    w.WriteLine "GO"
    ObjectDefinitions {| Contains = [synonym.Object.ObjectId]; DependsOn = [] |}


let generateXmlSchemaCollectionScript (w : System.IO.StreamWriter) (opt : Options) (s : XmlSchemaCollection) =
    let name = $"[{s.Schema.Name}].[{s.Name}]"
    w.WriteLine $"CREATE XML SCHEMA COLLECTION {name} AS N'{s.Definition}'"
    w.WriteLine "GO"
    
    XProperties.xmlSchemaCollection w.WriteLine s

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

let generateSequenceScript (w : System.IO.StreamWriter) (opt : Options) (s : Sequence) =
    let name = $"[{s.Object.Schema.Name}].[{s.Object.Name}]"
    let typeStr = Datatype.typeStr s.Datatype
    let startWith = match s.SequenceDefinition.StartValue with Some v -> $" START WITH {v}" | None -> ""
    let incrementBy = $" INCREMENT BY {s.SequenceDefinition.Increment}"
    let minValue = match s.SequenceDefinition.MinimumValue with Some v -> $" MINVALUE {v}" | None -> " NO MINVALUE"
    let maxValue = match s.SequenceDefinition.MaximumValue with Some v -> $" MAXVALUE {v}" | None -> " NO MAXVALUE"
    let cycle = if s.IsCycling then " CYCLE" else " NO CYCLE"
    let cache = match s.CacheSize with Some s -> $" CACHE {s}" | None -> if s.IsCached then "" else " NO CACHE"
    w.WriteLine $"CREATE SEQUENCE {name} AS {typeStr}{startWith}{incrementBy}{minValue}{maxValue}{cycle}{cache}"

    ObjectDefinitions {| Contains = [s.Object.ObjectId]; DependsOn = [] |}

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

let generateUserDefinedTypeScript (types : Map<int, Datatype>) (w : System.IO.StreamWriter) (opt : Options) (t : Datatype) =
    let tDef =
        let baseType = types |> Map.find (int t.SystemTypeId)
        Datatype.typeStr' baseType.Name baseType.DatatypeSpec t.Parameter
    let nullStr = if t.Parameter.IsNullable then "NULL" else "NOT NULL"
    w.WriteLine $"CREATE TYPE [{t.Schema.Name}].[{t.Name}] FROM {tDef} {nullStr}"
    w.WriteLine "GO"

    XProperties.typeProps w.WriteLine t.Schema t.Name t.XProperties

    UserDefinedTypeDefinition


let generateScripts (opt : Options) (schema : DatabaseSchema) scriptConsumer =
    let db = schema
    let dataForFolder subfolderName (nameFn : 'a -> string) f (xs : 'a list) =
        if xs |> List.isEmpty |> not 
        then
            for x in xs do
                let (isDatabaseDefinition, script) =
                    use ms = new System.IO.MemoryStream()
                    let scriptObjects =
                        use w = new System.IO.StreamWriter(ms)
                        f w opt x
                    let (isDatabaseDefinition, priority, containsObjects, dependsOn) =
                        match scriptObjects with
                        | DatabaseDefinition -> true, -1, [],[]
                        | SchemaDefinition -> false, 1, [],[]
                        | UserDefinedTypeDefinition -> false, 2,  [],[]
                        | ObjectDefinitions x -> false, 3, x.Contains, x.DependsOn
                        | XmlSchemaCollectionDefinition -> false, 4, [], []
                        
                    isDatabaseDefinition,
                    {
                        Contains = Set.ofList containsObjects
                        DependsOn = Set.ofList dependsOn
                        Priority = priority
                        
                        Content  = { 
                            Subdirectory = match subfolderName with "" -> None | s -> Some s; 
                            Filename = nameFn x; 
                            Content = ms.ToArray () |> System.Text.Encoding.UTF8.GetString
                        }
                    }
                scriptConsumer isDatabaseDefinition script
    
    let allTypes = 
        db.Types
        |> List.map (fun t -> t.UserTypeId, t)
        |> Map.ofList
    
    [db]
    |> dataForFolder "" (fun s -> $"props.sql") generateSettingsScript

    db.Schemas |> List.filter (fun s -> not s.IsSystemSchema)
    |> dataForFolder "schemas" (fun s -> $"{s.Name}.sql") generateSchemaScript

    db.Types
    |> List.filter (fun t -> match t.DatatypeSpec with UserDefined -> true | _ -> false)
    |> dataForFolder "user_defined_types" (fun t -> objectFilename t.Schema.Name t.Name) 
        (generateUserDefinedTypeScript allTypes)

    db.Types
    |> List.choose (fun t -> match t.DatatypeSpec with TableType o -> Some (t, Map.find o.ObjectId db.TableTypes) | _ -> None)
    |> dataForFolder "table_types" (fun (ty, tt) -> $"TYPE_{tt.Name}.sql") 
        (generateTableTypeScript allTypes db.Properties)

    db.Tables 
    |> dataForFolder "tables" (fun t -> objectFilename t.Schema.Name t.Name) 
        (generateTableScript allTypes db.Properties)
    
    db.Views
    |> dataForFolder "views" (fun v -> objectFilename v.Schema.Name v.Name) generateViewScript
    db.Views 
    |> List.collect 
        (fun v -> 
            v.Indexes 
            |> Array.choose 
                (fun i -> 
                    match i.Name, getIndexDefinitionStr opt $"[{v.Schema.Name}].[{v.Name}]" true i with 
                    | Some n, Some def -> Some (n, v, i, def) 
                    | _ -> None) 
            |> Array.toList) 
    |> dataForFolder "views" (fun (name,_view,_index,_def) -> $"{name}.sql") 
        (fun w _o (_name,view,index,def)-> 
            w.WriteLine def
            w.WriteLine "GO"
            
            view.Indexes
            |> Array.iter (XProperties.index w.WriteLine "N'VIEW'")

            ObjectDefinitions 
                {| 
                    Contains = match index.Object with Some o -> [o.ObjectId] | None -> []; 
                    DependsOn = [view.Object.ObjectId] 
                |})
    
    db.Procedures |> List.filter  (fun p -> p.Object.ObjectType <> ObjectType.SqlStoredProcedure)
    |> dataForFolder "functions" (fun p -> objectFilename p.Object.Schema.Name p.Name) generateProcedureScript

    db.Tables 
    |> List.choose 
        (fun t -> 
            match t.CheckConstraints |> Array.filter (fun cc -> not cc.IsSystemNamed) with 
            | [||] -> None 
            | ccs -> Some (t.Schema.Name, t.Name, t.Object.ObjectId, ccs))
    |> dataForFolder "check_constraints" (fun (sn, tn, _, _) -> objectFilename sn tn) generateCheckConstraintsScript


    db.Tables |> List.choose (fun t -> match t.DefaultConstraints with [||] -> None | dcs -> Some (t, dcs))
    |> dataForFolder "defaults" (fun (t, _) -> objectFilename t.Schema.Name t.Name) generateDefaultConstraintsScript

    db.Tables 
    |> List.collect (fun t -> t.Triggers |> Array.toList)
    |> dataForFolder "triggers" (fun tr -> objectFilename tr.Object.Schema.Name tr.Name) generateTriggerScript

    db.Tables |> List.choose (fun t -> match t.ForeignKeys with [||] -> None | fks -> Some (t, fks))
    |> dataForFolder "foreign_keys" (fun (t, _) -> objectFilename t.Schema.Name t.Name) generateForeignKeysScript

    db.Procedures |> List.filter  (fun p -> p.Object.ObjectType = ObjectType.SqlStoredProcedure)
    |> dataForFolder "procedures" (fun p -> objectFilename p.Object.Schema.Name p.Name) generateProcedureScript

    db.Synonyms
    |> dataForFolder "synonyms" (fun s -> objectFilename s.Object.Schema.Name s.Object.Name) generateSynonymScript

    db.XmlSchemaCollections
    |> dataForFolder "xmlschemacollections" (fun s -> objectFilename s.Schema.Name s.Name) generateXmlSchemaCollectionScript

    db.Sequences
    |> dataForFolder "sequences" (fun s -> objectFilename s.Object.Schema.Name s.Object.Name) generateSequenceScript    

