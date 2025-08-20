ALTER TABLE [Production].[ProductModel] ADD CONSTRAINT [DF_ProductModel_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Production].[ProductModel] ADD CONSTRAINT [DF_ProductModel_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
