namespace DbFlow.Tests

open Xunit

open DbFlow
open DbFlow.SqlServer

open Microsoft.Data.SqlClient
open Xunit.Abstractions


type Tests (outputHelper : ITestOutputHelper) = 
    [<Fact>]
    let ``Copy data`` () =
        let silentLogger = Logger.create ignore
        let logger = Logger.create outputHelper.WriteLine
        let options = Options.Default 
    
        let dbFolder = __SOURCE_DIRECTORY__ + "\\Samples\\adventure-works-2012-oltp-lt\\scripts"

        Helpers.withLocalDbFromScripts logger dbFolder
            (fun testConnStr ->
                let dbSchema =
                    use testDbConn = new SqlConnection (testConnStr)
                    testDbConn.Open ()
                    Execute.readSchema silentLogger options 
                    |> Logger.logTime logger "read schema" testDbConn 
    
                let dataRef = 
                    fun () -> 
                        use testDbConn = new SqlConnection (testConnStr)
                        testDbConn.Open ()
                        
                        CopyData.TopN (dbSchema.Tables |> List.find (fun t -> t.Name = "Product")) 300 
                        |> DbTr.commit_ testDbConn
                    |> Logger.logTime logger "TopN" ()

                use localDb = new LocalTempDb(silentLogger)
                use localDbConn = new SqlConnection (localDb.ConnectionString)
                localDbConn.Open ()
                
                Execute.clone silentLogger options dbSchema
                |> Logger.logTime logger "clone schema" localDbConn
    
                let () =
                    use testDbConn = new SqlConnection (testConnStr)
                    testDbConn.Open ()
                    
                    CopyData.copyData logger dbSchema dataRef CopyData.CopyMethod.InsertCopy testDbConn localDbConn
                    CopyData.copyData logger dbSchema dataRef CopyData.CopyMethod.UpsertCopy testDbConn localDbConn
                ())
    
        ()