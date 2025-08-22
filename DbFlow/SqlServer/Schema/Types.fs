namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql?view=sql-server-ver17
// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-table-types-transact-sql?view=sql-server-ver17
// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-columns-transact-sql?view=sql-server-ver17

type SystemDatatype = 
    // Exact numerics
    | TINYINT | SMALLINT | INT | BIGINT | BIT | DECIMAL | NUMERIC | MONEY | SMALLMONEY
    // Approximate numerics
    | FLOAT | REAL
    // Date and time
    | DATE | TIME | DATETIME2 | DATETIMEOFFSET | DATETIME | SMALLDATETIME
    // Character strings
    | CHAR | VARCHAR | TEXT
    // Unicode character strings
    | NCHAR | NVARCHAR | NTEXT
    // Binary strings
    | BINARY | VARBINARY | IMAGE

    // Other data types
    | CURSOR | GEOGRAPHY | GEOMETRY | HIERARCHYID | JSON | VECTOR | ROWVERSION | SQL_VARIANT | TABLE | UNIQUEIDENTIFIER | XML

    | SYSNAME // sysname(varchar(128)) 
    | TIMESTAMP // ???

type DatatypeParameter = {
    max_length : int16
    precision : byte
    scale : byte
    collation_name : string option
    is_nullable : bool
}

type TableDatatype = {
    object : OBJECT
}

type Datatype = {
    Name : string
    Schema : Schema

    SystemTypeId : byte
    UserTypeId : int // ID

    Parameter : DatatypeParameter
    
    IsUserDefined : bool

    SystemDatatype : SystemDatatype option
    TableDatatype : TableDatatype option
}

