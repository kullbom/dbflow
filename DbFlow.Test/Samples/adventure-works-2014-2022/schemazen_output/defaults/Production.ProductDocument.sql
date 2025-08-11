ALTER TABLE [Production].[ProductDocument] ADD CONSTRAINT [DF_ProductDocument_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
