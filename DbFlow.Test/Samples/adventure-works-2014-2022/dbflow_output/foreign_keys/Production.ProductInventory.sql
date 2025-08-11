ALTER TABLE [Production].[ProductInventory] WITH CHECK ADD CONSTRAINT [FK_ProductInventory_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
ALTER TABLE [Production].[ProductInventory] WITH CHECK ADD CONSTRAINT [FK_ProductInventory_Location_LocationID]
   FOREIGN KEY([LocationID]) REFERENCES [Production].[Location] ([LocationID])

GO
