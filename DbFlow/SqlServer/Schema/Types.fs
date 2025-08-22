namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql?view=sql-server-ver17
// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-table-types-transact-sql?view=sql-server-ver17
// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-columns-transact-sql?view=sql-server-ver17

type SysDatatype = 
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
    name : string
    schema : Schema

    system_type_id : byte
    user_type_id : int // ID

    parameter : DatatypeParameter
    
    is_user_defined : bool

    sys_datatype : SysDatatype option
    table_datatype : TableDatatype option
}

module Datatype =
    let typeStr' schemazenCompatibility is_user_defined_type (dtName : string) (sys_datatype : SysDatatype option) (p : DatatypeParameter)=
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
        | Some(SysDatatype.DATETIME2) -> 
            if schemazenCompatibility then plain dtName else withPrecision dtName p.scale
        | Some(SysDatatype.DATETIMEOFFSET) -> 
            if schemazenCompatibility then plain dtName else withPrecision dtName p.scale

        | Some(SysDatatype.CHAR) -> withSize dtName p.max_length 1s
        | Some(SysDatatype.VARCHAR) -> withSize dtName p.max_length 1s
        | Some(SysDatatype.NCHAR) -> withSize dtName p.max_length 2s
        | Some(SysDatatype.NVARCHAR) -> withSize dtName p.max_length 2s
        | Some(SysDatatype.BINARY) -> withSize dtName p.max_length 1s
        | Some(SysDatatype.VARBINARY) -> withSize dtName p.max_length 1s

        | Some(SysDatatype.DECIMAL) -> withPrecisionScale dtName p.precision p.scale
        | Some(SysDatatype.NUMERIC) -> withPrecisionScale dtName p.precision p.scale

        | _ -> plain dtName

    let typeStr schemazenCompatibility is_user_defined_type (dt : Datatype) =
        typeStr' schemazenCompatibility is_user_defined_type dt.name dt.sys_datatype dt.parameter

    let createSystemDataType sys_type_name =
        match sys_type_name with 
        | "tinyint"    -> SysDatatype.TINYINT
        | "smallint"   -> SysDatatype.SMALLINT
        | "int"        -> SysDatatype.INT
        | "bigint"     -> SysDatatype.BIGINT
        | "bit"        -> SysDatatype.BIT
        | "decimal"    -> SysDatatype.DECIMAL
        | "numeric"    -> SysDatatype.NUMERIC
        | "money"      -> SysDatatype.MONEY
        | "smallmoney" -> SysDatatype.SMALLMONEY
        
        | "float" -> SysDatatype.FLOAT
        | "real"-> SysDatatype.REAL
                
        | "date" -> SysDatatype.DATE
        | "time" -> SysDatatype.TIME 
        | "datetime2" -> SysDatatype.DATETIME2  
        | "datetimeoffset" -> SysDatatype.DATETIMEOFFSET  
        | "datetime" -> SysDatatype.DATETIME
        | "smalldatetime" -> SysDatatype.SMALLDATETIME

        | "char" -> SysDatatype.CHAR
        | "varchar" -> SysDatatype.VARCHAR 
        | "text" -> SysDatatype.TEXT
        
        | "nchar" -> SysDatatype.NCHAR
        | "nvarchar" -> SysDatatype.NVARCHAR
        | "ntext" -> SysDatatype.NTEXT

        | "binary" -> SysDatatype.BINARY
        | "varbinary" -> SysDatatype.VARBINARY
        | "image" -> SysDatatype.IMAGE

        | "cursor" -> SysDatatype.CURSOR
        | "geography" -> SysDatatype.GEOGRAPHY
        | "geometry" -> SysDatatype.GEOMETRY
        | "hierarchyid" -> SysDatatype.HIERARCHYID
        | "json"   -> SysDatatype.JSON
        | "vector" -> SysDatatype.VECTOR
        | "rowversion"  -> SysDatatype.ROWVERSION
        | "sql_variant" -> SysDatatype.SQL_VARIANT
        | "table"     -> SysDatatype.TABLE
        | "uniqueidentifier" -> SysDatatype.UNIQUEIDENTIFIER
        | "xml"       -> SysDatatype.XML

        | "sysname"   -> SysDatatype.SYSNAME
        | "timestamp" -> SysDatatype.TIMESTAMP

        | sys_type_name -> failwithf "Unknown system type '%s'" sys_type_name
   

    let readSystemTypes connection =
        DbTr.reader
            "SELECT name, user_type_id FROM sys.types WHERE is_user_defined = 0"
            []
            (fun m r ->
                let user_type_id = readInt32 "user_type_id" r
                let name = readString "name" r
                Map.add user_type_id (createSystemDataType name) m)
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
                let schema_id = readInt32 "schema_id" r
                let user_type_id = readInt32 "user_type_id" r
                let system_type_id = readByte "system_type_id" r
                let is_user_defined = readBool "is_user_defined" r
                let is_table_type = readBool "is_table_type" r
                let name = readString "name" r
 
                Map.add
                    user_type_id
                    {
                        name = name
                        schema = RCMap.pick schema_id schemas

                        system_type_id = system_type_id
                        user_type_id = user_type_id

                        parameter = 
                            { 
                                max_length = readInt16 "max_length" r
                                precision = readByte "precision" r
                                scale = readByte "scale" r
                                collation_name = nullable "collation_name" readString r
                                is_nullable = readBool "is_nullable" r
                            }
                        
                        is_user_defined = is_user_defined

                        sys_datatype = 
                            if is_table_type
                            then None
                            else Map.tryFind user_type_id systemTypes
                        table_datatype = 
                            if is_table_type
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
            parameter = {
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
                | ObjectType.INTERNAL_TABLE 
                | ObjectType.SYSTEM_TABLE
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



    