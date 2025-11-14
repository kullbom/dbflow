ALTER TABLE [Sales].[SalesTerritoryHistory] ADD CONSTRAINT [DF_SalesTerritoryHistory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[SalesTerritoryHistory] ADD CONSTRAINT [DF_SalesTerritoryHistory_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
