namespace DbFlow.CSharpExample
{
    using DbFlow;
    using DbFlow.SqlServer;
    public class ExampleUsage
    {
        static void CloneDb (string sourceConnectionString)
        {
            // I don't want "LoggerModule" here
            var logger = LoggerModule.fromFunc(s => { });
            // I don't want "OptionsModule" here
            var options = OptionsModule.Default;

            // I'd like to be able to do this in one call
            using var sourceConnection = new Microsoft.Data.SqlClient.SqlConnection(sourceConnectionString);
            sourceConnection.Open();
            var schema = Execute.readSchema(logger, options, sourceConnection);

            // I'd like to be able to do this in one call - that returns a localDb
            using var localDb = new LocalTempDb(logger);
            using var targetConnection = new Microsoft.Data.SqlClient.SqlConnection(localDb.ConnectionString);
            targetConnection.Open();
            Execute.clone(logger, options, schema, targetConnection);
        }
    }
}
