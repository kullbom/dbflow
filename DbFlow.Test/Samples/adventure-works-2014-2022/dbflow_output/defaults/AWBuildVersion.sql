ALTER TABLE [dbo].[AWBuildVersion] ADD CONSTRAINT [DF_AWBuildVersion_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
