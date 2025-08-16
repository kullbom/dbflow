ALTER TABLE [Production].[ProductInventory] ADD CONSTRAINT [DF_ProductInventory_Quantity] DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [Production].[ProductInventory] ADD CONSTRAINT [DF_ProductInventory_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Production].[ProductInventory] ADD CONSTRAINT [DF_ProductInventory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
