namespace DbFlow.Tests

open Xunit
open Xunit.Abstractions
open Microsoft.Data.SqlClient

open DbFlow
open DbFlow.SqlServer

module Common = 
                
    let testDbFlowRoundtrip logger options sourceSchema sourceScriptFolder destScriptFolder =
        use localDb =
            Logger.infoWithTime "Clone db (start)" logger 
            Execute.cloneToLocal (Logger.decorate (fun m -> $"  {m}") logger) options  
            |> Logger.logTime logger "Clone db" sourceSchema

        let cloneSchema = 
            use connection = new SqlConnection(localDb.ConnectionString)
            connection.Open()
            Execute.readSchema Logger.dummy options
            |> Logger.logTime logger "Load clone" connection

        Execute.generateScriptFiles options cloneSchema
        |> Logger.logTime logger "Generate scripts (of clone)" destScriptFolder 

        Helpers.compareScriptFolders logger sourceScriptFolder 
        |> Logger.logTime logger "Compare scripts (source vs. clone)"destScriptFolder
        
        match Execute.compare sourceSchema cloneSchema with
        | [] -> ()
        | diff -> Assert.Fail (sprintf "Schema is not same (%i differences)" diff.Length)
        
        
    let fullTestSuite logger options rules directory (dbName : string) =
        Helpers.withLocalDbFromScripts logger None (directory + $"{dbName}\\scripts")
            (fun connectionString ->
                let dbFlowOutputDir = directory + $"{dbName}\\dbflow_output"
                let dbFlowOutputDir2 = directory + $"{dbName}\\dbflow_output2"

                // Generate DbFlow Output
                use connection = new SqlConnection(connectionString)
                connection.Open()
                
                let dbSchema = 
                    Execute.readSchema Logger.dummy options
                    |> Logger.logTime logger "Load schema" connection

                Execute.generateScriptFiles options dbSchema 
                |> Logger.logTime logger "Generate scripts" dbFlowOutputDir 

                // Test DbFlow "roundtrip"
                testDbFlowRoundtrip logger options dbSchema dbFlowOutputDir dbFlowOutputDir2

                for (rule : Rule) in rules do
                    match rule.CheckRule dbSchema with
                    | Ok () -> ()
                    | Error e -> Assert.Fail e)

type ``Test suite`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine
    let samplesFolder = __SOURCE_DIRECTORY__ + "\\Samples\\"

    [<Theory>]
    [<InlineData("test_db")>]
    member x.SampleDbs(db : string) = 
        let options = { BypassReferenceChecksOnLoad = false; SkipCompatibilityLevel = true; TypenameFormatter = Options.defaultTypenameFormatter }
        Common.fullTestSuite logger options (Rule.ALL RuleExclusion.none) samplesFolder db

    // AdventureWorks in different versions - the scripts are modified to be compatible with DbUp
    // source: https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver17&tabs=ssms

    [<Theory>]
    [<InlineData("2008r2-lt")>]
    [<InlineData("2012-oltp-lt")>]
    [<InlineData("2014-2022")>]
    member x.AdventureWorks (ver : string) =
        let options = { BypassReferenceChecksOnLoad = false; SkipCompatibilityLevel = true; TypenameFormatter = Options.defaultTypenameFormatter }
        Common.fullTestSuite logger options [] samplesFolder ("adventure-works-" + ver)



module RegressionDirectory =
    let dbflow_regression_directory = 
        let dbflow_regression_directory' = __SOURCE_DIRECTORY__ + "\\..\\..\dbflow-regression\\"
        System.IO.Path.GetFullPath (dbflow_regression_directory')
    
// This test looks for "regression" database definitions in the directory "dbflow-regression" (in the same directory as the repo)
// If such a directory is found it expects all subdirectories of that to represesent a database to test.
// A database directory should contain a folder "scripts" containing scripts that define the database
type ``Regression`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine
    
    static member dbflow_regression_data = 
            if System.IO.Directory.Exists (RegressionDirectory.dbflow_regression_directory)
            then System.IO.Directory.GetDirectories (RegressionDirectory.dbflow_regression_directory)
            else [||]
            |> Seq.choose (fun dir -> 
                let dirName = dir.Substring(dir.LastIndexOf("\\") + 1) 
                if dirName.StartsWith(".")
                then None
                else Some [| dirName |> box |])

    [<Xunit.Theory; Xunit.MemberData("dbflow_regression_data")>]
    member x.``Test suite`` (db : string) = 
        let options = { BypassReferenceChecksOnLoad = false; SkipCompatibilityLevel = true; TypenameFormatter = Options.defaultTypenameFormatter }
        Common.fullTestSuite logger options [] RegressionDirectory.dbflow_regression_directory db


type ``SqlLocalDb_exe`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine

    let cmd' output s =
        let proc = new System.Diagnostics.Process()
        let startInfo = new System.Diagnostics.ProcessStartInfo()
        startInfo.WindowStyle <- System.Diagnostics.ProcessWindowStyle.Hidden
        startInfo.FileName <- "cmd.exe"
        startInfo.Arguments <- "/C " + s
        startInfo.RedirectStandardInput <- true
        startInfo.RedirectStandardOutput <- true
        proc.StartInfo <- startInfo
        if proc.Start ()
        then
            proc.StandardInput.Flush ()
            proc.StandardInput.Close ()
            proc.WaitForExit ()
            proc.StandardOutput.ReadToEnd () |> output
            true
        else
            output (sprintf "ERROR: Could not start %s" s)
            false

    let cmd output f = Printf.kprintf (cmd' output) f

    //[<Fact>]
    member _.``Scripts from SqlLocalDb`` () =
        let dbName = "foobar"
        let dbConnStr = $"Server=(localdb)\{dbName};Integrated Security=true;"
        let outputFolder = __SOURCE_DIRECTORY__ + "\\SqlLocalDbTest\\"

        if cmd outputHelper.WriteLine "SqlLocalDB.exe create \"%s\" -s" dbName 
        then
            let options = Options.Default
            let schema = 
                use dbConn = new Microsoft.Data.SqlClient.SqlConnection (dbConnStr)
                dbConn.Open ()
                SqlServer.Execute.readSchema logger options dbConn

            SqlServer.Execute.generateScriptFiles options schema outputFolder

            if cmd outputHelper.WriteLine "SqlLocalDB.exe stop \"%s\"" dbName 
            then 
                cmd outputHelper.WriteLine "SqlLocalDB.exe delete \"%s\"" dbName
                |> ignore<bool> 
        
        