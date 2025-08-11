CREATE TABLE [dbo].[DatabaseLog] (
   [DatabaseLogID] [int] NOT NULL
      IDENTITY (1,1),
   [PostTime] [datetime] NOT NULL,
   [DatabaseUser] [nvarchar](128) NOT NULL,
   [Event] [nvarchar](128) NOT NULL,
   [Schema] [nvarchar](128) NULL,
   [Object] [nvarchar](128) NULL,
   [TSQL] [nvarchar](max) NOT NULL,
   [XmlEvent] [xml] NOT NULL

   ,CONSTRAINT [PK_DatabaseLog_DatabaseLogID] PRIMARY KEY NONCLUSTERED ([DatabaseLogID])
)


GO
