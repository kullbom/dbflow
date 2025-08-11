ALTER TABLE [Production].[BillOfMaterials] WITH CHECK ADD CONSTRAINT [FK_BillOfMaterials_UnitMeasure_UnitMeasureCode]
   FOREIGN KEY([UnitMeasureCode]) REFERENCES [Production].[UnitMeasure] ([UnitMeasureCode])

GO
ALTER TABLE [Production].[BillOfMaterials] WITH CHECK ADD CONSTRAINT [FK_BillOfMaterials_Product_ComponentID]
   FOREIGN KEY([ComponentID]) REFERENCES [Production].[Product] ([ProductID])

GO
ALTER TABLE [Production].[BillOfMaterials] WITH CHECK ADD CONSTRAINT [FK_BillOfMaterials_Product_ProductAssemblyID]
   FOREIGN KEY([ProductAssemblyID]) REFERENCES [Production].[Product] ([ProductID])

GO
