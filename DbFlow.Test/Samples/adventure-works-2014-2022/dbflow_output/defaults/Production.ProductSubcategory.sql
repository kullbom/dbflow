ALTER TABLE [Production].[ProductSubcategory] ADD CONSTRAINT [DF_ProductSubcategory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Production].[ProductSubcategory] ADD CONSTRAINT [DF_ProductSubcategory_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
