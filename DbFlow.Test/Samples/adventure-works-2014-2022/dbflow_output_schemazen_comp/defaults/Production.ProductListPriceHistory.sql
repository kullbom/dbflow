ALTER TABLE [Production].[ProductListPriceHistory] ADD CONSTRAINT [DF_ProductListPriceHistory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
