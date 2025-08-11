ALTER TABLE [SalesLT].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID]
   FOREIGN KEY([SalesOrderID]) REFERENCES [SalesLT].[SalesOrderHeader] ([SalesOrderID])

GO
ALTER TABLE [SalesLT].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [SalesLT].[Product] ([ProductID])

GO
