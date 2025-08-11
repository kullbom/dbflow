ALTER TABLE [Production].[ProductModelProductDescriptionCulture] ADD CONSTRAINT [DF_ProductModelProductDescriptionCulture_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
