ALTER TABLE [Production].[ScrapReason] ADD CONSTRAINT [DF_ScrapReason_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
