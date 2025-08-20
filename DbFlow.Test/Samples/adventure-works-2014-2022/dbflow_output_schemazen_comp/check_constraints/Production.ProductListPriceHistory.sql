ALTER TABLE [Production].[ProductListPriceHistory] WITH CHECK ADD CONSTRAINT [CK_ProductListPriceHistory_ListPrice] CHECK  ([ListPrice]>(0.00))
GO
ALTER TABLE [Production].[ProductListPriceHistory] WITH CHECK ADD CONSTRAINT [CK_ProductListPriceHistory_EndDate] CHECK  ([EndDate]>=[StartDate] OR [EndDate] IS NULL)
GO
