ALTER TABLE [SalesLT].[Product] ADD CONSTRAINT [DF_Product_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [SalesLT].[Product] ADD CONSTRAINT [DF_Product_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
