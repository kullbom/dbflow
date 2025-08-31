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
    MaxLength : int16
    Precision : byte
    Scale : byte
    CollationName : string option
    IsNullable : bool
}

type DatatypeSpec =
      UserDefined //of SystemDatatype
    | SystemType of SystemDatatype
    | TableType of OBJECT

type Datatype = {
    Name : string
    Schema : Schema

    SystemTypeId : byte
    UserTypeId : int // "PK"

    Parameter : DatatypeParameter
    
    DatatypeSpec : DatatypeSpec
    
    XProperties : Map<string, string>
}

module Datatype =
    let typeStr' (dtName : string) (typeSpec : DatatypeSpec) (p : DatatypeParameter)=
        let formatTypeName (tName : string) = tName.ToUpperInvariant () 
        let plain tName =
            $"[{formatTypeName tName}]"
        let withSize tName size divisor = 
            let sizeS = 
                match size with 
                | -1s -> "MAX" 
                | s -> $"{s / divisor}"
            $"[{formatTypeName tName}]({sizeS})"
        let withPrecision tName precision =
            $"[{formatTypeName tName}]({precision})"
        let withPrecisionScale tName precision scale =
            $"[{formatTypeName tName}]({precision},{scale})"
        match typeSpec with
        | SystemType SystemDatatype.DATETIME2 -> 
            withPrecision dtName p.Scale
        | SystemType SystemDatatype.DATETIMEOFFSET -> 
            withPrecision dtName p.Scale

        | SystemType SystemDatatype.CHAR -> withSize dtName p.MaxLength 1s
        | SystemType SystemDatatype.VARCHAR -> withSize dtName p.MaxLength 1s
        | SystemType SystemDatatype.NCHAR -> withSize dtName p.MaxLength 2s
        | SystemType SystemDatatype.NVARCHAR -> withSize dtName p.MaxLength 2s
        | SystemType SystemDatatype.BINARY -> withSize dtName p.MaxLength 1s
        | SystemType SystemDatatype.VARBINARY -> withSize dtName p.MaxLength 1s

        | SystemType SystemDatatype.DECIMAL -> withPrecisionScale dtName p.Precision p.Scale
        | SystemType SystemDatatype.NUMERIC -> withPrecisionScale dtName p.Precision p.Scale

        | _ -> plain dtName

    let typeStr (dt : Datatype) =
        typeStr' dt.Name dt.DatatypeSpec dt.Parameter

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

    let readAll schemas objects xProperties connection =
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
                let systemTypeId = readByte "system_type_id" r
                let isUserDefined = readBool "is_user_defined" r
                let tableTypeObjectId = 
                    if readBool "is_table_type" r 
                    then Some (readInt32 "type_table_object_id" r)
                    else None
                let name = readString "name" r
 
                Map.add
                    userTypeId
                    {
                        Name = name
                        Schema = RCMap.pick schemaId schemas

                        SystemTypeId = systemTypeId
                        UserTypeId = userTypeId

                        Parameter = 
                            { 
                                MaxLength = readInt16 "max_length" r
                                Precision = readByte "precision" r
                                Scale = readByte "scale" r
                                CollationName = nullable "collation_name" readString r
                                IsNullable = readBool "is_nullable" r
                            }
                        
                        DatatypeSpec =
                            match tableTypeObjectId, isUserDefined with
                            | Some objectId, _ -> 
                                RCMap.pick objectId objects
                                |> TableType 
                            | None, true ->
                                UserDefined
                            | None, false ->
                                SystemType (Map.find userTypeId systemTypes)

                        XProperties = 
                            let x0 = XProperty.getXProperties (XPropertyClass.Type, userTypeId, 0) xProperties
                            match tableTypeObjectId with
                            | None -> x0
                            | Some objectId ->
                                XProperty.getXProperties (XPropertyClass.ObjectOrColumn, objectId, 0) xProperties
                                |> Map.fold (fun m k v -> Map.add k v m) x0
                    }
                    m)
            Map.empty
        |> DbTr.commit_ connection

    let readType types collation r =
        let userTypeId = readInt32 "user_type_id" r
        { Map.find userTypeId types with
            Parameter = {
                MaxLength = readInt16 "max_length" r
                Precision = readByte "precision" r
                Scale = readByte "scale" r
                CollationName = collation
                IsNullable = readBool "is_nullable" r
            }
        }

type ComputedDefinition = { ComputedDefinition : string; IsPersisted : bool }
type IdentityDefinition = { SeedValue : obj; IncrementValue : obj; LastValue : obj } 

type Column = {
    Name : string     
    Object : OBJECT
    ColumnId : int           
    
    Datatype : Datatype
    
    IsAnsiPadded : bool    
    ComputedDefinition : ComputedDefinition option
    IdentityDefinition : IdentityDefinition option
    MaskingFunction : string option
    IsRowguidcol : bool     

    XProperties : Map<string,string>
}    

module Column =
    let readAll' objects types xProperties connection =
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
                    let datatype = Datatype.readType types (nullable "collation_name" readString r) r
                    {
                        Name = readString "column_name" r
                        Object = object
                            
                        ColumnId = column_id
                        
                        Datatype = datatype
                        
                        IsAnsiPadded = readBool "is_ansi_padded" r
                        ComputedDefinition = 
                            match readBool "is_computed" r with
                            | true -> 
                                { 
                                    ComputedDefinition = readString "computed_definition" r 
                                    IsPersisted = readBool "computed_is_persisted" r 
                                } |> Some
                            | false -> None
                        IdentityDefinition = 
                            match readBool "is_identity" r with
                            | true -> 
                                { 
                                    SeedValue = readObject "identity_seed_value" r
                                    IncrementValue = readObject "identity_increment_value" r
                                    LastValue = readObject "identity_last_value" r 
                                } |> Some
                            | false -> None
                        MaskingFunction =
                            match readBool "is_masked" r with
                            | true -> readString "masking_function" r |> Some 
                            | false -> None
                        IsRowguidcol = readBool "is_rowguidcol" r

                        XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, object_id, column_id) xProperties
                    } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects types xProperties connection =
        let columns' = readAll' objects types xProperties connection
        let columns =
            columns' 
            |> List.map (fun c -> (c.Object.ObjectId, c.ColumnId), c)
            |> Map.ofList
            |> RCMap.ofMap
        let columnsByObject =
            columns'
            |> List.groupBy (fun c -> c.Object.ObjectId)
            |> List.fold 
                (fun m (object_id, cs) -> 
                    Map.add object_id (cs |> List.sortBy (fun c -> c.ColumnId) |> List.toArray) m)
                Map.empty
            |> RCMap.ofMap
        columns, columnsByObject



    