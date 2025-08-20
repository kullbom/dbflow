CREATE TABLE [dbo].[BuildVersion] (
   [SystemInformationID] [TINYINT] NOT NULL
      IDENTITY (1,1),
   [Database Version] [NVARCHAR](25) NOT NULL,
   [VersionDate] [DATETIME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
)


GO
