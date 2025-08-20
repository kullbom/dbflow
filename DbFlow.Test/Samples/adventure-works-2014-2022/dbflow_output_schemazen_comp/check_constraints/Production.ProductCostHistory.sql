ALTER TABLE [Production].[ProductCostHistory] WITH CHECK ADD CONSTRAINT [CK_ProductCostHistory_StandardCost] CHECK  ([StandardCost]>=(0.00))
GO
ALTER TABLE [Production].[ProductCostHistory] WITH CHECK ADD CONSTRAINT [CK_ProductCostHistory_EndDate] CHECK  ([EndDate]>=[StartDate] OR [EndDate] IS NULL)
GO
