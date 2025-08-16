ALTER TABLE [Sales].[SalesOrderHeaderSalesReason] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeaderSalesReason_SalesOrderHeader_SalesOrderID]
   FOREIGN KEY([SalesOrderID]) REFERENCES [Sales].[SalesOrderHeader] ([SalesOrderID])

GO
ALTER TABLE [Sales].[SalesOrderHeaderSalesReason] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeaderSalesReason_SalesReason_SalesReasonID]
   FOREIGN KEY([SalesReasonID]) REFERENCES [Sales].[SalesReason] ([SalesReasonID])

GO
