ALTER TABLE [SalesLT].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID]
   FOREIGN KEY([SalesOrderID]) REFERENCES [SalesLT].[SalesOrderHeader] ([SalesOrderID])
   ON DELETE CASCADE

GO
ALTER TABLE [SalesLT].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [SalesLT].[Product] ([ProductID])

GO
