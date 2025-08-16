ALTER TABLE [Production].[ProductListPriceHistory] WITH CHECK ADD CONSTRAINT [FK_ProductListPriceHistory_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
