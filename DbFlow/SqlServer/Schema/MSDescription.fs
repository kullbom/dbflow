namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/extended-properties-catalog-views-sys-extended-properties?view=sql-server-ver17

[<RequireQualifiedAccess>]
type XPropertyClass = 
    | Database = 0uy
    | ObjectOrColumn = 1uy
    | Parameter = 2uy
    | Schema = 3uy
    | DatabasePrincipal = 4uy
    | Assembly = 5uy
    | Type = 6uy
    | Index = 7uy
    // 8 = User defined table type column
    | XmlSchemaCollection = 10uy
    | MessageType = 15uy
    | ServiceContract = 16uy
    | Service = 17uy
    | RemoteServiceBinding = 18uy
    | Route = 19uy
    | Dataspace = 20uy
    | PartitionFunction = 21uy
    | DatabaseFile = 22uy
    | PlanGuide = 27uy

module XPROPERTY_CLASS = 
    let findClass classCode =
        match classCode with
        |  0uy -> XPropertyClass.Database
        |  1uy -> XPropertyClass.ObjectOrColumn
        |  2uy -> XPropertyClass.Parameter
        |  3uy -> XPropertyClass.Schema
        |  4uy -> XPropertyClass.DatabasePrincipal
        |  5uy -> XPropertyClass.Assembly
        |  6uy -> XPropertyClass.Type
        |  7uy -> XPropertyClass.Index
        | 10uy -> XPropertyClass.XmlSchemaCollection 
        | 15uy -> XPropertyClass.MessageType          
        | 16uy -> XPropertyClass.ServiceContract      
        | 17uy -> XPropertyClass.Service
        | 18uy -> XPropertyClass.RemoteServiceBinding 
        | 19uy -> XPropertyClass.Route
        | 20uy -> XPropertyClass.Dataspace
        | 21uy -> XPropertyClass.PartitionFunction 
        | 22uy -> XPropertyClass.DatabaseFile 
        | 27uy -> XPropertyClass.PlanGuide 
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