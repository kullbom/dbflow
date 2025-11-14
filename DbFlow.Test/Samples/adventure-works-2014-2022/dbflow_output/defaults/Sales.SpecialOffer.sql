ALTER TABLE [Sales].[SpecialOffer] ADD CONSTRAINT [DF_SpecialOffer_DiscountPct] DEFAULT ((0.00)) FOR [DiscountPct]
GO
ALTER TABLE [Sales].[SpecialOffer] ADD CONSTRAINT [DF_SpecialOffer_MinQty] DEFAULT ((0)) FOR [MinQty]
GO
ALTER TABLE [Sales].[SpecialOffer] ADD CONSTRAINT [DF_SpecialOffer_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[SpecialOffer] ADD CONSTRAINT [DF_SpecialOffer_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
