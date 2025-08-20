ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_SellEndDate] CHECK ([SellEndDate]>=[SellStartDate] OR [SellEndDate] IS NULL)
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_Style] CHECK (upper([Style])='U' OR upper([Style])='M' OR upper([Style])='W' OR [Style] IS NULL)
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_Class] CHECK (upper([Class])='H' OR upper([Class])='M' OR upper([Class])='L' OR [Class] IS NULL)
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_ProductLine] CHECK (upper([ProductLine])='R' OR upper([ProductLine])='M' OR upper([ProductLine])='T' OR upper([ProductLine])='S' OR [ProductLine] IS NULL)
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_DaysToManufacture] CHECK ([DaysToManufacture]>=(0))
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_Weight] CHECK ([Weight]>(0.00))
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_ListPrice] CHECK ([ListPrice]>=(0.00))
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_StandardCost] CHECK ([StandardCost]>=(0.00))
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_ReorderPoint] CHECK ([ReorderPoint]>(0))
GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_SafetyStockLevel] CHECK ([SafetyStockLevel]>(0))
GO
