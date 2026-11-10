namespace DbFlow.SqlServer

open System
open System.IO

open Microsoft.Data.SqlClient

open DbFlow

type LocalDb = {
    DBName : string
    DbFile : string
    LogFile : string

    SchemaVersionHash : string option

    Options : LocalDbOptions
}

module LocalDb =
    let private create (dbName : string) (dbFile : string) (logFile : string) = 
        let dbFileSpec = $"(NAME={dbName}_db, FILENAME = '{dbFile}')"
        let logFileSpec = $"(NAME={dbName}_log, FILENAME = '{logFile}')"
        DbTr.nonQuery $@"CREATE DATABASE {dbName} ON PRIMARY {dbFileSpec} LOG ON {logFileSpec}" []

    let private takeOffline (dbName : string) = 
        DbTr.nonQuery @$"ALTER DATABASE {dbName} SET OFFLINE WITH ROLLBACK IMMEDIATE" []

    let private detach (dbName : string) =
        DbTr.nonQuery @$"EXEC sp_detach_db '{dbName}'" []

    let private killAllUsers (dbName : string) =
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
            [("@dbname", dbName)]

    let dbExists (dbName : string) =
        DbTr.readSingle
            "SELECT COUNT(1) X FROM sys.databases WHERE name = @dbname"
            [("@dbname", dbName)]
            (Readers.readInt32 "X")
        |> DbTr.bind 
            (function 
            | 1 -> 
                DbTr.zip
                    (DbTr.readSingle
                        """
                            SELECT physical_name
                            FROM sys.master_files
                            WHERE database_id = DB_ID(@dbname) AND type = 0
                        """
                        [("@dbname", dbName)]
                        (Readers.readString "physical_name"))
                    (DbTr.readSingle
                        """
                            SELECT physical_name
                            FROM sys.master_files
                            WHERE database_id = DB_ID(@dbname) AND type = 1
                        """
                        [("@dbname", dbName)]
                        (Readers.readString "physical_name"))
                |> DbTr.map Some 
            | _ -> DbTr.ret None)

    let tryGetDatabaseExtendedProperty (propertyName: string) : string option DbTr =
        DbTr.readList 
            """
                SELECT CAST(value AS nvarchar(max)) V
                FROM sys.extended_properties
                WHERE class = 0 AND name = @name
            """
            [("@name", box propertyName)]
            (Readers.readString "V")
        |> DbTr.map List.tryHead

    let setDatabaseExtendedProperty (propertyName: string) (value: string) : unit DbTr =
        DbTr.nonQuery 
            """
                IF EXISTS (SELECT 1 FROM sys.extended_properties WHERE class = 0 AND name = @name)
                    EXEC sys.sp_updateextendedproperty @name = @name, @value = @value;
                ELSE
                    EXEC sys.sp_addextendedproperty @name = @name, @value = @value;
            """
            [
                "@name", propertyName
                "@value", value
            ]

    let createDb logger dbName (options : LocalDbOptions) =
        let path = options.DbFolderPath()
        let di = DirectoryInfo(path)
        let dbFile = Path.Combine(di.FullName, $"{dbName}.mdf")
        let logFile = Path.Combine(di.FullName, $"{dbName}_log.ldf")

        let connectionString = options.ConnectionString dbName

        if not di.Exists 
        then di.Create()

        use connection = new SqlConnection(options.ConnectionString "master")
        connection.Open()

        create dbName dbFile logFile
        |> DbTr.exe connection

        {
            DBName = dbName
            DbFile = dbFile
            LogFile = logFile
            SchemaVersionHash = None
            Options = options
        }

    let killDb logger (db : LocalDb) =
        use connection = new SqlConnection(db.Options.ConnectionString "master")
        connection.Open()
        // Removing all users before detaching is a huge speed up...
        (fun () -> killAllUsers db.DBName |> DbTr.exe connection)
        |> Logger.logTime logger "Kill all users" ()
        
        (fun () -> takeOffline db.DBName |> DbTr.exe connection)
        |> Logger.logTime logger "Take db offline" ()
        
        (fun () -> detach db.DBName |> DbTr.exe connection)
        |> Logger.logTime logger "Detach db" ()
        
        File.Delete(db.DbFile)
        File.Delete(db.LogFile)

    let getDb (dbName : string) (options : LocalDbOptions) : LocalDb option =
        (use conn = new SqlConnection(options.ConnectionString "master") 
         conn.Open()
         dbExists dbName 
         |> DbTr.exe conn)
        |> Option.map 
            (fun (dbFile, logFile) ->
                {
                    DBName = dbName
                    DbFile = dbFile
                    LogFile = logFile
                    SchemaVersionHash = 
                        use conn = new SqlConnection(options.ConnectionString dbName) 
                        conn.Open()
                        tryGetDatabaseExtendedProperty "DbFlow_SchemaVersionHash" 
                        |> DbTr.exe conn
                    Options = options
                })

    let getDbHash (scripts : string list) : string =
        scripts
        |> String.concat "\n"
        |> System.Text.Encoding.UTF8.GetBytes 
        |> System.Security.Cryptography.MD5.HashData 
        |> System.Convert.ToHexString

/// Creates a local temporary (random guid name) database
type LocalTempDb(logger, options : LocalDbOptions) =
    let dbName = "dbflow_" + Guid.NewGuid().ToString().Replace('-', '_')
    
    let db = LocalDb.createDb logger dbName options 

    member _.ConnectionString = options.ConnectionString db.DBName

    interface IDisposable with
        member _.Dispose() = LocalDb.killDb logger db