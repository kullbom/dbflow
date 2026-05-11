module DbFlow.Tests.Plans

open Microsoft.Data.SqlClient
open DbFlow

open Xunit
open DbFlow.SqlServer.Experimental
open DbFlow.SqlServer.Experimental.ShowPlanXml

type ``Sql Query Plans`` (outputHelper:ITestOutputHelper) = 
    let output fmt = Printf.ksprintf outputHelper.WriteLine fmt

    let localSamplesFolder = __SOURCE_DIRECTORY__ + "\\PlanSamples\\"

    let testConnStr2 = ""
    
    //[<Fact>]
    let ``Populate sample plans`` () =
        let result = 
            use testDbConn = new SqlConnection (testConnStr2)
            testDbConn.Open ()
               
            DbTr.readList 
                "SELECT TOP 100000 
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
            let fileName = sprintf "%sXyz_Plan%05d.xml" localSamplesFolder planId
            System.IO.File.WriteAllText (fileName, planXml)
            output "Wrote plan %d to file %s" planId fileName
        ()
            
    let testPlanIssues (logger : Logger) localDbConn expectedIssues sqlQuery =
        let result = 
            QueryMeta.Sys.estimatedPlan sqlQuery
            |> DbTr.tryCommit_ localDbConn
        match result with
        | Ok r -> 
            let plan = Plan.parseString r
            let problems =
                match plan with
                | Error e -> [e]
                | Ok p -> 
                    Abstractions.foldQueryPlanIssues (fun acc qp i -> Abstractions.issueString qp i :: acc) [] p
                |> List.sort
            Assert.StrictEqual (expectedIssues, problems)
            ()
        | Error e -> 
            Assert.Fail (e.Message)

    [<Fact>]
    let ``Get plans [SalesLT].[Customer]`` () =
        let logger = Logger.create outputHelper.WriteLine
        let localDbOptions = LocalDbOptions.Default
        let samplesFolder = __SOURCE_DIRECTORY__ + "\\Samples\\"
        let dbName = "adventure-works-2012-oltp-lt"
        Helpers.withLocalDbFromScripts logger localDbOptions None (samplesFolder + $"{dbName}\\scripts")
            (fun connectionString ->
                use localDbConn = new SqlConnection (connectionString)
                localDbConn.Open ()

                // Should allow for "early exit" of the scan operator, so no issue expected
                testPlanIssues logger localDbConn []
                    "SELECT TOP 2 * FROM [SalesLT].[Customer] ORDER BY [CustomerID]"

                testPlanIssues logger localDbConn ["ScanWithoutSeekPredicate"]
                    "SELECT * FROM [SalesLT].[Customer] ORDER BY [CustomerID]"

                testPlanIssues logger localDbConn ["ScanWithoutSeekPredicate"]
                    "SELECT * FROM [SalesLT].[Customer]"
                    
                // There is an index on EmailAddress
                testPlanIssues logger localDbConn [] 
                    "SELECT [EmailAddress] FROM [SalesLT].[Customer] WHERE [EmailAddress] = 'foobar'"
                testPlanIssues logger localDbConn []
                    "SELECT TOP 2 [EmailAddress] FROM [SalesLT].[Customer] WHERE [EmailAddress] = 'foobar'"
                // ... but since there is no data the optimizer will choose a full scan of the PK instead of the index when other columns are selected
                testPlanIssues logger localDbConn ["ScanWithoutSeekPredicate"] 
                    "SELECT [EmailAddress], [Phone] FROM [SalesLT].[Customer] WHERE [EmailAddress] = 'foobar'"
                

                // Without the index hint the optimizer will choose a full scan of the PK because there is no data 
                testPlanIssues logger localDbConn []
                    "SELECT TOP 2 * FROM [SalesLT].[Customer] WITH (INDEX = [IX_Customer_EmailAddress]) WHERE [EmailAddress] = 'foobar'"

                testPlanIssues logger localDbConn ["ScanWithoutSeekPredicate"]
                    "SELECT TOP 2 * FROM [SalesLT].[Customer] WHERE [EmailAddress] = 'foobar'"
                    
                // There is no index on Phone, so a full scan with no seek predicate is expected
                testPlanIssues logger localDbConn ["ScanWithoutSeekPredicate"]
                    "SELECT TOP 2 * FROM [SalesLT].[Customer] WHERE [Phone] = 'foobar'"

                testPlanIssues logger localDbConn ["ScanWithoutSeekPredicate"]
                    "SELECT TOP 2 * FROM [SalesLT].[Customer] ORDER BY [Phone]"
                )

    [<Fact>]
    let ``Get plans [SalesLT].[ProductModel]`` () =
        let logger = Logger.create outputHelper.WriteLine
        let localDbOptions = LocalDbOptions.Default
        let samplesFolder = __SOURCE_DIRECTORY__ + "\\Samples\\"
        let dbName = "adventure-works-2012-oltp-lt"
        Helpers.withLocalDbFromScripts logger localDbOptions None (samplesFolder + $"{dbName}\\scripts")
            (fun connectionString ->
                use localDbConn = new SqlConnection (connectionString)
                localDbConn.Open ()

                
                testPlanIssues logger localDbConn []
                    "SELECT [Name] FROM [SalesLT].[ProductModel] WHERE [ProductModelID] = 78"

                testPlanIssues logger localDbConn []
                    "SELECT [Name] FROM [SalesLT].[ProductModel] WHERE [Name] = 'foobar'"

                testPlanIssues logger localDbConn []
                    "SELECT [rowguid] FROM [SalesLT].[ProductModel] WHERE [rowguid] = 'foobar'"
                    
                )


    let parseSamplePlans samplesFolder filter =
        let sw = System.Diagnostics.Stopwatch ()
        sw.Start ()
        let files = 
            System.IO.Directory.GetFiles(samplesFolder, filter) |> List.ofArray
            |> List.map (fun f -> f, System.IO.File.ReadAllText f)
        output "%d samples found" files.Length
        let timeReadDir = sw.ElapsedMilliseconds 
        sw.Restart ()
        
        let plans =
            files
            |> List.map
                (fun (file, planXml) -> 
                    let plan = Plan.parseString planXml
                    file, planXml, plan)
        
        let timePlanParse = sw.ElapsedMilliseconds 

        let errors =
            plans
            |> List.fold
                (fun acc (file, planXml, plan) -> 
                    match plan with 
                    | Ok _ -> acc 
                    | Error e -> (file, e) :: acc)
                []
        
        let issues =
            plans
            |> List.fold
                (fun acc (file, planXml, plan) -> 
                    match plan with 
                    | Ok p -> Abstractions.foldQueryPlanIssues (fun acc qp i -> (file, Abstractions.issueString qp i) :: acc) acc p
                    | Error _ -> acc)
                []
        
        
        output "Time to read files: %d ms" timeReadDir
        output "Time to parse plans: %d ms" timePlanParse

        output "Errors: %d / %d" errors.Length files.Length
        output "Issues: %d" issues.Length

        Assert.Equal (0, errors.Length)

        ()

    //[<Fact>]
    let ``Parse specific local sample plan`` () =
        parseSamplePlans localSamplesFolder "Xyz_Plan00081.xml"

    [<Fact>]
    let ``Parse all local sample plans`` () =
        parseSamplePlans localSamplesFolder "*.xml"

    [<Fact>]
    let ``Parse all regression sample plans`` () =
        let regressionSamplePlans = __SOURCE_DIRECTORY__ + "\\..\\..\dbflow-regression\\ShowPlanXmlSamples\\"
        parseSamplePlans localSamplesFolder "*.xml"
