ALTER TABLE [Sales].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [CK_SalesOrderDetail_UnitPriceDiscount] CHECK ([UnitPriceDiscount]>=(0.00))
GO
ALTER TABLE [Sales].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [CK_SalesOrderDetail_UnitPrice] CHECK ([UnitPrice]>=(0.00))
GO
ALTER TABLE [Sales].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [CK_SalesOrderDetail_OrderQty] CHECK ([OrderQty]>(0))
GO
