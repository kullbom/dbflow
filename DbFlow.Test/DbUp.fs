module MyDbUp.Db

open DbUp

let createDbupConfiguration (connectionString:string) path (minutesTimeout : int) output =
    let config =
        DeployChanges.To.SqlDatabase(connectionString)
            .WithTransaction()
            .WithScriptsFromFileSystem(path, ScriptProviders.FileSystemScriptOptions(IncludeSubDirectories = true))
            //.WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly(), (fun x -> x.StartsWith(scriptPrefixFilter)))
            .WithExecutionTimeout(System.TimeSpan.FromMinutes(float minutesTimeout))
            .LogTo({ new Engine.Output.IUpgradeLog with
                        override x.LogError (format,args) = output (System.String.Format(format, args))
                        override x.LogError (e,format,args) = output (System.String.Format(format, e, args))
                        override x.LogInformation (format,args) = output (System.String.Format(format, args))
                        override x.LogWarning (format,args) = output (System.String.Format(format, args))
                        override x.LogTrace (format,args) = output (System.String.Format(format, args))
                        override x.LogDebug (format,args) = output (System.String.Format(format, args))
                    })
            .LogToConsole()
            
    config.Configure(fun c -> 
        let s = 
            DbUp.SqlServer.SqlTableJournal(
                (fun () -> c.ConnectionManager), 
                (fun () -> c.Log), 
                "dbo", "SchemaVersions"  )
        c.Journal <- s
        ()    
    )

    config

let performDbUpgrade (config : Builder.UpgradeEngineBuilder) output =
    let upgrader = config.Build()

    let result = upgrader.PerformUpgrade ()

    let (message, consoleColor) = 
        if result.Successful then
            "Success!", System.ConsoleColor.Green
        else
            result.Error.ToString(), System.ConsoleColor.Red
    System.Console.ForegroundColor <- consoleColor
    System.Console.WriteLine (message)
    output message
    System.Console.ResetColor ()
    result.Successful
