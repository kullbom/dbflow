ALTER TABLE [Production].[ProductCostHistory] ADD CONSTRAINT [DF_ProductCostHistory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
