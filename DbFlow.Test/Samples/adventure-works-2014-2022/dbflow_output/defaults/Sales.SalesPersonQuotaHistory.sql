ALTER TABLE [Sales].[SalesPersonQuotaHistory] ADD CONSTRAINT [DF_SalesPersonQuotaHistory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[SalesPersonQuotaHistory] ADD CONSTRAINT [DF_SalesPersonQuotaHistory_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
