ALTER TABLE [Production].[BillOfMaterials] WITH CHECK ADD CONSTRAINT [CK_BillOfMaterials_BOMLevel] CHECK  ([ProductAssemblyID] IS NULL AND [BOMLevel]=(0) AND [PerAssemblyQty]=(1.00) OR [ProductAssemblyID] IS NOT NULL AND [BOMLevel]>=(1))
GO
ALTER TABLE [Production].[BillOfMaterials] WITH CHECK ADD CONSTRAINT [CK_BillOfMaterials_EndDate] CHECK  ([EndDate]>[StartDate] OR [EndDate] IS NULL)
GO
ALTER TABLE [Production].[BillOfMaterials] WITH CHECK ADD CONSTRAINT [CK_BillOfMaterials_PerAssemblyQty] CHECK  ([PerAssemblyQty]>=(1.00))
GO
ALTER TABLE [Production].[BillOfMaterials] WITH CHECK ADD CONSTRAINT [CK_BillOfMaterials_ProductAssemblyID] CHECK  ([ProductAssemblyID]<>[ComponentID])
GO
