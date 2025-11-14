ALTER TABLE [Sales].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID]
   FOREIGN KEY([SalesOrderID]) REFERENCES [Sales].[SalesOrderHeader] ([SalesOrderID])
   ON DELETE CASCADE

GO
ALTER TABLE [Sales].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_SpecialOfferProduct_SpecialOfferIDProductID]
   FOREIGN KEY([SpecialOfferID], [ProductID]) REFERENCES [Sales].[SpecialOfferProduct] ([SpecialOfferID], [ProductID])

GO