module Datatype =
    let typeStr' schemazenCompatibility is_user_defined_type (dtName : string) (sys_datatype : SystemDatatype option) (p : DatatypeParameter)=
        let formatTypeName (tName : string) = if schemazenCompatibility then tName.ToLowerInvariant () else tName.ToUpperInvariant () 
        let plain tName =
            $"[{formatTypeName tName}]"
        let withSize tName size divisor = 
            let sizeS = 
                match size with 
                | -1s -> if schemazenCompatibility then "max" else "MAX" 
                | s -> $"{s / divisor}"
            let extraSpace = if schemazenCompatibility && is_user_defined_type then " " else ""
            $"[{formatTypeName tName}]{extraSpace}({sizeS})"
        let withPrecision tName precision =
            $"[{formatTypeName tName}]({precision})"
        let withPrecisionScale tName precision scale =
            $"[{formatTypeName tName}]({precision},{scale})"
        match sys_datatype with
        | Some(SystemDatatype.DATETIME2) -> 
            if schemazenCompatibility then plain dtName else withPrecision dtName p.scale
        | Some(SystemDatatype.DATETIMEOFFSET) -> 
            if schemazenCompatibility then plain dtName else withPrecision dtName p.scale

        | Some(SystemDatatype.CHAR) -> withSize dtName p.max_length 1s
        | Some(SystemDatatype.VARCHAR) -> withSize dtName p.max_length 1s
        | Some(SystemDatatype.NCHAR) -> withSize dtName p.max_length 2s
        | Some(SystemDatatype.NVARCHAR) -> withSize dtName p.max_length 2s
        | Some(SystemDatatype.BINARY) -> withSize dtName p.max_length 1s
        | Some(SystemDatatype.VARBINARY) -> withSize dtName p.max_length 1s

        | Some(SystemDatatype.DECIMAL) -> withPrecisionScale dtName p.precision p.scale
        | Some(SystemDatatype.NUMERIC) -> withPrecisionScale dtName p.precision p.scale

        | _ -> plain dtName

    let typeStr schemazenCompatibility is_user_defined_type (dt : Datatype) =
        typeStr' schemazenCompatibility is_user_defined_type dt.Name dt.SystemDatatype dt.Parameter

    let createSystemDataType sys_type_name =
        match sys_type_name with 
        | "tinyint"     -> SystemDatatype.TINYINT
        | "smallint"    -> SystemDatatype.SMALLINT
        | "int"         -> SystemDatatype.INT
        | "bigint"      -> SystemDatatype.BIGINT
        | "bit"         -> SystemDatatype.BIT
        | "decimal"     -> SystemDatatype.DECIMAL
        | "numeric"     -> SystemDatatype.NUMERIC
        | "money"       -> SystemDatatype.MONEY
        | "smallmoney"  -> SystemDatatype.SMALLMONEY
                        
        | "float"       -> SystemDatatype.FLOAT
        | "real"        -> SystemDatatype.REAL
                        
        | "date"        -> SystemDatatype.DATE
        | "time"        -> SystemDatatype.TIME 
        | "datetime2"   -> SystemDatatype.DATETIME2  
        | "datetimeoffset" -> SystemDatatype.DATETIMEOFFSET  
        | "datetime"    -> SystemDatatype.DATETIME
        | "smalldatetime" -> SystemDatatype.SMALLDATETIME

        | "char"        -> SystemDatatype.CHAR
        | "varchar"     -> SystemDatatype.VARCHAR 
        | "text"        -> SystemDatatype.TEXT
                        
        | "nchar"       -> SystemDatatype.NCHAR
        | "nvarchar"    -> SystemDatatype.NVARCHAR
        | "ntext"       -> SystemDatatype.NTEXT
                        
        | "binary"      -> SystemDatatype.BINARY
        | "varbinary"   -> SystemDatatype.VARBINARY
        | "image"       -> SystemDatatype.IMAGE

        | "cursor"      -> SystemDatatype.CURSOR
        | "geography"   -> SystemDatatype.GEOGRAPHY
        | "geometry"    -> SystemDatatype.GEOMETRY
        | "hierarchyid" -> SystemDatatype.HIERARCHYID
        | "json"        -> SystemDatatype.JSON
        | "vector"      -> SystemDatatype.VECTOR
        | "rowversion"  -> SystemDatatype.ROWVERSION
        | "sql_variant" -> SystemDatatype.SQL_VARIANT
        | "table"       -> SystemDatatype.TABLE
        | "uniqueidentifier" -> SystemDatatype.UNIQUEIDENTIFIER
        | "xml"         -> SystemDatatype.XML
                        
        | "sysname"     -> SystemDatatype.SYSNAME
        | "timestamp"   -> SystemDatatype.TIMESTAMP

        | sys_type_name -> failwithf "Unknown system type '%s'" sys_type_name
   

    let readSystemTypes connection =
        DbTr.reader
            "SELECT name, user_type_id FROM sys.types WHERE is_user_defined = 0"
            []
            (fun m r ->
                let userTypeId = readInt32 "user_type_id" r
                let name = readString "name" r
                Map.add userTypeId (createSystemDataType name) m)
            Map.empty
        |> DbTr.commit_ connection

    let readAll schemas objects ms_descriptions connection =
        let systemTypes = readSystemTypes connection
        DbTr.reader 
            "SELECT
                  t.name
                 ,t.system_type_id
                 ,t.user_type_id
                 ,t.schema_id
                 ,t.principal_id
                 ,t.max_length
                 ,t.precision
                 ,t.scale
                 ,t.collation_name
                 ,t.is_nullable
                 ,t.is_user_defined
                 ,t.is_assembly_type
                 ,t.default_object_id
                 ,t.rule_object_id
                 ,t.is_table_type

                 ,tt.type_table_object_id
             FROM sys.types t
             LEFT OUTER JOIN sys.table_types tt ON t.user_type_id = tt.user_type_id"
            []
            (fun m r ->
                let schemaId = readInt32 "schema_id" r
                let userTypeId = readInt32 "user_type_id" r
                let system_type_id = readByte "system_type_id" r
                let isUserDefined = readBool "is_user_defined" r
                let isTableType = readBool "is_table_type" r
                let name = readString "name" r
 
                Map.add
                    userTypeId
                    {
                        Name = name
                        Schema = RCMap.pick schemaId schemas

                        SystemTypeId = system_type_id
                        UserTypeId = userTypeId

                        Parameter = 
                            { 
                                max_length = readInt16 "max_length" r
                                precision = readByte "precision" r
                                scale = readByte "scale" r
                                collation_name = nullable "collation_name" readString r
                                is_nullable = readBool "is_nullable" r
                            }
                        
                        IsUserDefined = isUserDefined

                        SystemDatatype = 
                            if isTableType
                            then None
                            else Map.tryFind userTypeId systemTypes
                        TableDatatype = 
                            if isTableType
                            then 
                                let object_id = readInt32 "type_table_object_id" r
                                {
                                    object = RCMap.pick object_id objects
                                }
                                |> Some
                            else None
                    }
                    m)
            Map.empty
        |> DbTr.commit_ connection

    let readType types collation r =
        let user_type_id = readInt32 "user_type_id" r
        { Map.find user_type_id types with
            Parameter = {
                max_length = readInt16 "max_length" r
                precision = readByte "precision" r
                scale = readByte "scale" r
                collation_name = collation
                is_nullable = readBool "is_nullable" r
            }
        }

