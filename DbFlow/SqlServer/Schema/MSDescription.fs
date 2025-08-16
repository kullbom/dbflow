namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/extended-properties-catalog-views-sys-extended-properties?view=sql-server-ver17

[<RequireQualifiedAccess>]
type XPROPERTY_CLASS = 
    | DATABASE = 0uy
    | OBJECT_OR_COLUMN = 1uy
    | PARAMETER = 2uy
    | SCHEMA = 3uy
    | DATABASE_PRINCIPAL = 4uy
    | ASSEMBLY = 5uy
    | TYPE = 6uy
    | INDEX = 7uy
    // 8 = User defined table type column
    | XML_SCHEMA_COLLECTION = 10uy
    | MESSAGE_TYPE = 15uy
    | SERVICE_CONTRACT = 16uy
    | SERVICE = 17uy
    | REMOTE_SERVICE_BINDING = 18uy
    | ROUTE = 19uy
    | DATASPACE = 20uy
    | PARTITION_FUNCTION = 21uy
    | DATABASE_FILE = 22uy
    | PLAN_GUIDE = 27uy

module XPROPERTY_CLASS = 
    let findClass classCode =
        match classCode with
        |  0uy -> XPROPERTY_CLASS.DATABASE
        |  1uy -> XPROPERTY_CLASS.OBJECT_OR_COLUMN
        |  2uy -> XPROPERTY_CLASS.PARAMETER
        |  3uy -> XPROPERTY_CLASS.SCHEMA
        |  4uy -> XPROPERTY_CLASS.DATABASE_PRINCIPAL
        |  5uy -> XPROPERTY_CLASS.ASSEMBLY
        |  6uy -> XPROPERTY_CLASS.TYPE
        |  7uy -> XPROPERTY_CLASS.INDEX
        | 10uy -> XPROPERTY_CLASS.XML_SCHEMA_COLLECTION 
        | 15uy -> XPROPERTY_CLASS.MESSAGE_TYPE          
        | 16uy -> XPROPERTY_CLASS.SERVICE_CONTRACT      
        | 17uy -> XPROPERTY_CLASS.SERVICE
        | 18uy -> XPROPERTY_CLASS.REMOTE_SERVICE_BINDING 
        | 19uy -> XPROPERTY_CLASS.ROUTE
        | 20uy -> XPROPERTY_CLASS.DATASPACE
        | 21uy -> XPROPERTY_CLASS.PARTITION_FUNCTION 
        | 22uy -> XPROPERTY_CLASS.DATABASE_FILE 
        | 27uy -> XPROPERTY_CLASS.PLAN_GUIDE 
        | _ -> failwithf "Unknown XPROPERTY_CLASS : %i" classCode

module MS_Description =
    let readAll connection =
        DbTr.readMap
            "SELECT class, class_desc, major_id, minor_id, value FROM sys.extended_properties WHERE name = 'MS_Description'"
            []
            (fun r -> 
                (XPROPERTY_CLASS.findClass (readByte "class" r), readInt32 "major_id" r, readInt32 "minor_id" r),
                readString "value" r)
        |> DbTr.commit_ connection
        |> RCMap.ofMap