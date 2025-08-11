CREATE TABLE [dbo].[DatabaseLog] (
   [DatabaseLogID] [INT] NOT NULL
      IDENTITY (1,1),
   [PostTime] [DATETIME] NOT NULL,
   [DatabaseUser] [SYSNAME] NOT NULL,
   [Event] [SYSNAME] NOT NULL,
   [Schema] [SYSNAME] NULL,
   [Object] [SYSNAME] NULL,
   [TSQL] [NVARCHAR](MAX) NOT NULL,
   [XmlEvent] [XML] NOT NULL

   ,CONSTRAINT [PK_DatabaseLog_DatabaseLogID] PRIMARY KEY NONCLUSTERED ([DatabaseLogID])
)


GO
