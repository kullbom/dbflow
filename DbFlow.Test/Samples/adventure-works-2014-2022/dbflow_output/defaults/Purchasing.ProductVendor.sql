ALTER TABLE [Purchasing].[ProductVendor] ADD CONSTRAINT [DF_ProductVendor_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
