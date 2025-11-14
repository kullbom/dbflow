ALTER TABLE [Sales].[SpecialOfferProduct] WITH CHECK ADD CONSTRAINT [FK_SpecialOfferProduct_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
ALTER TABLE [Sales].[SpecialOfferProduct] WITH CHECK ADD CONSTRAINT [FK_SpecialOfferProduct_SpecialOffer_SpecialOfferID]
   FOREIGN KEY([SpecialOfferID]) REFERENCES [Sales].[SpecialOffer] ([SpecialOfferID])

GO
