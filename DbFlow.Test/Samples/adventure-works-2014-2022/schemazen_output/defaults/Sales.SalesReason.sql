ALTER TABLE [Sales].[SalesReason] ADD CONSTRAINT [DF_SalesReason_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
