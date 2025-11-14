ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_ProductModel_ProductModelID]
   FOREIGN KEY([ProductModelID]) REFERENCES [Production].[ProductModel] ([ProductModelID])

GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_ProductSubcategory_ProductSubcategoryID]
   FOREIGN KEY([ProductSubcategoryID]) REFERENCES [Production].[ProductSubcategory] ([ProductSubcategoryID])

GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_UnitMeasure_SizeUnitMeasureCode]
   FOREIGN KEY([SizeUnitMeasureCode]) REFERENCES [Production].[UnitMeasure] ([UnitMeasureCode])

GO
ALTER TABLE [Production].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_UnitMeasure_WeightUnitMeasureCode]
   FOREIGN KEY([WeightUnitMeasureCode]) REFERENCES [Production].[UnitMeasure] ([UnitMeasureCode])

GO
