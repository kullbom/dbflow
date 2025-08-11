CREATE TABLE [dbo].[AWBuildVersion] (
   [SystemInformationID] [TINYINT] NOT NULL
      IDENTITY (1,1),
   [Database Version] [NVARCHAR](25) NOT NULL,
   [VersionDate] [DATETIME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_AWBuildVersion_SystemInformationID] PRIMARY KEY CLUSTERED ([SystemInformationID])
)


GO
