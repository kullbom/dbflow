CREATE TABLE [dbo].[AWBuildVersion] (
   [SystemInformationID] [tinyint] NOT NULL
      IDENTITY (1,1),
   [Database Version] [nvarchar](25) NOT NULL,
   [VersionDate] [datetime] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_AWBuildVersion_SystemInformationID] PRIMARY KEY CLUSTERED ([SystemInformationID])
)


GO
