ALTER TABLE [Sales].[ShoppingCartItem] WITH CHECK ADD CONSTRAINT [CK_ShoppingCartItem_Quantity] CHECK  ([Quantity]>=(1))
GO
