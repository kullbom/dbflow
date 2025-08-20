ALTER TABLE [dbo].[BuildVersion] ADD CONSTRAINT [DF_BuildVersion_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
