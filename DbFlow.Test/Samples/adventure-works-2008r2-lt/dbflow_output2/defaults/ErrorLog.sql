ALTER TABLE [dbo].[ErrorLog] ADD CONSTRAINT [DF_ErrorLog_ErrorTime] DEFAULT (getdate()) FOR [ErrorTime]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [dbo], N'TABLE', [ErrorLog], N'CONSTRAINT', [DF_ErrorLog_ErrorTime];

GO
