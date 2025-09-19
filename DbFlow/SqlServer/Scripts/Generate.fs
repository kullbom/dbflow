module DbFlow.SqlServer.Scripts.Generate

open DbFlow
open DbFlow.SqlServer.Schema

type ScriptContent = ScriptContent of string list

/// Creates a new ScriptContent with a new line appended
let (|>+) (ScriptContent lines) line = ScriptContent (line :: lines)

module ScriptContent =
    let empty = ScriptContent []
    let single s = ScriptContent [s]

    let addLine line (ScriptContent lines) = ScriptContent (line :: lines)

    /// Scripts added (by {f}) are surrounded by the {before}- and {after}-clauses. If nothing is added by {f} nothing is surrounded. 
    let surround before after f sc =
        let sc' = before |> List.fold (|>+) sc
        let sc'' = f sc'
        if sc' = sc''
        then sc
        else after |> List.fold (|>+) sc''

    // ScriptContent should be some more suitable data structure ...?
    let toString (ScriptContent lines) =
        let sb = System.Text.StringBuilder ()
        List.foldBack (fun (s : string) () -> sb.AppendLine(s) |> ignore) lines ()
        sb.ToString ()
        
type Script = { 
    Subdirectory : string option 
    Filename : string
    Content : ScriptContent 
}

type SchemaScriptPart =
    | DatabaseDefinition of ScriptContent
    | SchemaDefinition of ScriptContent
    | UserDefinedTypeDefinition of ScriptContent
    | ObjectDefinitions of {| Contains : int list; DependsOn : int list; Script : ScriptContent |}
    | XmlSchemaCollectionDefinition of ScriptContent

