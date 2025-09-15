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

    XProperties : Map<string, string>
}

module Schema =
    let isSystemSchema =
        let sysSchemas = ["dbo"; "guest"; "sys"; "INFORMATION_SCHEMA"] |> Set.ofList
        fun name id -> id >= 16384 || Set.contains name sysSchemas

    let readAll xProperties connection =
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
                    
                        XProperties = XProperty.getXProperties (XPropertyClass.Schema, schemaId, 0) xProperties
                    } 
                    m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-objects-transact-sql?view=sql-server-ver17

[<RequireQualifiedAccess>]
type ObjectType =
    | AggregateFunction  // AF = Aggregate function (CLR)
    | CheckConstraint    // C = Check constraint
    | DefaultConstraint  // D = Default (constraint or stand-alone)
    | ForeignKeyConstraint // F = Foreign key constraint
    | SqlScalarFunction // FN = SQL scalar function
    | ClrScalarFunction // FS = Assembly (CLR) scalar-function
    | ClrTableValuedFunction // FT = Assembly (CLR) table-valued function
    
    | SqlInlineTableValuedFunction // IF = SQL inline table-valued function (TVF)
    | InternalTable // IT = Internal table
    | SqlStoredProcedure // P = SQL stored procedure
    | ClrStoredProcedure // PC = Assembly (CLR) stored-procedure
    | PlanGuide // PG = Plan guide
    | PrimaryKeyConstraint // PK = Primary key constraint
    | Rule // R = Rule (old-style, stand-alone)
    | ReplicationFilterProcedure // RF = Replication-filter-procedure
    | SystemTable // S = System base table
    | Synonym // SN = Synonym
    | SequenceObject // SO = Sequence object
    | UserTable // U = Table (user-defined)
    | View // V = View

    // Applies to: SQL Server 2012 (11.x) and later versions
    | ServiceQueue // SQ = Service queue
    | ClrTrigger // TA = Assembly (CLR) DML trigger
    | SqlTableValuedFunction // TF = SQL table-valued-function (TVF)
    | SqlTrigger // TR = SQL DML trigger
    | TypeTable // TT = Table type
    | UniqueConstraint // UQ = unique constraint
    | ExtendedStoredProcedure // X = Extended stored procedure
    
    // Applies to: SQL Server 2014 (12.x) and later versions, Azure SQL Database, Azure Synapse Analytics, Analytics Platform System (PDW)
    // ST = Statistics tree
    // NOT YET SUPPORTED!
    
    // Applies to: SQL Server 2016 (13.x) and later versions, Azure SQL Database, Azure Synapse Analytics, Analytics Platform System (PDW)
    // ET = External table
    // NOT YET SUPPORTED!

    // Applies to: SQL Server 2017 (14.x) and later versions, Azure SQL Database, Azure Synapse Analytics, Analytics Platform System (PDW)
    | EdgeConstraint // EC = Edge constraint
    

module ObjectType =
    let mappingTable' =
        [
            "AF", ObjectType.AggregateFunction 
            "C ", ObjectType.CheckConstraint
            "D ", ObjectType.DefaultConstraint
            "F ", ObjectType.ForeignKeyConstraint
            "FN", ObjectType.SqlScalarFunction
            "FS", ObjectType.ClrScalarFunction
            "FT", ObjectType.ClrTableValuedFunction
            "IF", ObjectType.SqlInlineTableValuedFunction
            "IT", ObjectType.InternalTable
            "P ", ObjectType.SqlStoredProcedure
            "PC", ObjectType.ClrStoredProcedure
            "PG", ObjectType.PlanGuide
            "PK", ObjectType.PrimaryKeyConstraint
            "R ", ObjectType.Rule
            "RF", ObjectType.ReplicationFilterProcedure
            "S ", ObjectType.SystemTable
            "SN", ObjectType.Synonym
            "SO", ObjectType.SequenceObject
            "U ", ObjectType.UserTable
            "V ", ObjectType.View
            "SQ", ObjectType.ServiceQueue
            "TA", ObjectType.ClrTrigger
            "TF", ObjectType.SqlTableValuedFunction
            "TR", ObjectType.SqlTrigger
            "TT", ObjectType.TypeTable
            "UQ", ObjectType.UniqueConstraint
            "X ", ObjectType.ExtendedStoredProcedure
            "EC", ObjectType.EdgeConstraint
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
    let fullName (o : OBJECT) = $"[{o.Schema.Name}].[{o.Name}]"
    
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

    XProperties : Map<string, string>
}

module XmlSchemaCollection = 
    let readAll schemas xProperties connection =
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

                    XProperties = XProperty.getXProperties (XPropertyClass.XmlSchemaCollection, xmlCollectionId, 0) xProperties
                } :: acc)
            []
        |> DbTr.commit_ connection


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-sql-modules-transact-sql?view=sql-server-ver17

type SqlModule = {
    ObjectId : int
    Definition : string
}

module SqlModule =
    let readAll connection =
        DbTr.reader 
            "SELECT object_id, definition FROM sys.sql_modules" 
            []
            (fun m r ->
                let objectId = readInt32 "object_id" r
                Map.add objectId { ObjectId = objectId; Definition = readString "definition" r} m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-synonyms-transact-sql?view=sql-server-ver17

type Synonym = {
    Object : OBJECT

    // Fully quoted name of the object to which the user of this synonym is redirected.
    BaseObjectName : string
}

module Synonym =
    let readAll objects connection =
        DbTr.reader 
            "SELECT object_id, base_object_name FROM sys.synonyms" 
            []
            (fun m r ->
                let object_id = readInt32 "object_id" r
                Map.add object_id { Object = RCMap.pick object_id objects; BaseObjectName = readString "base_object_name" r } m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap