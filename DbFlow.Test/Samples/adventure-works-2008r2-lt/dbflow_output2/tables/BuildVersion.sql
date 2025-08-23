CREATE TABLE [dbo].[BuildVersion] (
   [SystemInformationID] [TINYINT] NOT NULL
      IDENTITY (1,1),
   [Database Version] [NVARCHAR](25) NOT NULL,
   [VersionDate] [DATETIME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [dbo], N'TABLE', [BuildVersion], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [dbo], N'TABLE', [BuildVersion], N'COLUMN', [VersionDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Version number of the database in 9.yy.mm.dd.00 format.', N'SCHEMA', [dbo], N'TABLE', [BuildVersion], N'COLUMN', [Database Version];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for BuildVersion records.', N'SCHEMA', [dbo], N'TABLE', [BuildVersion], N'COLUMN', [SystemInformationID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Current version number of the AdventureWorksLT 2008R2 sample database. ', N'SCHEMA', [dbo], N'TABLE', [BuildVersion], NULL, NULL;
