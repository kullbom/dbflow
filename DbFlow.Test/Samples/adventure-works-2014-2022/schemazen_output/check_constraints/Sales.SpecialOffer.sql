ALTER TABLE [Sales].[SpecialOffer] WITH CHECK ADD CONSTRAINT [CK_SpecialOffer_DiscountPct] CHECK  ([DiscountPct]>=(0.00))
GO
ALTER TABLE [Sales].[SpecialOffer] WITH CHECK ADD CONSTRAINT [CK_SpecialOffer_EndDate] CHECK  ([EndDate]>=[StartDate])
GO
ALTER TABLE [Sales].[SpecialOffer] WITH CHECK ADD CONSTRAINT [CK_SpecialOffer_MaxQty] CHECK  ([MaxQty]>=(0))
GO
ALTER TABLE [Sales].[SpecialOffer] WITH CHECK ADD CONSTRAINT [CK_SpecialOffer_MinQty] CHECK  ([MinQty]>=(0))
GO
