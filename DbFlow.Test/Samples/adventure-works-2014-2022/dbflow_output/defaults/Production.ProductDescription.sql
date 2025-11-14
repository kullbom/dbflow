ALTER TABLE [Production].[ProductDescription] ADD CONSTRAINT [DF_ProductDescription_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Production].[ProductDescription] ADD CONSTRAINT [DF_ProductDescription_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
