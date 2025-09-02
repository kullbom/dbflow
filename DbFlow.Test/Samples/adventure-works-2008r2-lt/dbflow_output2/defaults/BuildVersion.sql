ALTER TABLE [dbo].[BuildVersion] ADD CONSTRAINT [DF_BuildVersion_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [dbo], N'TABLE', [BuildVersion], N'CONSTRAINT', [DF_BuildVersion_ModifiedDate];
GO
