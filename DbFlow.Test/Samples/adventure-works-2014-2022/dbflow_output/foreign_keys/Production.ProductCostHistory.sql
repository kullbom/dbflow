ALTER TABLE [Production].[ProductCostHistory] WITH CHECK ADD CONSTRAINT [FK_ProductCostHistory_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
