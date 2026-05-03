module DbFlow.Test.Plans

open System
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


open Xunit
open DbFlow.SqlServer.ShowPlanXml

type ``Sql Query Plans`` (outputHelper:ITestOutputHelper) = 
    let output fmt = Printf.ksprintf outputHelper.WriteLine fmt

    let samplesFolder = __SOURCE_DIRECTORY__ + "\\PlanSamples\\"

    let testConnStr2 = "Server=t-dbs-scs-01.ia.corp.svea.com; Database=T-SCS-Admittance;Integrated Security=true;Persist Security Info=False; Trusted_Connection=True; Encrypt=false; Connect Timeout = 120;Application Name=AdmittanceApiUnitTest"
    
    let foldQueryPlans f seed (p : Plan) =
        p.BatchSequence.Batches
        |> List.fold 
            (fun acc b ->
                b.Statements
                |> List.fold 
                    (fun acc' s -> 
                        match s with
                        | StmtSimple stmtSimple -> 
                            stmtSimple.QueryPlan
                            |> Option.fold (fun a0 qp -> f a0 qp) acc'
                        | StmtCond _stmtCond -> acc'
                        | StmtCursor _stmtCursor -> acc'
                        | StmtReceive _stmtReceive -> acc'
                        | StmtUseDb _stmtUseDb -> acc'
                        | ExternalDistributedComputation _distributedComputation -> acc')
                    acc)
            seed

    let foldRelOp f seed (ro : RelOpType) =
        let acc = f seed ro
        //ro.
        //let rec loop acc (relOp : RelOp) =
        //    let acc' = f acc relOp
        //    relOp.Children
        //    |> Option.fold (List.fold loop) acc'
        //loop seed qp.RelOp
        acc

    //[<Fact>]
    let ``Populate sample plans`` () =
        let result = 
            use testDbConn = new SqlConnection (testConnStr2)
            testDbConn.Open ()
               
            DbTr.readList 
                "SELECT TOP 10000 
            		q.query_id,
            	    qt.query_sql_text AS QueryText,
            	    p.plan_id,
            	    p.query_plan AS ExecutionPlanXML
            	FROM sys.query_store_plan p
            	JOIN sys.query_store_query q ON q.query_id = p.query_id
            	JOIN sys.query_store_query_text qt ON q.query_text_id = qt.query_text_id"
                []
                (fun r -> 
                    let planId = Readers.readInt64 "plan_id" r
                    let planXml = Readers.readString "ExecutionPlanXML" r
                    planId, planXml)
            |> DbTr.commit_ testDbConn
        for (planId, planXml) in result do
            let fileName = sprintf "%sPlan%05d.xml" samplesFolder planId
            System.IO.File.WriteAllText (fileName, planXml)
            output "Wrote plan %d to file %s" planId fileName
        ()
            
    [<Fact>]
    let ``Get plan`` () =
        let myQuery = "SELECT TOP 2 * FROM [User] ORDER BY [LoginName]"
        
        let result = 
            use testDbConn = new SqlConnection (testConnStr2)
            testDbConn.Open ()
               
            estimatedPlan myQuery
            |> DbTr.commit_ testDbConn
        let plan = DbFlow.SqlServer.ShowPlanXml.Plan.parseString result
        let problems =
            match plan with
            | Error e -> [e]
            | Ok p -> 
                p
                |> foldQueryPlans 
                    (fun acc qp -> 
                        let warnings = 
                            qp.Warnings 
                            |> Option.fold 
                                (fun acc' ws -> 
                                    //ws.PlanAffectingConvert
                                    //|>List.fold (fun acc'' pac -> $"{pac.ConvertIssue} - {pac.Expression}" :: acc'') 
                                    acc')
                                acc
                        qp.RelOp
                        |> foldRelOp
                            (fun acc' ro ->
                                // Leta efter IndexScanType som saknar SeekPredicates - det borde rimligen vara en "full table/index scan"
                                //ro
                                acc')
                            warnings)
                    []
        ()

    
    [<Fact>]
    let ``Parse plan`` () =
        let sw = System.Diagnostics.Stopwatch ()
        sw.Start ()
        let plan01 = System.IO.File.ReadAllText (samplesFolder + "Plan01.xml")
        let timeRead = sw.ElapsedMilliseconds 
        sw.Restart ()
        let doc = Xml.Linq.XDocument.Parse plan01
        let timeXmlParse = sw.ElapsedMilliseconds 
        sw.Restart ()
       
        let mutable plans = [] 
        for i = 0 to 10000 do
            let plan = DbFlow.SqlServer.ShowPlanXml.Plan.parseXDocument doc
            plans <- plan :: plans
            ()

        let timePlanParse = sw.ElapsedMilliseconds 
        
        output "Time to read file: %d ms" timeRead
        output "Time to parse XML: %d ms" timeXmlParse
        output "Time to parse plan: %d ms" timePlanParse

        ()

    // {(C:\Projects\dbflow\DbFlow.Test\PlanSamples\Plan10702.xml, Expected valid StmtType but got 'Statements')}

    [<Fact>]
    let ``Parse all sample plans`` () =
        let sw = System.Diagnostics.Stopwatch ()
        sw.Start ()
        let files = System.IO.Directory.GetFiles(samplesFolder, "*.xml") |> List.ofArray
        output "%d samples found" files.Length
        let timeReadDir = sw.ElapsedMilliseconds 
        sw.Restart ()
        
        let errors =
            files
            |> List.fold
                (fun acc file -> 
                    let planXml = System.IO.File.ReadAllText file
                    let plan = DbFlow.SqlServer.ShowPlanXml.Plan.parseString planXml
                    match plan with 
                    | Ok _ -> acc 
                    | Error e -> (file, e) :: acc)
                []
        
        let timePlanParse = sw.ElapsedMilliseconds 
        
        output "Time to read file: %d ms" timeReadDir
        output "Time to parse plan: %d ms" timePlanParse

        output "Errors: %d / %d" errors.Length files.Length

        Assert.Equal (0, errors.Length)

        ()