namespace DbFlow.SqlServer

open System
open System.IO

open Microsoft.Data.SqlClient

open DbFlow

/// Db queries used to create and tear down databases
module LocalDatabase =
    let create (dbName : string) (file : string) = 
        DbTr.nonQuery $@"CREATE DATABASE {dbName} ON PRIMARY (NAME={dbName}, FILENAME = '{file}')" []

    let takeOffline (dbName : string) = 
        DbTr.nonQuery @$"ALTER DATABASE {dbName} SET OFFLINE WITH ROLLBACK IMMEDIATE" []

    let detach (dbName : string) =
        DbTr.nonQuery @$"EXEC sp_detach_db '{dbName}'" []

    let killAllUsers (dbName : string) =
        DbTr.nonQuery
            """
                DECLARE @Spid INT
                DECLARE @ExecSQL VARCHAR(255)

                DECLARE KillCursor CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY
                FOR
                SELECT  DISTINCT SPID
                FROM    MASTER..SysProcesses
                WHERE   DBID = DB_ID(@dbname)
                  AND   hostprocess > ''

                OPEN    KillCursor

                -- Grab the first SPID
                FETCH   NEXT
                FROM    KillCursor
                INTO    @Spid

                WHILE   @@FETCH_STATUS = 0
                    BEGIN
                        SET     @ExecSQL = 'KILL ' + CAST(@Spid AS VARCHAR(50))

                        EXEC    (@ExecSQL)

                        -- Pull the next SPID
                        FETCH   NEXT 
                        FROM    KillCursor 
                        INTO    @Spid  
                    END

                CLOSE   KillCursor

                DEALLOCATE  KillCursor
            """
            ["@dbname", dbName]


/// Creates a local temporary (random guid name) database
type LocalTempDb(logger) =
    let dbName = "_" + Guid.NewGuid().ToString().Replace('-', '_')
    
    let cs initialCatalog =
        $"Server=(LocalDB)\\mssqllocaldb;Initial Catalog={initialCatalog};Integrated Security=true"

    let path = Path.Combine(Directory.GetCurrentDirectory(), "Data")
    let di = DirectoryInfo(path)
    let file = Path.Combine(di.FullName, $"TestDb_{dbName}.mdf")

    do
        if not di.Exists 
        then di.Create()

        use connection = new SqlConnection(cs "master")
        connection.Open()

        LocalDatabase.create dbName file
        |> DbTr.exe connection

    member _.GetConnectionString() = cs dbName

    interface IDisposable with
        member _.Dispose() =
            use connection = new SqlConnection(cs "master")
            connection.Open()
            // Removing all users before detaching is a huge speed up...
            // Yes - but it doesn't work in async environment - tries to kill all processes
            (fun () -> LocalDatabase.killAllUsers dbName |> DbTr.exe connection)
            |> Logger.logTime logger "kill all users" ()
            
            (fun () -> LocalDatabase.takeOffline dbName |> DbTr.exe connection)
            |> Logger.logTime logger "take db offline" ()
            
            (fun () -> LocalDatabase.detach dbName |> DbTr.exe connection)
            |> Logger.logTime logger "detach db" ()
            
            File.Delete(file)
