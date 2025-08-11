CREATE TABLE [dbo].[BuildVersion] (
   [SystemInformationID] [tinyint] NOT NULL
      IDENTITY (1,1),
   [Database Version] [nvarchar](25) NOT NULL,
   [VersionDate] [datetime] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL
)


GO