type COMPUTED_DEFINITION = { computed_definition : string; is_persisted : bool }
type IDENTITY_DEFINITION = { seed_value : obj; increment_value : obj; last_value : obj } 

type COLUMN = {
    column_name : string     
    object : OBJECT
    column_id : int           
    
    data_type : Datatype
    
    is_ansi_padded : bool    
    computed_definition : COMPUTED_DEFINITION option
    identity_definition : IDENTITY_DEFINITION option
    masking_function : string option
    is_rowguidcol : bool     

    ms_description : string option
}    

module COLUMN =
    let readAll' objects types ms_descriptions connection =
        DbTr.reader 
            "SELECT 
                 c.name column_name, c.object_id, c.column_id, 
                 c.user_type_id, c.max_length, c.precision, c.scale, c.collation_name, 
                 c.is_nullable, c.is_ansi_padded, 
		    	 c.is_computed, cc.definition computed_definition, cc.is_persisted computed_is_persisted,
		    	 c.is_identity, ic.seed_value identity_seed_value, ic.increment_value identity_increment_value, ic.last_value identity_last_value,
		    	 c.is_rowguidcol,
                 ISNULL(mc.is_masked, 0) is_masked, mc.masking_function 
             FROM sys.columns c
             
		     LEFT OUTER JOIN sys.computed_columns cc ON cc.object_id = c.object_id AND cc.column_id = c.column_id 
		     LEFT OUTER JOIN sys.identity_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
             LEFT OUTER JOIN sys.masked_columns mc ON mc.object_id = c.object_id AND mc.column_id = c.column_id"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                let column_id = readInt32 "column_id" r
                let object : OBJECT = RCMap.pick object_id objects
                match object.ObjectType with
                | ObjectType.InternalTable 
                | ObjectType.SystemTable
                    -> acc
                | _ ->
                    {
                        column_name = readString "column_name" r
                        object = object
                            
                        column_id = column_id
                        
                        data_type = Datatype.readType types (nullable "collation_name" readString r) r
                        
                        is_ansi_padded = readBool "is_ansi_padded" r
                        computed_definition = 
                            match readBool "is_computed" r with
                            | true -> 
                                { 
                                    computed_definition = readString "computed_definition" r 
                                    is_persisted = readBool "computed_is_persisted" r 
                                } |> Some
                            | false -> None
                        identity_definition = 
                            match readBool "is_identity" r with
                            | true -> 
                                { 
                                    seed_value = readObject "identity_seed_value" r
                                    increment_value = readObject "identity_increment_value" r
                                    last_value = readObject "identity_last_value" r 
                                } |> Some
                            | false -> None
                        masking_function =
                            match readBool "is_masked" r with
                            | true -> readString "masking_function" r |> Some 
                            | false -> None
                        is_rowguidcol = readBool "is_rowguidcol" r

                        ms_description = RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, column_id) ms_descriptions 
                    } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects types ms_descriptions connection =
        let columns' = readAll' objects types ms_descriptions connection
        let columns =
            columns' 
            |> List.map (fun c -> (c.object.ObjectId, c.column_id), c)
            |> Map.ofList
            |> RCMap.ofMap
        let columnsByObject =
            columns'
            |> List.groupBy (fun c -> c.object.ObjectId)
            |> List.fold 
                (fun m (object_id, cs) -> 
                    Map.add object_id (cs |> List.sortBy (fun c -> c.column_id) |> List.toArray) m)
                Map.empty
            |> RCMap.ofMap
        columns, columnsByObject



    