module XProperties =
    let xPropStr (propName : string) (propValue : string) keysAndValues =
        let escapedPropValue = propValue.Replace("'", "''")
        match keysAndValues with
        | [key0, value0] ->
            $"EXECUTE [sys].[sp_addextendedproperty] N'{propName}', N'{escapedPropValue}', {key0}, {value0};"
        | [key0, value0; key1, value1] ->
            $"EXECUTE [sys].[sp_addextendedproperty] N'{propName}', N'{escapedPropValue}', {key0}, {value0}, {key1}, {value1};"
        | [key0, value0; key1, value1; key2, value2] ->
            $"EXECUTE [sys].[sp_addextendedproperty] N'{propName}', N'{escapedPropValue}', {key0}, {value0}, {key1}, {value1}, {key2}, {value2};"
        
        | _ -> failwithf "Can not generate documentation script for %A" keysAndValues

    let xProps (xProperties : Map<string, string>) keysAndValues f seed =
        xProperties
        |> Map.fold (fun acc k v -> f acc (xPropStr k v keysAndValues)) seed
    
    let collectXProps (xProperties : Map<string, string>) keysAndValues sc =
        xProps xProperties keysAndValues 
            (fun (isFirst, sc') xp -> 
                false, ScriptContent.addLine xp (if isFirst then ScriptContent.addLine "" sc' else sc'))
            (true, sc)
        |> snd
    
    let database xProperties = collectXProps xProperties ["NULL", "NULL";]
    
    let xmlSchemaCollection (xmlSchema : XmlSchemaCollection) =
        collectXProps xmlSchema.XProperties 
            ["N'SCHEMA'", $"[{xmlSchema.Schema.Name}]"; "N'XML SCHEMA COLLECTION'", $"[{xmlSchema.Name}]";]

    let schema (s : Schema) = collectXProps s.XProperties ["N'SCHEMA'", $"[{s.Name}]";]
    
    let procedure (p : Procedure) =
        let pType =
            match p.Object.ObjectType with
            | ObjectType.SqlScalarFunction 
            | ObjectType.SqlInlineTableValuedFunction
            | ObjectType.SqlTableValuedFunction  -> "FUNCTION"
            | ObjectType.SqlStoredProcedure -> "PROCEDURE"
            | t -> failwithf "Unsupported object type %A" t
             
        ScriptContent.surround [""] []
            (fun sc' ->
                p.Parameters
                |> Array.fold 
                    (fun acc para ->
                        collectXProps para.XProperties
                            ["N'SCHEMA'", $"[{p.Object.Schema.Name}]"; $"N'{pType}'", $"[{p.Name}]"; "N'PARAMETER'", $"'{para.Name}'"]
                            acc)
                    (collectXProps p.XProperties 
                        ["N'SCHEMA'", $"[{p.Object.Schema.Name}]"; $"N'{pType}'", $"[{p.Name}]"]
                        sc'))

    let constraint' (parent : OBJECT) constraintName xProperties sc =
        sc
        |> ScriptContent.surround [] ["GO"]
            (collectXProps xProperties 
                ["N'SCHEMA'", $"[{parent.Schema.Name}]"; "N'TABLE'", $"[{parent.Name}]"; "N'CONSTRAINT'", $"[{constraintName}]"])
                
            
    let trigger (tr : Trigger) =
        collectXProps tr.XProperties
            ["N'SCHEMA'", $"[{tr.Parent.Schema.Name}]"; "N'TABLE'", $"[{tr.Parent.Name}]"; "N'TRIGGER'", $"[{tr.Name}]"]
    
        
    let index cType (i : Index) sc =
        match i.Name with
        | Some iName ->
            collectXProps i.XProperties 
                ["N'SCHEMA'", $"[{i.Parent.Schema.Name}]"; cType, $"[{i.Parent.Name}]"; "N'INDEX'", $"[{iName}]"]
                sc
        | None -> sc
    
    let containerAndColumns schemaName cType cName cXProperties (columns : Column array) sc =
        columns
        |> Array.fold 
            (fun acc c ->
                collectXProps c.XProperties
                    ["N'SCHEMA'", $"[{schemaName}]"; cType, $"[{cName}]"; "N'COLUMN'", $"[{c.Name}]"]
                    acc)
            (collectXProps cXProperties
                ["N'SCHEMA'", $"[{schemaName}]"; cType, $"[{cName}]";]
                sc)

    let table (t : Table) sc =
        sc
        |> containerAndColumns t.Schema.Name "N'TABLE'" t.Name t.XProperties t.Columns
        |> (fun sc' -> 
            t.Indexes
            |> Array.fold (fun sc'' i -> index "N'TABLE'" i sc'') sc')

    let viewAndColumns (v : View) =
        containerAndColumns v.Schema.Name "N'VIEW'" v.Name v.XProperties v.Columns

    let typeProps (s : Schema) (typeName : string) (xProps : Map<string, string>) =
        collectXProps xProps  ["N'SCHEMA'", $"[{s.Name}]"; "N'TYPE'", $"[{typeName}]";]

        
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
    
let generateSettingsScript (opt : Options) (schema : DatabaseSchema) =
    let s = schema.Properties
    let onOff isSet = if isSet then "ON" else "OFF"
    let recoveryModel = match s.recovery_model with 1uy -> "FULL" | 2uy -> "BULK_LOGGED" | 3uy -> "SIMPLE" | rm -> failwithf "Unknown recovery model %i" rm
    let parameterization = if s.is_parameterization_forced then "FORCED" else "SIMPLE"
    let cursorDefault = if s.is_local_cursor_default then "LOCAL" else "GLOBAL"
    let allowSnapshotIsolation = match s.snapshot_isolation_state with 0uy | 2uy -> "OFF" | 1uy | 3uy -> "ON" | sis -> failwithf "Unknown snapshot isolation state %i" sis
    let pageVerify = match s.page_verify_option with 0uy -> "NONE" | 1uy -> "TORN_PAGE_DETECTION" | 2uy -> "CHECKSUM" | sis -> failwithf "Unknown page verify option %i" sis
    
    ScriptContent.empty
    |>+ "DECLARE @DB VARCHAR(255)"
    |>+ "SET @DB = DB_NAME()"
    |>+ $"EXEC dbo.sp_dbcmptlevel @DB, {s.compatibility_level}"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] COLLATE {s.collation_name}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_CLOSE {onOff s.is_auto_close_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_SHRINK {onOff s.is_auto_shrink_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET ALLOW_SNAPSHOT_ISOLATION {allowSnapshotIsolation}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET READ_COMMITTED_SNAPSHOT {onOff s.is_read_committed_snapshot_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET RECOVERY {recoveryModel}')" 
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET PAGE_VERIFY {pageVerify}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_CREATE_STATISTICS {onOff s.is_auto_create_stats_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_UPDATE_STATISTICS {onOff s.is_auto_update_stats_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET AUTO_UPDATE_STATISTICS_ASYNC {onOff s.is_auto_update_stats_async_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_NULL_DEFAULT {onOff s.is_ansi_null_default_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_NULLS {onOff s.is_ansi_nulls_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_PADDING {onOff s.is_ansi_padding_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET ANSI_WARNINGS {onOff s.is_ansi_warnings_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET ARITHABORT {onOff s.is_arithabort_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET CONCAT_NULL_YIELDS_NULL {onOff s.is_concat_null_yields_null_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET NUMERIC_ROUNDABORT {onOff s.is_numeric_roundabort_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET QUOTED_IDENTIFIER {onOff s.is_quoted_identifier_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET RECURSIVE_TRIGGERS {onOff s.is_recursive_triggers_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET CURSOR_CLOSE_ON_COMMIT {onOff s.is_cursor_close_on_commit_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET CURSOR_DEFAULT {cursorDefault}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET TRUSTWORTHY {onOff s.is_trustworthy_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET DB_CHAINING {onOff s.is_db_chaining_on}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET PARAMETERIZATION {parameterization}')"
    |>+ $"EXEC('ALTER DATABASE [' + @DB + '] SET DATE_CORRELATION_OPTIMIZATION {onOff s.is_date_correlation_on}')"
    |>+ "GO"
    
    |> XProperties.database schema.XProperties

    |> DatabaseDefinition


let generateSchemaScript (opt : Options) (schema : Schema) =
    ScriptContent.empty
    |>+ $"CREATE SCHEMA [{schema.Name}] AUTHORIZATION [{schema.PrincipalName}]"
    |>+ "GO"

    |> XProperties.schema schema

    |> SchemaDefinition

let commaSeparated (indentionStr : string) xs (formatter : _ -> string) sc =
    let n = xs |> Array.length
    xs 
    |> Array.fold
        (fun (i, sc') x -> 
            let commaStr = if i = n - 1 then "" else ","
            i + 1, sc' |>+ $"{indentionStr}{formatter x}{commaStr}")
        (0, sc)
    |> snd

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
    
    match index.XmlIndexSecondaryType with
    | None -> $"CREATE PRIMARY XML INDEX [{indexName}] ON {parentName} ({keyColumnsStr})" 
    | Some sType -> 
        let indexScript = $"CREATE XML INDEX [{indexName}] ON {parentName} ({keyColumnsStr})"
        $"{indexScript}\r\nUSING XML INDEX [{sType.PrimaryIndexName}] FOR {sType.SecondaryType};"
        

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

let addTableBody (opt : Options) ds allTypes isTableType columns tableInlineIndexes tableInlineChecks columnInlineDefaults sc =
    commaSeparated "   " columns (columnDefinitionStr opt ds allTypes isTableType columnInlineDefaults) sc
    |> ScriptContent.surround [""] []
        (fun sc' -> 
            tableInlineIndexes
            |> List.fold
                (fun sc'' (inlineIndex : Index) ->
                    match inlineIndex.IsSystemNamed, inlineIndex.IsPrimaryKey, inlineIndex.IsUniqueConstraint with 
                    | _, true, false -> sc'' |>+ $"   ,{primaryKeyStr opt isTableType inlineIndex}"
                    | _, false, true -> sc'' |>+ $"   ,{uniqueKeyStr opt inlineIndex}"
                    | _ -> failwithf "Unknown inline index %A" inlineIndex)
                (tableInlineChecks
                 |> List.fold 
                    (fun sc'' (inlineCheck : CheckConstraint) ->
                        if inlineCheck.IsSystemNamed
                        then sc'' |>+ $"   ,CHECK {inlineCheck.Definition}"
                        else sc'' |>+ $"   ,CONSTRAINT [{inlineCheck.Object.Name}] CHECK ({inlineCheck.Definition})")
                    sc'))

                
let generateTableScript' (opt : Options) ds allTypes isTableType parentName columns indexes checkConstraints (defaultConstraints : DefaultConstraint array) sc =
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

    sc
    |> addTableBody opt ds allTypes isTableType columns tableInlineIndexes tableInlineChecks columnInlineDefaults
    |>+ $")"

    |>+ ""
    |> fun sc' -> 
        standaloneIndexes |> List.sortBy (fun i -> i.IndexId) 
        |> List.fold 
            (fun sc'' index -> 
                let indexStr = getIndexDefinitionStr opt parentName false index
                match indexStr with
                | Some s -> 
                    sc'' 
                    |>+ s
                    |> match index.IsDisabled, index.Name with
                        | true, Some iName -> 
                            ScriptContent.addLine $"ALTER INDEX {iName} ON {parentName} DISABLE"
                        | _ -> id
                | None -> sc'')
            sc'
    |>+ ""
    |>+ "GO"
    |> fun sc' ->
        sc',
        []
        |> (fun acc -> columns |> Array.fold (fun acc' c -> c.Object.ObjectId :: acc') acc)
        |> (fun acc -> indexes |> Array.fold (fun acc' i -> match i.Object with Some o -> o.ObjectId :: acc' | None -> acc') acc)
        |> (fun acc -> columnInlineDefaults |> Map.fold (fun acc' _ dc -> dc.Object.ObjectId :: acc') acc)
    

let generateTableScript allTypes ds (opt : Options) (t : Table) =
    let tableName = Table.fullName t
    ScriptContent.empty
    |>+ $"CREATE TABLE {tableName} ("
    |> generateTableScript' opt ds allTypes false tableName 
            t.Columns t.Indexes t.CheckConstraints t.DefaultConstraints
    |> (fun (sc, objectIds) ->
        let sc' = sc |> XProperties.table t

        ObjectDefinitions {| Contains = t.Object.ObjectId :: objectIds; DependsOn = []; Script = sc'|})

let generateTableTypeScript allTypes ds (opt : Options) (ty : Datatype, tt : TableType) =
    let tName = $"[{tt.Schema.Name}].[{tt.Name}]"
    ScriptContent.empty
    |>+ $"CREATE TYPE {tName} AS TABLE ("
    |> generateTableScript' opt ds allTypes true tName 
            tt.Columns tt.Indexes tt.CheckConstraints tt.DefaultConstraints
    |> fun (sc, objectIds) ->
        let sc' = XProperties.typeProps tt.Schema tt.Name ty.XProperties sc

        ObjectDefinitions {| Contains = tt.Object.ObjectId :: objectIds; DependsOn = []; Script = sc' |}


let generateViewScript (opt : Options) (view : View)=
    ScriptContent.empty
    |>+ "SET QUOTED_IDENTIFIER ON"
    |>+ "GO"
    |>+ "SET ANSI_NULLS ON"
    |>+ "GO"
    |>+ view.Definition
    |>+ "GO"
    |>+ "SET QUOTED_IDENTIFIER OFF"
    |>+ "GO"
    |>+ "SET ANSI_NULLS OFF"
    |>+ "GO"
    
    |> XProperties.viewAndColumns view

    |> fun sc -> ObjectDefinitions {| Contains = [view.Object.ObjectId]; DependsOn = []; Script = sc |}


let generateCheckConstraintsScript (opt : Options) (table : Table, table_object_id : int, ccs : CheckConstraint array) =
    ccs
    |> Array.fold 
        (fun (sc', objectIds) cc ->
            let tableFullname = Table.fullName table
            if cc.IsSystemNamed
            then sc' |>+ $"ALTER TABLE {tableFullname} WITH CHECK ADD CHECK {cc.Definition}"
            else 
                let trustClause = if cc.IsNotTrusted then "NOCHECK" else "CHECK" 
                sc' 
                |>+ $"ALTER TABLE {tableFullname} WITH {trustClause} ADD CONSTRAINT [{cc.Object.Name}] CHECK {cc.Definition}"
                |> (if cc.IsDisabled
                    then ScriptContent.addLine $"ALTER TABLE {tableFullname} NOCHECK CONSTRAINT [{cc.Object.Name}]"
                    else id)
            |>+ "GO"
            
            |> XProperties.constraint' table.Object cc.Object.Name cc.XProperties,

            cc.Object.ObjectId :: objectIds)
        (ScriptContent.empty, [])
    |> fun (sc', objectIds) -> 
        ObjectDefinitions {| Contains = objectIds; DependsOn = [table_object_id]; Script = sc' |}

let generateDefaultConstraintsScript (opt : Options) (table : Table, dcs : DefaultConstraint array) =
    dcs 
    |> Array.sortBy (fun dc -> dc.Object.ObjectId)
    |> Array.fold
        (fun (sc, objectIds) dc ->
            let tableName = Table.fullName table
            sc
            |>+ 
                (if dc.IsSystemNamed
                 then $"ALTER TABLE {tableName} ADD DEFAULT {dc.Definition} FOR [{dc.Column.Name}]"
                 else $"ALTER TABLE {tableName} ADD CONSTRAINT [{dc.Object.Name}] DEFAULT {dc.Definition} FOR [{dc.Column.Name}]")
            |>+ "GO"

            |> XProperties.constraint' table.Object dc.Object.Name dc.XProperties,
            
            dc.Object.ObjectId :: objectIds)
        (ScriptContent.empty, [])
    |> fun (sc, objectIds) -> 
        ObjectDefinitions {| Contains = objectIds; DependsOn = [table.Object.ObjectId]; Script = sc |}

let generateForeignKeysScript (opt : Options) (table : Table, fks : ForeignKey array) =
    let (sc, object_ids, depends_on) =
        fks
        |> Array.fold 
            (fun (sc, object_ids, depends_on) fk ->
                let columnsStr = fk.Columns |> Array.joinBy ", " (fun c -> $"[{c.ParentColumn.Name}]")
                let refColumnsStr = fk.Columns |> Array.joinBy ", " (fun c -> $"[{c.ReferencedColumn.Name}]")
                let name = if fk.IsSystemNamed then "" else $"CONSTRAINT [{fk.Name}]"
                
                sc
                |>+ $"ALTER TABLE [{table.Schema.Name}].[{table.Name}] WITH CHECK ADD {name}"
                |>+ $"   FOREIGN KEY({columnsStr}) REFERENCES [{fk.Referenced.Schema.Name}].[{fk.Referenced.Name}] ({refColumnsStr})"
                |> match fk.UpdateReferentialAction with
                    | ReferentialAction.NoAction -> id
                    | ReferentialAction.Cascade -> ScriptContent.addLine "   ON UPDATE CASCADE"
                    | ReferentialAction.SetNull -> ScriptContent.addLine "   ON UPDATE SET NULL"
                    | ReferentialAction.SetDefault -> ScriptContent.addLine "   ON UPDATE SET DEFAULT"
                |> match fk.DeleteReferentialAction with
                    | ReferentialAction.NoAction -> id 
                    | ReferentialAction.Cascade -> ScriptContent.addLine "   ON DELETE CASCADE"
                    | ReferentialAction.SetNull -> ScriptContent.addLine "   ON DELETE SET NULL"
                    | ReferentialAction.SetDefault -> ScriptContent.addLine "   ON DELETE SET DEFAULT"
                
                |> if fk.IsDisabled 
                   then ScriptContent.addLine $"ALTER TABLE [{table.Schema.Name}].[{table.Name}] NOCHECK {name}"
                   else id
                |>+ ""
                |>+ "GO"

                |> XProperties.constraint' table.Object fk.Object.Name fk.XProperties,

                fk.Object.ObjectId :: object_ids,
                fk.Parent.ObjectId :: fk.Referenced.ObjectId :: depends_on)
            (ScriptContent.empty, [], [])
    ObjectDefinitions {| Contains = object_ids; DependsOn = depends_on; Script = sc |}

let generateTriggerScript (opt : Options) (tr : Trigger) =
    ScriptContent.empty
    |>+ "SET QUOTED_IDENTIFIER ON "
    |>+ "GO"
    |>+ "SET ANSI_NULLS ON "
    |>+ "GO"
    |>+ tr.Definition
    |>+ "GO"
    |>+ "SET QUOTED_IDENTIFIER OFF "
    |>+ "GO"
    |>+ "SET ANSI_NULLS OFF "
    |>+ "GO"
    |>+ "" 
    |>+
        (if tr.IsDisabled
         then $"DISABLE TRIGGER [{tr.Object.Schema.Name}].[{tr.Object.Name}] ON [{tr.Parent.Schema.Name}].[{tr.Parent.Name}]"
         else $"ENABLE TRIGGER [{tr.Object.Schema.Name}].[{tr.Object.Name}] ON [{tr.Parent.Schema.Name}].[{tr.Parent.Name}]")
    |>+ "GO"
    |> XProperties.trigger tr
    |> fun sc ->
        ObjectDefinitions {| Contains = [tr.Object.ObjectId]; DependsOn = [tr.Parent.ObjectId]; Script = sc |}
    

let generateProcedureScript (opt : Options) (p : Procedure) =
    ScriptContent.empty
    |>+ "SET QUOTED_IDENTIFIER ON "
    |>+ "GO"
    |>+ "SET ANSI_NULLS ON "
    |>+ "GO"
    |>+ p.Definition
    |>+ "GO"
    |>+ "SET QUOTED_IDENTIFIER OFF "
    |>+ "GO"
    |>+ "SET ANSI_NULLS OFF "
    |>+ "GO"

    |> XProperties.procedure p

    |> fun sc -> ObjectDefinitions {| Contains = [p.Object.ObjectId]; DependsOn = []; Script = sc |}

let generateSynonymScript (opt : Options) (synonym : Synonym) =
    ScriptContent.empty
    |>+ $"CREATE SYNONYM [{synonym.Object.Schema.Name}].[{synonym.Object.Name}] FOR {synonym.BaseObjectName}"
    |>+ "GO"
    |> fun sc -> ObjectDefinitions {| Contains = [synonym.Object.ObjectId]; DependsOn = []; Script = sc |}


let generateXmlSchemaCollectionScript (opt : Options) (s : XmlSchemaCollection) =
    let name = $"[{s.Schema.Name}].[{s.Name}]"
    ScriptContent.empty
    |>+ $"CREATE XML SCHEMA COLLECTION {name} AS N'{s.Definition}'"
    |>+ "GO"
    
    |> XProperties.xmlSchemaCollection s

    |> XmlSchemaCollectionDefinition 

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

let generateSequenceScript (opt : Options) (s : Sequence) =
    let name = $"[{s.Object.Schema.Name}].[{s.Object.Name}]"
    let typeStr = Datatype.typeStr s.Datatype
    let startWith = match s.SequenceDefinition.StartValue with Some v -> $" START WITH {v}" | None -> ""
    let incrementBy = $" INCREMENT BY {s.SequenceDefinition.Increment}"
    let minValue = match s.SequenceDefinition.MinimumValue with Some v -> $" MINVALUE {v}" | None -> " NO MINVALUE"
    let maxValue = match s.SequenceDefinition.MaximumValue with Some v -> $" MAXVALUE {v}" | None -> " NO MAXVALUE"
    let cycle = if s.IsCycling then " CYCLE" else " NO CYCLE"
    let cache = match s.CacheSize with Some s -> $" CACHE {s}" | None -> if s.IsCached then "" else " NO CACHE"
    ScriptContent.empty
    |>+ $"CREATE SEQUENCE {name} AS {typeStr}{startWith}{incrementBy}{minValue}{maxValue}{cycle}{cache}"
    |> fun sc -> 
        ObjectDefinitions {| Contains = [s.Object.ObjectId]; DependsOn = []; Script = sc |}

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

let generateUserDefinedTypeScript (types : Map<int, Datatype>) (opt : Options) (t : Datatype) =
    let tDef =
        let baseType = types |> Map.find (int t.SystemTypeId)
        Datatype.typeStr' baseType.Name baseType.DatatypeSpec t.Parameter
    let nullStr = if t.Parameter.IsNullable then "NULL" else "NOT NULL"
    
    ScriptContent.empty
    |>+ $"CREATE TYPE [{t.Schema.Name}].[{t.Name}] FROM {tDef} {nullStr}"
    |>+ "GO"

    |> XProperties.typeProps t.Schema t.Name t.XProperties

    |> UserDefinedTypeDefinition


let generateScripts (opt : Options) (schema : DatabaseSchema) f seed =
    let db = schema
    let dataForFolder subfolderName (xs : 'a list) (nameFn : 'a -> string) generator acc =
        match xs with
        | [] -> acc
        | _ -> 
            xs 
            |> List.fold 
                (fun acc' x -> 
                    let (isDatabaseDefinition, script) =
                        let (isDatabaseDefinition, priority, containsObjects, dependsOn, script) =
                            match generator opt x with
                            | DatabaseDefinition script -> true, -1, [], [], script
                            | SchemaDefinition script -> false, 1, [],[], script
                            | UserDefinedTypeDefinition script -> false, 2,  [],[], script
                            | ObjectDefinitions x -> false, 3, x.Contains, x.DependsOn, x.Script
                            | XmlSchemaCollectionDefinition script -> false, 4, [], [], script
                        isDatabaseDefinition,
                        {
                            Contains = Set.ofList containsObjects
                            DependsOn = Set.ofList dependsOn
                            Priority = priority
                            
                            Content  = { 
                                Subdirectory = match subfolderName with "" -> None | s -> Some s; 
                                Filename = nameFn x; 
                                Content = script
                            }
                        }
                    f acc' isDatabaseDefinition script)
                acc
    
    let allTypes = 
        db.Types
        |> List.map (fun t -> t.UserTypeId, t)
        |> Map.ofList
    
    seed    
    |> dataForFolder "" [db] (fun s -> $"props.sql") generateSettingsScript  

    |> dataForFolder "schemas" 
        (db.Schemas |> List.filter (fun s -> not s.IsSystemSchema))
        (fun s -> $"{s.Name}.sql") generateSchemaScript

    |> dataForFolder "user_defined_types"  
        (db.Types |> List.filter (fun t -> match t.DatatypeSpec with UserDefined -> true | _ -> false))
        (fun t -> objectFilename t.Schema.Name t.Name)
        (generateUserDefinedTypeScript allTypes)

    |> dataForFolder "table_types" 
        (db.Types |> List.choose (fun t -> match t.DatatypeSpec with TableType o -> Some (t, Map.find o.ObjectId db.TableTypes) | _ -> None))
        (fun (ty, tt) -> $"TYPE_{tt.Name}.sql") 
        (generateTableTypeScript allTypes db.Properties)

    |> dataForFolder "tables" 
        db.Tables
        (fun t -> objectFilename t.Schema.Name t.Name) 
        (generateTableScript allTypes db.Properties)
    // Views
    |> dataForFolder "views" db.Views (fun v -> objectFilename v.Schema.Name v.Name) generateViewScript
    // View indexes
    |> dataForFolder "views" 
        (db.Views 
         |> List.collect 
            (fun v -> 
                v.Indexes 
                |> Array.choose 
                    (fun i -> 
                        match i.Name, getIndexDefinitionStr opt $"[{v.Schema.Name}].[{v.Name}]" true i with 
                        | Some n, Some def -> Some (n, v, i, def) 
                        | _ -> None) 
                |> Array.toList))
        (fun (name,_view,_index,_def) -> $"{name}.sql") 
        (fun _o (_name,view,index,def)-> 
            ScriptContent.empty
            |>+ def
            |>+ "GO"
            |> fun sc ->
                let sc' = 
                    view.Indexes
                    |> Array.fold (fun sc' i -> XProperties.index "N'VIEW'" i sc') sc

                ObjectDefinitions
                    {| 
                        Contains = match index.Object with Some o -> [o.ObjectId] | None -> []; 
                        DependsOn = [view.Object.ObjectId] 
                        Script = sc'
                    |})
    
    |> dataForFolder "functions" 
        (db.Procedures |> List.filter  (fun p -> p.Object.ObjectType <> ObjectType.SqlStoredProcedure))
        (fun p -> objectFilename p.Object.Schema.Name p.Name) generateProcedureScript

    |> dataForFolder "check_constraints" 
        (db.Tables 
        |> List.choose 
            (fun t -> 
                match t.CheckConstraints |> Array.filter (fun cc -> not cc.IsSystemNamed) with 
                | [||] -> None 
                | ccs -> Some (t, t.Object.ObjectId, ccs)))
        (fun (t, _, _) -> objectFilename t.Schema.Name t.Name) generateCheckConstraintsScript

    |> dataForFolder "defaults" 
        (db.Tables |> List.choose (fun t -> match t.DefaultConstraints with [||] -> None | dcs -> Some (t, dcs)))
        (fun (t, _) -> objectFilename t.Schema.Name t.Name) generateDefaultConstraintsScript

    |> dataForFolder "triggers" 
        (db.Tables |> List.collect (fun t -> t.Triggers |> Array.toList))
        (fun tr -> objectFilename tr.Object.Schema.Name tr.Name) generateTriggerScript

    |> dataForFolder "foreign_keys" 
        (db.Tables |> List.choose (fun t -> match t.ForeignKeys with [||] -> None | fks -> Some (t, fks)))
        (fun (t, _) -> objectFilename t.Schema.Name t.Name) generateForeignKeysScript

    |> dataForFolder "procedures" 
        (db.Procedures |> List.filter  (fun p -> p.Object.ObjectType = ObjectType.SqlStoredProcedure))
        (fun p -> objectFilename p.Object.Schema.Name p.Name) generateProcedureScript

    |> dataForFolder "synonyms" 
        db.Synonyms
        (fun s -> objectFilename s.Object.Schema.Name s.Object.Name) generateSynonymScript

    |> dataForFolder "xmlschemacollections" 
        db.XmlSchemaCollections
        (fun s -> objectFilename s.Schema.Name s.Name) generateXmlSchemaCollectionScript

    |> dataForFolder "sequences" 
        db.Sequences
        (fun s -> objectFilename s.Object.Schema.Name s.Object.Name) generateSequenceScript    

