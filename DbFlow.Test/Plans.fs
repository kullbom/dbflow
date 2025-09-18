module Xsd
open System
open System.IO
open System.Xml
open System.Xml.Schema
open System.Text
open Microsoft.Data.SqlClient
open DbFlow

let estimatedPlan (sqlQuery : string) : string DbTr =
    DbTr.nonQuery "SET SHOWPLAN_XML ON" []
    |> DbTr.bind
        (fun () -> 
            DbTr.reader sqlQuery [] (fun acc r -> r.GetString(0) :: acc) []
            |> DbTr.map (function [qPlan] -> qPlan | _ -> failwithf "Unexpected result from SHOWPLAN_XML"))
    |> DbTr.bind
        (fun r -> DbTr.nonQuery "SET SHOWPLAN_XML OFF" [] |> DbTr.map (fun () -> r))

(*
let describe_first_result_set (sqlQuery : string) =
    readerSP "sys.sp_describe_first_result_set" [param "tsql" (NCHAR,forSql); param "params" (NCHAR, null); param "browse_information_mode" (1uy) ]
            (fun acc r -> 
                if r.Read<bool> "is_hidden"
                then acc
                else 
                    {
                        column_ordinal = r.Read "column_ordinal"
                        name = r.Read "name"
                        is_nullable = r.Read "is_nullable"
                        collation_name = r.Read "collation_name"
                        DbType = {
                            user_type_id = r.Read  "user_type_id"
                            system_type_id = r.Read "system_type_id"
                            system_type_name = r.Read "system_type_name"
                            max_length = r.Read "max_length"
                            precision = r.Read "precision"
                            scale = r.Read "scale"                        
                        }
                        SourceColumn = 
                            r.Read "source_schema"
                            |> Option.map (fun s -> { Schema = s; Table = r.Read "source_table"; Column = r.Read "source_column" })
                    } :: acc
            )

let describe_undeclared_parameters (sqlQuery : string) =
    readerSP "sys.sp_describe_undeclared_parameters" [param "tsql" (NCHAR,forSql)]
                (fun acc r -> 
                    {
                        Ordinal = r.Read "parameter_ordinal"
                        Name = r.Read "name"
                        DbType = {
                            system_type_id = r.Read "suggested_system_type_id"
                            system_type_name = r.Read "suggested_system_type_name"
                            max_length = r.Read "suggested_max_length"
                            precision = r.Read "suggested_precision"
                            scale = r.Read "suggested_scale"                        
                            user_type_id = r.Read "suggested_user_type_id"
                        }
                        HasDefaultValue = false
                    } :: acc
                )
                []
*)

// Documentation:
// Design: https://dataedo.com/samples/html2/AdventureWorks/#/
// https://medium.com/ai-dev-tips/mermaid-markdown-to-create-er-diagrams-from-a-db-schema-ca109d4db140
// https://kroki.io/
// https://mermaid.js.org/config/schema-docs/config.html#defaultrenderer


// Record to hold type generation context
type TypeContext = {
    Name: string
    IsOptional: bool
    BaseType: string
    Attributes: (string * string) list
    Children: TypeContext list
}

// Map XML schema types to F# types
let mapFSharpType (typeName: string) =
    match typeName.ToLower() with
    | "string" -> "string"
    | "int" -> "int"
    | "unsignedint" -> "uint"
    | "double" -> "float"
    | "boolean" -> "bool"
    | "datetime" -> "DateTime"
    | "decimal" -> "decimal"
    | _ -> typeName // Use the type name directly for custom types

// Generate F# type definition from TypeContext
let generateFSharpType (ctx: TypeContext) =
    let sb = new StringBuilder()
    let typeName = if ctx.IsOptional then $"{ctx.Name} option" else ctx.Name
    sb.AppendLine($"type {typeName} = {{") |> ignore
    
    // Add attributes
    for (attrName, attrType) in ctx.Attributes do
        let fsharpType = mapFSharpType attrType
        sb.AppendLine($"    {attrName}: {fsharpType}") |> ignore
    
    // Add children (nested elements)
    for child in ctx.Children do
        let childType = if child.IsOptional then $"{child.Name} option" else child.Name
        sb.AppendLine($"    {child.Name}: {childType}") |> ignore
    
    sb.AppendLine("}") |> ignore
    sb.ToString()

