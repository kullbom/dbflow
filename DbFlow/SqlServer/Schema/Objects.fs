namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-sql-expression-dependencies-transact-sql?view=sql-server-ver17

module Dependency =
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


type Schema = { 
    Name : string; 
    SchemaId : int; 
    PrincipalName : string;

    IsSystemSchema : bool

    MSDescription : string option 
}

module Schema =
    let isSystemSchema =
        let sysSchemas = ["dbo"; "guest"; "sys"; "INFORMATION_SCHEMA"] |> Set.ofList
        fun name id -> id >= 16384 || Set.contains name sysSchemas

    let readAll ms_descriptions connection =
        DbTr.reader 
            "SELECT s.name schema_name, s.schema_id, p.name principal_name FROM sys.schemas s
             INNER JOIN sys.database_principals p ON s.principal_id = p.principal_id" 
            []
            (fun m r -> 
                let schemaId = readInt32 "schema_id" r
                let schemaName = readString "schema_name" r
                Map.add 
                    schemaId 
                    { 
                        Name = schemaName
                        SchemaId = schemaId 
                        PrincipalName = readString "principal_name" r

                        IsSystemSchema = isSystemSchema schemaName schemaId
                    
                        MSDescription = RCMap.tryPick (XPropertyClass.Schema, schemaId, 0) ms_descriptions
                    } 
                    m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-objects-transact-sql?view=sql-server-ver17

[<RequireQualifiedAccess>]
type ObjectType =
    | AGGREGATE_FUNCTION  // AF = Aggregate function (CLR)
    | CHECK_CONSTRAINT    // C = Check constraint
    | DEFAULT_CONSTRAINT  // D = Default (constraint or stand-alone)
    | FOREIGN_KEY_CONSTRAINT // F = Foreign key constraint
    | SQL_SCALAR_FUNCTION // FN = SQL scalar function
    | CLR_SCALAR_FUNCTION // FS = Assembly (CLR) scalar-function
    | CLR_TABLE_VALUED_FUNCTION // FT = Assembly (CLR) table-valued function
    
    | SQL_INLINE_TABLE_VALUED_FUNCTION // IF = SQL inline table-valued function (TVF)
    | INTERNAL_TABLE // IT = Internal table
    | SQL_STORED_PROCEDURE // P = SQL stored procedure
    | CLR_STORED_PROCEDURE // PC = Assembly (CLR) stored-procedure
    | PLAN_GUIDE // PG = Plan guide
    | PRIMARY_KEY_CONSTRAINT // PK = Primary key constraint
    | RULE // R = Rule (old-style, stand-alone)
    | REPLICATION_FILTER_PROCEDURE // RF = Replication-filter-procedure
    | SYSTEM_TABLE // S = System base table
    | SYNONYM // SN = Synonym
    | SEQUENCE_OBJECT // SO = Sequence object
    | USER_TABLE // U = Table (user-defined)
    | VIEW // V = View

    // Applies to: SQL Server 2012 (11.x) and later versions
    | SERVICE_QUEUE // SQ = Service queue
    | CLR_TRIGGER // TA = Assembly (CLR) DML trigger
    | SQL_TABLE_VALUED_FUNCTION // TF = SQL table-valued-function (TVF)
    | SQL_TRIGGER // TR = SQL DML trigger
    | TYPE_TABLE // TT = Table type
    | UNIQUE_CONSTRAINT // UQ = unique constraint
    | EXTENDED_STORED_PROCEDURE // X = Extended stored procedure
    
    // Applies to: SQL Server 2014 (12.x) and later versions, Azure SQL Database, Azure Synapse Analytics, Analytics Platform System (PDW)
    // ST = Statistics tree
    // NOT YET SUPPORTED!
    
    // Applies to: SQL Server 2016 (13.x) and later versions, Azure SQL Database, Azure Synapse Analytics, Analytics Platform System (PDW)
    // ET = External table
    // NOT YET SUPPORTED!

    // Applies to: SQL Server 2017 (14.x) and later versions, Azure SQL Database, Azure Synapse Analytics, Analytics Platform System (PDW)
    | EDGE_CONSTRAINT // EC = Edge constraint
    

module ObjectType =
    let mappingTable' =
        [
            "AF", ObjectType.AGGREGATE_FUNCTION 
            "C ", ObjectType.CHECK_CONSTRAINT
            "D ", ObjectType.DEFAULT_CONSTRAINT
            "F ", ObjectType.FOREIGN_KEY_CONSTRAINT
            "FN", ObjectType.SQL_SCALAR_FUNCTION
            "FS", ObjectType.CLR_SCALAR_FUNCTION
            "FT", ObjectType.CLR_TABLE_VALUED_FUNCTION
            "IF", ObjectType.SQL_INLINE_TABLE_VALUED_FUNCTION
            "IT", ObjectType.INTERNAL_TABLE
            "P ", ObjectType.SQL_STORED_PROCEDURE
            "PC", ObjectType.CLR_STORED_PROCEDURE
            "PG", ObjectType.PLAN_GUIDE
            "PK", ObjectType.PRIMARY_KEY_CONSTRAINT
            "R ", ObjectType.RULE
            "RF", ObjectType.REPLICATION_FILTER_PROCEDURE
            "S ", ObjectType.SYSTEM_TABLE
            "SN", ObjectType.SYNONYM
            "SO", ObjectType.SEQUENCE_OBJECT
            "U ", ObjectType.USER_TABLE
            "V ", ObjectType.VIEW
            "SQ", ObjectType.SERVICE_QUEUE
            "TA", ObjectType.CLR_TRIGGER
            "TF", ObjectType.SQL_TABLE_VALUED_FUNCTION
            "TR", ObjectType.SQL_TRIGGER
            "TT", ObjectType.TYPE_TABLE
            "UQ", ObjectType.UNIQUE_CONSTRAINT
            "X ", ObjectType.EXTENDED_STORED_PROCEDURE
            "EC", ObjectType.EDGE_CONSTRAINT
        ]

    let findType =
        let codeToType' = mappingTable' |> Map.ofList
        fun code -> 
            match Map.tryFind code codeToType' with
            | Some t -> t
            | None -> failwithf "Could not find object type from code '%s'" code

    let findCode =
        let typeToCode' = mappingTable' |> List.map (fun (code, t) -> t, code) |> Map.ofList
        fun t -> 
            match Map.tryFind t typeToCode' with
            | Some code -> code 
            | None -> failwithf "Could not find code from object type '%A'" t

type OBJECT = {
    Name : string
    ObjectId : int
    Schema : Schema
    ParentObjectId : int option
    ObjectType : ObjectType
    CreateDate : System.DateTime
    ModifyDate : System.DateTime
}

module OBJECT = 
    let readAll schemas connection =
        DbTr.reader
            "SELECT 
                 o.name, o.object_id, o.schema_id, o.parent_object_id, o.type,
                 o.create_date, o.modify_date
             FROM sys.objects o"
            []
            (fun m r -> 
                let object_id = readInt32 "object_id" r
                Map.add 
                    object_id
                    {
                        Name = readString "name" r
                        ObjectId = object_id
                        Schema = RCMap.pick (readInt32 "schema_id" r) schemas
                        ParentObjectId = nullable "parent_object_id" readInt32 r
                        ObjectType = readString "type" r |> ObjectType.findType
                        CreateDate = readDateTime "create_date" r
                        ModifyDate = readDateTime "modify_date" r 
                    }
                    m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap


type XmlSchemaCollection = {
    XmlCollectionId : int 
    Schema : Schema
    //principal_id
    Name : string
    CreateDate : System.DateTime
    ModifyDate : System.DateTime

    Definition : string 

    MSDescription : string option
}

module XmlSchemaCollection = 
    let readAll schemas ms_descriptions connection =
        DbTr.reader
            "SELECT 
                xc.xml_collection_id, xc.schema_id, xc.principal_id, xc.name, xc.create_date, xc.modify_date,
                xml_schema_namespace(s.name, xc.name) as definition
             FROM sys.xml_schema_collections xc
             INNER JOIN sys.schemas s ON s.schema_id = xc.schema_id
             WHERE s.name != 'sys'"
            []
            (fun acc r ->
                let xmlCollectionId = readInt32 "xml_collection_id" r
                {
                    XmlCollectionId = xmlCollectionId
                    Schema = RCMap.pick (readInt32 "schema_id" r) schemas
                    //principal_id
                    Name = readString "name" r
                    CreateDate = readDateTime "create_date" r
                    ModifyDate = readDateTime "modify_date" r

                    Definition = readString "definition" r

                    MSDescription = RCMap.tryPick (XPropertyClass.XmlSchemaCollection, xmlCollectionId, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-sql-modules-transact-sql?view=sql-server-ver17

type SQL_MODULE = {
    object_id : int
    definition : string
}

module SQL_MODULE =
    let readAll connection =
        DbTr.reader 
            "SELECT object_id, definition FROM sys.sql_modules" 
            []
            (fun m r ->
                let object_id = readInt32 "object_id" r
                Map.add object_id { object_id = object_id; definition = readString "definition" r} m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-synonyms-transact-sql?view=sql-server-ver17

type SYNONYM = {
    object : OBJECT

    // Fully quoted name of the object to which the user of this synonym is redirected.
    base_object_name : string
}

module SYNONYM =
    let readAll objects connection =
        DbTr.reader 
            "SELECT object_id, base_object_name FROM sys.synonyms" 
            []
            (fun m r ->
                let object_id = readInt32 "object_id" r
                Map.add object_id { object = RCMap.pick object_id objects; base_object_name = readString "base_object_name" r } m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap