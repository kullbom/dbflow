namespace DbFlow.CSharpExample
{
    using DbFlow;
    using DbFlow.SqlServer;
    using DbFlow.SqlServer.Schema;

    public class ExampleUsage
    {
        static void CloneDb (string sourceConnectionString)
        {
            // I don't want "LoggerModule" here
            var logger = LoggerModule.fromFunc(s => { });
            
            // I'd like to be able to do this in one call
            using var sourceConnection = new Microsoft.Data.SqlClient.SqlConnection(sourceConnectionString);
            sourceConnection.Open();
            var schema = Execute.readSchema(logger, ReadOptions.Default, sourceConnection);

            var cloneDb = Execute.cloneToLocal(logger, LocalDbOptions.Default, ScriptOptions.Default, schema);
        }

        static void GenerateScripts(string sourceConnectionString, string destinationDirectory)
        {
            // I don't want "LoggerModule" here
            var logger = LoggerModule.fromFunc(s => { });
            
            // I'd like to be able to do this in one call
            using var sourceConnection = new Microsoft.Data.SqlClient.SqlConnection(sourceConnectionString);
            sourceConnection.Open();
            var schema = Execute.readSchema(logger, ReadOptions.Default, sourceConnection);

            Execute.generateScriptFiles(ScriptOptions.Default, schema, destinationDirectory);
        }

        
    }
}