// Process XmlSchemaComplexType into TypeContext
let rec processComplexType schema (complexType: XmlSchemaComplexType) : TypeContext =
    let name = complexType.Name
    let isOptional = false // Root types are not optional; handle nesting for optionality
    let attributes = 
        [ for item in complexType.AttributeUses.Values do
            match item with
            | :? XmlSchemaAttribute as attr ->
                let attrType = attr.SchemaTypeName.Name |> mapFSharpType
                (attr.Name, attrType)
            | _ -> () ]
        |> List.ofSeq
    let children = 
        match complexType.Particle with
        | :? XmlSchemaSequence as seq ->
            [ for item in seq.Items do
                match item with
                | :? XmlSchemaElement as elem ->
                    let childType = 
                        if not (isNull elem.SchemaTypeName) && not (String.IsNullOrEmpty(elem.SchemaTypeName.Name)) then
                            processComplexTypeOrSimpleType elem.SchemaTypeName schema
                        else
                            { Name = elem.Name; IsOptional = elem.MinOccurs = 0M; BaseType = "obj"; Attributes = []; Children = [] }
                    childType
                | _ -> { Name = "Unknown"; IsOptional = false; BaseType = "obj"; Attributes = []; Children = [] } ]
        | _ -> []
    { Name = name; IsOptional = isOptional; BaseType = "object"; Attributes = attributes; Children = children }

// Handle simple types (e.g., enumerations)
and processComplexTypeOrSimpleType (typeName: XmlQualifiedName) (schema: XmlSchema) : TypeContext =
    let items = schema.Items |> Seq.cast<XmlSchemaObject>
    match items |> Seq.tryFind (fun x -> 
        match x with
        | :? XmlSchemaComplexType as ct -> ct.Name = typeName.Name
        | :? XmlSchemaSimpleType as st -> st.Name = typeName.Name
        | _ -> false) with
    | Some (:? XmlSchemaComplexType as ct) -> processComplexType schema ct
    | Some (:? XmlSchemaSimpleType as st) ->
        match st.Content with
        | :? XmlSchemaSimpleTypeRestriction as restriction ->
            let baseType = restriction.BaseTypeName.Name |> mapFSharpType
            let values = [ for item in restriction.Facets do
                            match item with
                            | :? XmlSchemaEnumerationFacet as facet -> facet.Value
                            | _ -> "" ]
            let unionCases = String.Join(" | ", values |> List.map (fun v -> $"{v} = \"{v}\""))
            { Name = typeName.Name; IsOptional = false; BaseType = baseType; Attributes = []; Children = [] }
        | _ -> { Name = typeName.Name; IsOptional = false; BaseType = "string"; Attributes = []; Children = [] }
    | _ -> { Name = typeName.Name; IsOptional = false; BaseType = "obj"; Attributes = []; Children = [] }

// Generate F# code for the entire schema
let generateFSharpCode (schema: XmlSchema) =
    let sb = new StringBuilder()
    sb.AppendLine("namespace ShowPlanTypes") |> ignore
    sb.AppendLine("open System") |> ignore
    sb.AppendLine() |> ignore
    
    for item in schema.Items do
        match item with
        | :? XmlSchemaComplexType as complexType ->
            sb.Append(generateFSharpType (processComplexType schema complexType)) |> ignore
            sb.AppendLine() |> ignore
        | :? XmlSchemaSimpleType as simpleType ->
            let ctx = processComplexTypeOrSimpleType (XmlQualifiedName(simpleType.Name, schema.TargetNamespace)) schema
            if ctx.Children.IsEmpty && ctx.Attributes.IsEmpty then
                sb.AppendLine($"type {ctx.Name} = {ctx.BaseType}") |> ignore
            sb.AppendLine() |> ignore
        | _ -> ()
    
    sb.ToString()


open Xunit
open Xunit.Abstractions

type ``Sql Query Plans`` (outputHelper:ITestOutputHelper) = 
    
    let testConnStr2 = "<<<<NO...>>>>"
    
    [<Fact>]
    let ``Get plan`` () =
        let myQuery = "SELECT TOP 2 * FROM SignatureArrangements"
        
        let result = 
            use testDbConn = new SqlConnection (testConnStr2)
            testDbConn.Open ()
               
            estimatedPlan myQuery
            |> DbTr.commit_ testDbConn
        ()

    [<Fact>]
    let ``Generate plan code`` () =
        let xsdContent = System.IO.File.ReadAllText "showplanxml.xsd"
        let schema = System.Xml.Schema.XmlSchema.Read(System.Xml.XmlReader.Create(new System.IO.StringReader(xsdContent)), fun sender args -> printfn "Warning: %s" args.Message)

        let fsharpCode = generateFSharpCode schema
        printfn "%s" fsharpCode
        let outputPath = __SOURCE_DIRECTORY__ + "/GeneratedTypes.fs"
        System.IO.File.WriteAllText(outputPath, fsharpCode)