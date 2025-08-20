ALTER TABLE [dbo].[ErrorLog] ADD CONSTRAINT [DF_ErrorLog_ErrorTime] DEFAULT (getdate()) FOR [ErrorTime]
GO
