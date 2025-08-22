namespace DbFlow.Tests

open Xunit
open Xunit.Abstractions
open Microsoft.Data.SqlClient

open MyDbUp

open DbFlow
open DbFlow.SqlServer

module Common = 
                
    let testDbFlowRoundtrip logger options sourceSchema sourceScriptFolder destScriptFolder =
        use localDb =
            Execute.cloneToLocal (Logger.decorate (fun m -> $"  {m}") logger) options  
            |> Logger.logTime logger "DbFlow - clone db" sourceSchema

        let cloneSchema = 
            use connection = new SqlConnection(localDb.ConnectionString)
            connection.Open()
            Execute.readSchema Logger.dummy options
            |> Logger.logTime logger "DbFlow - load clone" connection

        Execute.generateScriptFiles options cloneSchema
        |> Logger.logTime logger "DbFlow - generate scripts (of clone)" destScriptFolder 

        Helpers.compareScriptFolders logger sourceScriptFolder 
        |> Logger.logTime logger "Compare scripts (source vs. clone)"destScriptFolder
        
        match Execute.compare sourceSchema cloneSchema with
        | [] -> ()
        | diff -> Assert.Fail (sprintf "Schema is not same (%i differences)" diff.Length)
        
        
    let fullTestSuite logger options rules directory (dbName : string) =
        Helpers.withLocalDbFromScripts logger (directory + $"{dbName}\\scripts")
            (fun connectionString ->
                let dbFlowOutputDir = directory + $"{dbName}\\dbflow_output"
                let dbFlowOutputDir2 = directory + $"{dbName}\\dbflow_output2"

                // Generate DbFlow Output
                use connection = new SqlConnection(connectionString)
                connection.Open()
                
                let dbSchema = 
                    Execute.readSchema Logger.dummy options
                    |> Logger.logTime logger "DbFlow - load model" connection

                Execute.generateScriptFiles options dbSchema 
                |> Logger.logTime logger "DbFlow - generate scripts" dbFlowOutputDir 

                // Test DbFlow "roundtrip"
                testDbFlowRoundtrip logger options dbSchema dbFlowOutputDir dbFlowOutputDir2

                for (rule : Rule) in rules do
                    match rule.CheckRule dbSchema with
                    | Ok () -> ()
                    | Error e -> Assert.Fail e)

    let testSchemazenCompatibility logger (directory : string) (dbName : string) =
        Helpers.withLocalDbFromScripts logger (directory + $"{dbName}\\scripts")
            (fun connectionString ->
                let schemazenOutputDir = directory + $"{dbName}\\schemazen_output"
                let dbFlowOutputDir = directory + $"{dbName}\\dbflow_output_schemazen_comp"

                // Generate Schemazen Scripts 
                let schemaZenDb = SchemaZen.Library.Models.Database(Connection = connectionString, Dir = schemazenOutputDir)
                schemaZenDb.Load |> Logger.logTime logger "Schemazen - load" ()
                schemaZenDb.ScriptToDir |> Logger.logTime logger "Schemazen - generateScripts" ()
                
                // Generate DbFlow Output
                use connection = new SqlConnection(connectionString)
                connection.Open()
                
                let options = { Options.SchemazenCompatibility = true; BypassReferenceChecksOnLoad = false }
                let dbSchema = 
                    Execute.readSchema Logger.dummy options
                    |> Logger.logTime logger "DbFlow - load model" connection

                Execute.generateScriptFiles options dbSchema 
                |> Logger.logTime logger "DbFlow - generate scripts" dbFlowOutputDir

                // Compare the output from Schemazen and DbFlow
                Helpers.compareScriptFolders logger schemazenOutputDir dbFlowOutputDir)

    let testSchemazenRoundtrip logger directory (dbName : string) =
        Helpers.withLocalDbFromScripts logger (directory + $"{dbName}\\scripts")
            (fun connectionStr ->
                let schemazenOutputDir = directory + $"{dbName}\\schemazen_output"
                let schemazenOutputDir2 = directory + $"{dbName}\\schemazen_output2"

                // Generate Schemazen Scripts 
                let schemaZenDb = SchemaZen.Library.Models.Database(Connection = connectionStr, Dir = schemazenOutputDir)
                schemaZenDb.Load |> Logger.logTime logger "Schemazen - load" ()
                schemaZenDb.ScriptToDir |> Logger.logTime logger "Schemazen - generateScripts" ()
                
                let schemaZenDb2 = 
                    SchemaZen.Library.Models.Database(Connection = connectionStr, Dir = schemazenOutputDir2)
                
                //(fun () -> 
                //    schemaZenDb2.CreateFromDir(true, 
                //        databaseFilesPath = schemazenOutputDir, 
                //        log = (fun _ s -> logger s)))
                //|> Logger.logTime logger "Schemazen - load copy" ()
                Helpers.withLocalDb logger 
                    (fun connectionStr2 ->
                        Helpers.runSchemazenGeneratedScripts logger connectionStr2 
                        |> Logger.logTime logger "Schemazen - execute generated scripts" schemazenOutputDir
                
                        schemaZenDb2.Load |> Logger.logTime logger "Schemazen - load copy" ())
                
                schemaZenDb2.ScriptToDir |> Logger.logTime logger "Schemazen - generateScripts from copy" ()
                
                Helpers.compareScriptFolders logger schemazenOutputDir schemazenOutputDir2
                )
        

