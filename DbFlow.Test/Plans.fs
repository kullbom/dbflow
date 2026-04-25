module DbFlow.Test.Plans

open System
open System.IO
open System.Xml
open System.Xml.Schema
open System.Text
open Microsoft.Data.SqlClient
open DbFlow
open DbFlow.Test

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

    // let xsdContent = System.IO.File.ReadAllText "showplanxml.xsd"
    // let schema = System.Xml.Schema.XmlSchema.Read(System.Xml.XmlReader.Create(new System.IO.StringReader(xsdContent)), fun sender args -> printfn "Warning: %s" args.Message)

    [<Fact>]
    let ``Parse plan`` () =
        let plan01 = System.IO.File.ReadAllText "Plan01.xml"
        let doc = Xml.Linq.XDocument.Parse plan01
        let plan = PlanParser.parseShowPlanXML doc.Root
        ()