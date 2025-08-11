ALTER TABLE [Sales].[SalesOrderHeaderSalesReason] ADD CONSTRAINT [DF_SalesOrderHeaderSalesReason_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
