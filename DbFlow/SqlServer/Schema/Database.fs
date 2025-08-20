namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-databases-transact-sql?view=sql-server-ver17

type DATABASE_SETTINGS = {
    compatibility_level : byte
    collation_name : string
}
    (*
    https://github.com/sethreno/schemazen/blob/6787ba30e555220c61186cb7b9cd3713cc9226d0/Library/Models/Database.cs#L1131

    select
	[compatibility_level],
	[collation_name],
	[is_auto_close_on],
	[is_auto_shrink_on],
	[snapshot_isolation_state],
	[is_read_committed_snapshot_on],
	[recovery_model_desc],
	[page_verify_option_desc],
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
from sys.databases
where name = @dbname

(Undersök om/hur "CURRENT" kan användas...)
    *)
module DATABASE_SETTINGS = 
    let readAll connection =
        DbTr.reader
            "SELECT
                [compatibility_level],
                [collation_name],
                [is_auto_close_on],
                [is_auto_shrink_on],
                [snapshot_isolation_state],
                [is_read_committed_snapshot_on],
                [recovery_model_desc],
                [page_verify_option_desc],
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
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> function
            | [settings] -> settings
            | _ -> failwithf "Multiple of no settings found - should not happen"
        

type DATABASE = {
    SCHEMAS : SCHEMA list
    TABLES : TABLE list
    VIEWS : VIEW list

    TYPES : DATATYPE list
    TABLE_TYPES : TABLE_TYPE list

    PROCEDURES : PROCEDURE list

    XML_SCHEMA_COLLECTIONS : XML_SCHEMA_COLLECTION list

    TRIGGERS : DATABASE_TRIGGER list
    SYNONYMS : SYNONYM list
    SEQUENCES : SEQUENCE list

    SETTINGS : DATABASE_SETTINGS

    ms_description : string option

    all_objects : OBJECT list
    dependencies : Map<int, int list>
}

