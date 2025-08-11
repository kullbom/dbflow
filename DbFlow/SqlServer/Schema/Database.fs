namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-sql-expression-dependencies-transact-sql?view=sql-server-ver17

module DEPENDENCY =
    let readAll connection =
        DbTr.reader
            "SELECT DISTINCT d.referencing_id, d.referenced_id, d.is_schema_bound_reference 
             FROM sys.sql_expression_dependencies d
             WHERE d.referencing_id IS NOT NULL 
                AND d.referenced_id IS NOT NULL
                AND d.referencing_id <> d.referenced_id"
            []
            (fun acc r -> 
                (readInt32 "referencing_id" r, readInt32 "referenced_id" r) :: acc)
            []
        |> DbTr.commit_ connection
        |> List.groupBy fst 
        |> List.map (fun (referencing_id, xs) -> referencing_id, xs |> List.map snd)
        |> Map.ofList

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

    ms_description : string option

    all_objects : OBJECT list
    dependencies : Map<int, int list>
}

module DATABASE =
    let read logger (options : Options) connection =
        let ms_descs = MS_Description.readAll |> Logger.logTime logger "MS_Description" connection
        let dependencies = DEPENDENCY.readAll |> Logger.logTime logger "Dependencies" connection
        
        let schemas = SCHEMA.readAll ms_descs connection
        let objects = OBJECT.readAll schemas |> Logger.logTime logger "OBJECT" connection
        let types = DATATYPE.readAll schemas objects ms_descs connection

        let sequences = SEQUENCE.readAll objects types connection
        let synonyms = SYNONYM.readAll objects connection
        let sql_modules = SQL_MODULE.readAll connection
        let xml_schema_collections = XML_SCHEMA_COLLECTION.readAll schemas ms_descs connection
        
        let (columns, columnsByObject) = COLUMN.readAll objects types ms_descs |> Logger.logTime logger "COLUMN" connection
        let triggersByParent = TRIGGER.readAll objects sql_modules ms_descs |> Logger.logTime logger "TRIGGER" connection
        
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
        
        let db_msDesc = PickMap.tryPick (XPROPERTY_CLASS.DATABASE, 0, 0) ms_descs
        let db_triggers = TRIGGER.readAllDatabaseTriggers objects sql_modules ms_descs connection

        let checkUnused (id : string) exclude pm =
            match PickMap.unused exclude pm with
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
            SCHEMAS = schemas |> PickMap.toList 
            TABLES = tables
            VIEWS = views         
            
            TYPES = types |> Map.fold (fun acc _ v -> v :: acc) []
            TABLE_TYPES = tableTypes

            PROCEDURES = procedures

            XML_SCHEMA_COLLECTIONS = xml_schema_collections

            TRIGGERS = db_triggers
            SYNONYMS = synonyms |> PickMap.toList
            SEQUENCES = sequences |> PickMap.toList

            ms_description = db_msDesc

            all_objects = objects |> PickMap.toList
            dependencies = dependencies
        }