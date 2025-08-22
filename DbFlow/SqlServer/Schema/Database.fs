namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-databases-transact-sql?view=sql-server-ver17

type DatabaseProperties = {
    compatibility_level : byte
    collation_name : string
    is_auto_close_on : bool
    is_auto_shrink_on : bool
    snapshot_isolation_state : byte // See documentation...
    is_read_committed_snapshot_on : bool
    recovery_model : byte // 1 = FULL, 2 = BULK_LOGGED, 3 = SIMPLE
    page_verify_option : byte // 0 = NONE, 1 = TORN_PAGE_DETECTION, 2 = CHECKSUM
    is_auto_create_stats_on : bool
    is_auto_update_stats_on : bool
    is_auto_update_stats_async_on : bool
    is_ansi_null_default_on : bool
    is_ansi_nulls_on : bool
    is_ansi_padding_on : bool
    is_ansi_warnings_on : bool
    is_arithabort_on : bool
    is_concat_null_yields_null_on : bool
    is_numeric_roundabort_on : bool
    is_quoted_identifier_on : bool
    is_recursive_triggers_on : bool
    is_cursor_close_on_commit_on : bool
    is_local_cursor_default : bool
    is_trustworthy_on : bool
    is_db_chaining_on : bool
    is_parameterization_forced : bool
    is_date_correlation_on : bool
}