module DATABASE =
    // Read all views in order to redefined them - to get fresh/correct meta data 
    let preReadAllViews logger connection =
        let ms_descs = RCMap.ofMap Map.empty
        let dependencies = DEPENDENCY.readAll |> Logger.logTime logger "Dependencies" connection
        
        let schemas = SCHEMA.readAll ms_descs connection
        let objects = OBJECT.readAll schemas |> Logger.logTime logger "OBJECT" connection
        let types = DATATYPE.readAll schemas objects ms_descs connection

        let sql_modules = SQL_MODULE.readAll connection
        
        let (columns, columnsByObject) = COLUMN.readAll objects types ms_descs |> Logger.logTime logger "COLUMN" connection
        let triggersByParent = TRIGGER.readAll objects sql_modules ms_descs |> Logger.logTime logger "TRIGGER" connection
        
        let indexesColumnsByIndex = INDEX_COLUMN.readAll objects columns |> Logger.logTime logger "INDEX_COLUMN" connection
        let indexesByParent = INDEX.readAll objects indexesColumnsByIndex ms_descs |> Logger.logTime logger "INDEX" connection
        
        let views = 
            VIEW.readAll schemas objects columnsByObject indexesByParent triggersByParent sql_modules ms_descs
            |> Logger.logTime logger "VIEW" connection
        
        views, dependencies

    let read logger (options : Options) connection =
        let ms_descs = MS_Description.readAll |> Logger.logTime logger "MS_Description" connection
        let dependencies = DEPENDENCY.readAll |> Logger.logTime logger "Dependencies" connection
        
        let settings = DATABASE_SETTINGS.readAll connection

        let schemas = SCHEMA.readAll ms_descs connection
        let objects = OBJECT.readAll schemas |> Logger.logTime logger "OBJECT" connection
        let types = DATATYPE.readAll schemas objects ms_descs connection

        let sequences = SEQUENCE.readAll objects types connection
        let synonyms = SYNONYM.readAll objects connection
        let sql_modules = SQL_MODULE.readAll connection
        let xml_schema_collections = XML_SCHEMA_COLLECTION.readAll schemas ms_descs connection
        
        let (columns, columnsByObject) = COLUMN.readAll objects types ms_descs |> Logger.logTime logger "COLUMN" connection
        let triggersByParent = TRIGGER.readAll objects sql_modules ms_descs |> Logger.logTime logger "TRIGGER" connection
        
        // A bit strange that keyConstraints isn't used...?!
        let keyConstraints = KEY_CONSTRAINT.readAll objects ms_descs connection
        let checkConstraintsByParent = 
            CHECK_CONSTRAINT.readAll objects columns ms_descs
            |> Logger.logTime logger "CHECK_CONSTRAINT" connection
        let defaultConstraintsByParent = 
            DEFAULT_CONSTRAINT.readAll objects columns ms_descs 
            |> Logger.logTime logger "DEFAULT_CONSTRAINT" connection
        

        let fkColsByConstraint = FOREIGN_KEY_COLUMN.readAll objects columns |> Logger.logTime logger "FOREIGN_KEY_COLUMN" connection 
        let (foreignKeysByParent, foreignKeysByReferenced)  = 
            FOREIGN_KEY.readAll objects fkColsByConstraint ms_descs 
             |> Logger.logTime logger "FOREIGN_KEY" connection
        
        let indexesColumnsByIndex = INDEX_COLUMN.readAll objects columns |> Logger.logTime logger "INDEX_COLUMN" connection
        let indexesByParent = INDEX.readAll objects indexesColumnsByIndex ms_descs |> Logger.logTime logger "INDEX" connection
        
        let tableTypes =
            TABLE_TYPE.readAll schemas objects ms_descs columnsByObject 
                indexesByParent foreignKeysByParent foreignKeysByReferenced checkConstraintsByParent defaultConstraintsByParent 
            |> Logger.logTime logger "TABLE_TYPE" connection
            

        let parametersByObject = PARAMETER.readAll objects types ms_descs connection
        let procedures = PROCEDURE.readAll objects parametersByObject columnsByObject indexesByParent sql_modules ms_descs connection

        let tables = 
            TABLE.readAll 
                schemas objects columnsByObject indexesByParent
                triggersByParent foreignKeysByParent foreignKeysByReferenced 
                checkConstraintsByParent defaultConstraintsByParent ms_descs 
            |> Logger.logTime logger "TABLE" connection
        let views = 
            VIEW.readAll schemas objects columnsByObject indexesByParent triggersByParent sql_modules ms_descs
            |> Logger.logTime logger "VIEW" connection
        
        let db_msDesc = RCMap.tryPick (XPROPERTY_CLASS.DATABASE, 0, 0) ms_descs
        let db_triggers = TRIGGER.readAllDatabaseTriggers objects sql_modules ms_descs connection

        let checkUnused (id : string) exclude pm =
            match RCMap.unused exclude pm with
            | [] -> ()
            | unused -> failwith $"{id} not mapped for {unused}" 

        // Check that objects are "picked" (referenced) by something
        if not options.BypassReferenceChecksOnLoad
        then
            ms_descs |> checkUnused "ms_descriptions" (fun _ -> false)
            columnsByObject |> checkUnused "columns" (fun _ -> false)
            objects |> checkUnused "objects" (fun c -> c.object_type = OBJECT_TYPE.SERVICE_QUEUE)
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
                    match i[0].parent.object_type with 
                    | OBJECT_TYPE.INTERNAL_TABLE 
                    | OBJECT_TYPE.SYSTEM_TABLE -> true 
                    | _ -> false)

        {
            SCHEMAS = schemas |> RCMap.toList 
            TABLES = tables
            VIEWS = views         
            
            TYPES = types |> Map.fold (fun acc _ v -> v :: acc) []
            TABLE_TYPES = tableTypes

            PROCEDURES = procedures

            XML_SCHEMA_COLLECTIONS = xml_schema_collections

            TRIGGERS = db_triggers
            SYNONYMS = synonyms |> RCMap.toList
            SEQUENCES = sequences |> RCMap.toList

            SETTINGS = settings
            ms_description = db_msDesc

            all_objects = objects |> RCMap.toList
            dependencies = dependencies
        }