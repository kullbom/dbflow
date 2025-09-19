namespace DbFlow.SqlServer

open System
open System.IO

open Microsoft.Data.SqlClient

open DbFlow

/// Db queries used to create and tear down databases
module private LocalTempDb =
    let create (dbName : string) (dbFile : string) (logFile : string) = 
        let dbFileSpec = $"(NAME={dbName}_db, FILENAME = '{dbFile}')"
        let logFileSpec = $"(NAME={dbName}_log, FILENAME = '{logFile}')"
        DbTr.nonQuery $@"CREATE DATABASE {dbName} ON PRIMARY {dbFileSpec} LOG ON {logFileSpec}" []

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

                FETCH   NEXT
                FROM    KillCursor
                INTO    @Spid

                WHILE   @@FETCH_STATUS = 0
                    BEGIN
                        SET     @ExecSQL = 'KILL ' + CAST(@Spid AS VARCHAR(50))

                        EXEC    (@ExecSQL)

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
    let dbFile = Path.Combine(di.FullName, $"TestDb_{dbName}.mdf")
    let logFile = Path.Combine(di.FullName, $"TestDb_{dbName}_log.ldf")

    let connectionString = cs dbName

    do
        if not di.Exists 
        then di.Create()

        use connection = new SqlConnection(cs "master")
        connection.Open()

        LocalTempDb.create dbName dbFile logFile
        |> DbTr.exe connection

    member _.ConnectionString = connectionString

    interface IDisposable with
        member _.Dispose() =
            use connection = new SqlConnection(cs "master")
            connection.Open()
            // Removing all users before detaching is a huge speed up...
            (fun () -> LocalTempDb.killAllUsers dbName |> DbTr.exe connection)
            |> Logger.logTime logger "Kill all users" ()
            
            (fun () -> LocalTempDb.takeOffline dbName |> DbTr.exe connection)
            |> Logger.logTime logger "Take db offline" ()
            
            (fun () -> LocalTempDb.detach dbName |> DbTr.exe connection)
            |> Logger.logTime logger "Detach db" ()
            
            File.Delete(dbFile)
            File.Delete(logFile)
            ()