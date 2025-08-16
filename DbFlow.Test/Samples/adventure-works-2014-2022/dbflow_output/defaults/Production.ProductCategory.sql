ALTER TABLE [Production].[ProductCategory] ADD CONSTRAINT [DF_ProductCategory_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Production].[ProductCategory] ADD CONSTRAINT [DF_ProductCategory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
