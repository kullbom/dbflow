ALTER TABLE [Sales].[SalesTerritory] WITH CHECK ADD CONSTRAINT [CK_SalesTerritory_CostLastYear] CHECK  ([CostLastYear]>=(0.00))
GO
ALTER TABLE [Sales].[SalesTerritory] WITH CHECK ADD CONSTRAINT [CK_SalesTerritory_CostYTD] CHECK  ([CostYTD]>=(0.00))
GO
ALTER TABLE [Sales].[SalesTerritory] WITH CHECK ADD CONSTRAINT [CK_SalesTerritory_SalesLastYear] CHECK  ([SalesLastYear]>=(0.00))
GO
ALTER TABLE [Sales].[SalesTerritory] WITH CHECK ADD CONSTRAINT [CK_SalesTerritory_SalesYTD] CHECK  ([SalesYTD]>=(0.00))
GO
