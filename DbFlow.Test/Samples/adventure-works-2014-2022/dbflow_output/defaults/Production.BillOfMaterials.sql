ALTER TABLE [Production].[BillOfMaterials] ADD CONSTRAINT [DF_BillOfMaterials_StartDate] DEFAULT (getdate()) FOR [StartDate]
GO
ALTER TABLE [Production].[BillOfMaterials] ADD CONSTRAINT [DF_BillOfMaterials_PerAssemblyQty] DEFAULT ((1.00)) FOR [PerAssemblyQty]
GO
ALTER TABLE [Production].[BillOfMaterials] ADD CONSTRAINT [DF_BillOfMaterials_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
