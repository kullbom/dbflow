ALTER TABLE [Production].[ProductPhoto] ADD CONSTRAINT [DF_ProductPhoto_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
