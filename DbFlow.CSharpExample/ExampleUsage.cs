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

            var cloneDb = Execute.cloneToLocal(logger, options, schema);
        }
    }
}
