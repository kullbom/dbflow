ALTER TABLE [Production].[Product] ADD CONSTRAINT [DF_Product_FinishedGoodsFlag] DEFAULT ((1)) FOR [FinishedGoodsFlag]
GO
ALTER TABLE [Production].[Product] ADD CONSTRAINT [DF_Product_MakeFlag] DEFAULT ((1)) FOR [MakeFlag]
GO
ALTER TABLE [Production].[Product] ADD CONSTRAINT [DF_Product_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Production].[Product] ADD CONSTRAINT [DF_Product_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
