ALTER TABLE [Sales].[SalesTerritory] ADD CONSTRAINT [DF_SalesTerritory_CostLastYear] DEFAULT ((0.00)) FOR [CostLastYear]
GO
ALTER TABLE [Sales].[SalesTerritory] ADD CONSTRAINT [DF_SalesTerritory_CostYTD] DEFAULT ((0.00)) FOR [CostYTD]
GO
ALTER TABLE [Sales].[SalesTerritory] ADD CONSTRAINT [DF_SalesTerritory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[SalesTerritory] ADD CONSTRAINT [DF_SalesTerritory_SalesLastYear] DEFAULT ((0.00)) FOR [SalesLastYear]
GO
ALTER TABLE [Sales].[SalesTerritory] ADD CONSTRAINT [DF_SalesTerritory_SalesYTD] DEFAULT ((0.00)) FOR [SalesYTD]
GO
ALTER TABLE [Sales].[SalesTerritory] ADD CONSTRAINT [DF_SalesTerritory_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