module DatabaseProperties = 
    let readAll connection =
        DbTr.reader
            "SELECT
                [compatibility_level],
                [collation_name],
                [is_auto_close_on],
                [is_auto_shrink_on],
                [snapshot_isolation_state],
                [is_read_committed_snapshot_on],
                [recovery_model],
                [page_verify_option],
                [is_auto_create_stats_on],
                [is_auto_update_stats_on],
                [is_auto_update_stats_async_on],
                [is_ansi_null_default_on],
                [is_ansi_nulls_on],
                [is_ansi_padding_on],
                [is_ansi_warnings_on],
                [is_arithabort_on],
                [is_concat_null_yields_null_on],
                [is_numeric_roundabort_on],
                [is_quoted_identifier_on],
                [is_recursive_triggers_on],
                [is_cursor_close_on_commit_on],
                [is_local_cursor_default],
                [is_trustworthy_on],
                [is_db_chaining_on],
                [is_parameterization_forced],
                [is_date_correlation_on]
             FROM sys.databases
             WHERE name = DB_NAME()"
            []
            (fun acc r -> 
                {
                    compatibility_level = readByte "compatibility_level" r
                    collation_name = readString "collation_name" r
                    is_auto_close_on = readBool "is_auto_close_on" r
                    is_auto_shrink_on = readBool "is_auto_shrink_on" r
                    snapshot_isolation_state = readByte "snapshot_isolation_state" r
                    is_read_committed_snapshot_on = readBool "is_read_committed_snapshot_on" r
                    recovery_model = readByte "recovery_model" r
                    page_verify_option = readByte "page_verify_option" r
                    is_auto_create_stats_on = readBool "is_auto_create_stats_on" r
                    is_auto_update_stats_on = readBool "is_auto_update_stats_on" r
                    is_auto_update_stats_async_on = readBool "is_auto_update_stats_async_on" r
                    is_ansi_null_default_on = readBool "is_ansi_null_default_on" r
                    is_ansi_nulls_on = readBool "is_ansi_nulls_on" r
                    is_ansi_padding_on = readBool "is_ansi_padding_on" r
                    is_ansi_warnings_on = readBool "is_ansi_warnings_on" r
                    is_arithabort_on = readBool "is_arithabort_on" r
                    is_concat_null_yields_null_on = readBool "is_concat_null_yields_null_on" r
                    is_numeric_roundabort_on = readBool "is_numeric_roundabort_on" r
                    is_quoted_identifier_on = readBool "is_quoted_identifier_on" r
                    is_recursive_triggers_on = readBool "is_recursive_triggers_on" r
                    is_cursor_close_on_commit_on = readBool "is_cursor_close_on_commit_on" r
                    is_local_cursor_default = readBool "is_local_cursor_default" r
                    is_trustworthy_on = readBool "is_trustworthy_on" r
                    is_db_chaining_on = readBool "is_db_chaining_on" r
                    is_parameterization_forced = readBool "is_parameterization_forced" r
                    is_date_correlation_on = readBool "is_date_correlation_on" r
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> function
            | [settings] -> settings
            | _ -> failwithf "Multiple of no settings found - should not happen"
        

type DatabaseSchema = {
    Schemas : Schema list
    Tables : TABLE list
    Views : VIEW list

    Types : Datatype list
    TableTypes : TableType list

    Procedures : Procedure list

    XmlSchemaCollections : XmlSchemaCollection list

    Triggers : DatabaseTrigger list
    Synonyms : Synonym list
    Sequences : Sequence list
    
    Properties : DatabaseProperties

    MSDescription : string option

    //all_objects : OBJECT list
    Dependencies : Map<int, int list>
}

module DatabaseSchema =
    // Read all views in order to redefined them - to get fresh/correct meta data 
    let preReadAllViews logger connection =
        let ms_descs = RCMap.ofMap Map.empty
        let dependencies = Dependency.readAll |> Logger.logTime logger "Dependencies" connection
        
        let schemas = Schema.readAll ms_descs connection
        let objects = OBJECT.readAll schemas |> Logger.logTime logger "OBJECT" connection
        let types = Datatype.readAll schemas objects ms_descs connection

        let sql_modules = SqlModule.readAll connection
        
        let (columns, columnsByObject) = COLUMN.readAll objects types ms_descs |> Logger.logTime logger "COLUMN" connection
        let triggersByParent = Trigger.readAll objects sql_modules ms_descs |> Logger.logTime logger "TRIGGER" connection
        
        let indexesColumnsByIndex = INDEX_COLUMN.readAll objects columns |> Logger.logTime logger "INDEX_COLUMN" connection
        let indexesByParent = INDEX.readAll objects indexesColumnsByIndex ms_descs |> Logger.logTime logger "INDEX" connection
        
        let views = 
            VIEW.readAll schemas objects columnsByObject indexesByParent triggersByParent sql_modules ms_descs
            |> Logger.logTime logger "VIEW" connection
        
        views, dependencies

    let read logger (options : Options) connection =
        let ms_descs = MS_Description.readAll |> Logger.logTime logger "MS_Description" connection
        let dependencies = Dependency.readAll |> Logger.logTime logger "Dependencies" connection
        
        let dbProperties = DatabaseProperties.readAll connection

        let schemas = Schema.readAll ms_descs connection
        let objects = OBJECT.readAll schemas |> Logger.logTime logger "OBJECT" connection
        let types = Datatype.readAll schemas objects ms_descs connection

        let sequences = Sequence.readAll objects types connection
        let synonyms = Synonym.readAll objects connection
        let sql_modules = SqlModule.readAll connection
        let xml_schema_collections = XmlSchemaCollection.readAll schemas ms_descs connection
        
        let (columns, columnsByObject) = COLUMN.readAll objects types ms_descs |> Logger.logTime logger "COLUMN" connection
        let triggersByParent = Trigger.readAll objects sql_modules ms_descs |> Logger.logTime logger "TRIGGER" connection
        
        // A bit strange that keyConstraints isn't used...?!
        let keyConstraints = KeyConstraint.readAll objects ms_descs connection
        let checkConstraintsByParent = 
            CheckConstraint.readAll objects columns ms_descs
            |> Logger.logTime logger "CHECK_CONSTRAINT" connection
        let defaultConstraintsByParent = 
            DefaultConstraint.readAll objects columns ms_descs 
            |> Logger.logTime logger "DEFAULT_CONSTRAINT" connection
        

        let fkColsByConstraint = FOREIGN_KEY_COLUMN.readAll objects columns |> Logger.logTime logger "FOREIGN_KEY_COLUMN" connection 
        let (foreignKeysByParent, foreignKeysByReferenced)  = 
            FOREIGN_KEY.readAll objects fkColsByConstraint ms_descs 
             |> Logger.logTime logger "FOREIGN_KEY" connection
        
        let indexesColumnsByIndex = INDEX_COLUMN.readAll objects columns |> Logger.logTime logger "INDEX_COLUMN" connection
        let indexesByParent = INDEX.readAll objects indexesColumnsByIndex ms_descs |> Logger.logTime logger "INDEX" connection
        
        let tableTypes =
            TableType.readAll schemas objects ms_descs columnsByObject 
                indexesByParent foreignKeysByParent foreignKeysByReferenced checkConstraintsByParent defaultConstraintsByParent 
            |> Logger.logTime logger "TABLE_TYPE" connection
            

        let parametersByObject = PARAMETER.readAll objects types ms_descs connection
        let procedures = Procedure.readAll objects parametersByObject columnsByObject indexesByParent sql_modules ms_descs connection

        let tables = 
            TABLE.readAll 
                schemas objects columnsByObject indexesByParent
                triggersByParent foreignKeysByParent foreignKeysByReferenced 
                checkConstraintsByParent defaultConstraintsByParent ms_descs 
            |> Logger.logTime logger "TABLE" connection
        let views = 
            VIEW.readAll schemas objects columnsByObject indexesByParent triggersByParent sql_modules ms_descs
            |> Logger.logTime logger "VIEW" connection
        
        let db_msDesc = RCMap.tryPick (XPropertyClass.Database, 0, 0) ms_descs
        let db_triggers = Trigger.readAllDatabaseTriggers objects sql_modules ms_descs connection

        let checkUnused (id : string) exclude pm =
            match RCMap.unused exclude pm with
            | [] -> ()
            | unused -> failwith $"{id} not mapped for {unused}" 

        // Verify that all objects are "picked" (referenced) by something
        if not options.BypassReferenceChecksOnLoad
        then
            ms_descs |> checkUnused "ms_descriptions" (fun _ -> false)
            columnsByObject |> checkUnused "columns" (fun _ -> false)
            objects |> checkUnused "objects" (fun c -> c.ObjectType = ObjectType.ServiceQueue)
            triggersByParent |> checkUnused "triggers" (fun _ -> false)
            parametersByObject |> checkUnused "parameters" (fun _ -> false)
            checkConstraintsByParent |> checkUnused "check_constraints" (fun _ -> false)
            defaultConstraintsByParent |> checkUnused "default_constraints" (fun _ -> false)
            fkColsByConstraint |> checkUnused "foreign_key_columns" (fun _ -> false)
            foreignKeysByParent |> checkUnused "foreign_keys" (fun _ -> false)
            indexesColumnsByIndex |> checkUnused "index_columns" (fun _ -> false)
            indexesByParent 
            |> checkUnused "indexes" 
                (fun i -> 
                    match i[0].parent.ObjectType with 
                    | ObjectType.InternalTable 
                    | ObjectType.SystemTable -> true 
                    | _ -> false)

        {
            Schemas = schemas |> RCMap.toList 
            Tables = tables
            Views = views         
            
            Types = types |> Map.fold (fun acc _ v -> v :: acc) []
            TableTypes = tableTypes

            Procedures = procedures

            XmlSchemaCollections = xml_schema_collections

            Triggers = db_triggers
            Synonyms = synonyms |> RCMap.toList
            Sequences = sequences |> RCMap.toList

            Properties = dbProperties
            
            MSDescription = db_msDesc

            //all_objects = objects |> RCMap.toList
            Dependencies = dependencies
        }