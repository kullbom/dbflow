ALTER TABLE [Sales].[SalesPerson] WITH CHECK ADD CONSTRAINT [CK_SalesPerson_Bonus] CHECK  ([Bonus]>=(0.00))
GO
ALTER TABLE [Sales].[SalesPerson] WITH CHECK ADD CONSTRAINT [CK_SalesPerson_CommissionPct] CHECK  ([CommissionPct]>=(0.00))
GO
ALTER TABLE [Sales].[SalesPerson] WITH CHECK ADD CONSTRAINT [CK_SalesPerson_SalesLastYear] CHECK  ([SalesLastYear]>=(0.00))
GO
ALTER TABLE [Sales].[SalesPerson] WITH CHECK ADD CONSTRAINT [CK_SalesPerson_SalesQuota] CHECK  ([SalesQuota]>(0.00))
GO
ALTER TABLE [Sales].[SalesPerson] WITH CHECK ADD CONSTRAINT [CK_SalesPerson_SalesYTD] CHECK  ([SalesYTD]>=(0.00))
GO