type ``Test suite`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine
    let samplesFolder = __SOURCE_DIRECTORY__ + "\\Samples\\"

    [<Theory>]
    [<InlineData("test_db")>]
    member x.SampleDbs(db : string) = 
        let options = { Options.SchemazenCompatibility = false; BypassReferenceChecksOnLoad = false }
        Common.fullTestSuite logger options (Rule.ALL RuleExclusion.none) samplesFolder db

    // AdventureWorks in different versions - the scripts are modified to be compatible with DbUp
    // source: https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver17&tabs=ssms

    [<Theory>]
    [<InlineData("2008r2-lt")>]
    [<InlineData("2012-oltp-lt")>]
    [<InlineData("2014-2022")>]
    member x.AdventureWorks (ver : string) =
        let options = { Options.SchemazenCompatibility = false; BypassReferenceChecksOnLoad = false }
        Common.fullTestSuite logger options [] samplesFolder ("adventure-works-" + ver)



type ``Schemazen compatibility`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine
    let samplesFolder = __SOURCE_DIRECTORY__ + "\\Samples\\"

    [<Theory>]
    [<InlineData("test_db")>]
    member x.SampleDbs (db : string) = 
        Common.testSchemazenCompatibility logger samplesFolder db

    // AdventureWorks in different versions - the scripts are modified to be compatible with DbUp
    // source: https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver17&tabs=ssms
    [<Theory>]
    [<InlineData("2008r2-lt")>]
    [<InlineData("2012-oltp-lt")>]
    [<InlineData("2014-2022")>]
    member x.AdventureWorks (ver : string) = 
        Common.testSchemazenCompatibility logger samplesFolder ("adventure-works-" + ver)

    

type ``Schemazen roundtrip`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine
    let samplesFolder = __SOURCE_DIRECTORY__ + "\\Samples\\"
    
    [<Theory>]
    [<InlineData("test_db")>]
    member x.SampleDbs (db : string) = 
        Common.testSchemazenRoundtrip logger samplesFolder db

    
type ``Schemazen roundtrip - failure expected`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine

    // AdventureWorks in different versions - the scripts are modified to be compatible with DbUp
    // source: https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver17&tabs=ssms
    [<Theory>]
    [<InlineData("2008r2-lt")>]     
    [<InlineData("2012-oltp-lt")>]  
    [<InlineData("2014-2022")>]     
    member x.AdventureWorks (ver : string) =
        Assert.ThrowsAny<exn>(
            fun () -> Common.testSchemazenRoundtrip logger (__SOURCE_DIRECTORY__ + "\\Samples\\") ("adventure-works-" + ver)) |> ignore
        
// This test looks for "regression" database definitions in the directory "dbflow-regression" (in the same directory as the repo)
// If such a directory is found it expects all subdirectories of that to represesent a database to test.
// A database directory should contain a folder "scripts" containing scripts that define the database
type ``Regression`` (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine
    
    static member dbflow_regression_data = 
            let dbflow_regression_directory' = __SOURCE_DIRECTORY__ + "\\..\\..\dbflow-regression\\"
            let dbflow_regression_directory = System.IO.Path.GetFullPath (dbflow_regression_directory')
            if System.IO.Directory.Exists (dbflow_regression_directory)
            then System.IO.Directory.GetDirectories (dbflow_regression_directory)
            else [||]
            |> Seq.choose (fun dir -> 
                let dirName = dir.Substring(dir.LastIndexOf("\\") + 1) 
                if dirName.StartsWith(".")
                then None
                else Some [| dirName |> box; dbflow_regression_directory |> box |])

    [<Xunit.Theory; Xunit.MemberData("dbflow_regression_data")>]
    member x.``Schemazen comp`` (db : string, directory : string) = 
        Common.testSchemazenCompatibility logger directory db

    [<Xunit.Theory; Xunit.MemberData("dbflow_regression_data")>]
    member x.``Schemazen Roundtrip`` (db : string, directory : string) = 
        Common.testSchemazenRoundtrip logger directory db

    [<Xunit.Theory; Xunit.MemberData("dbflow_regression_data")>]
    member x.``Test suite`` (db : string, directory : string) = 
        let options = { Options.SchemazenCompatibility = false; BypassReferenceChecksOnLoad = false }
        Common.fullTestSuite logger options [] directory db
