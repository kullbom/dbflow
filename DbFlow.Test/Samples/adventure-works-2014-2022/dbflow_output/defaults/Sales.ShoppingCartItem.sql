ALTER TABLE [Sales].[ShoppingCartItem] ADD CONSTRAINT [DF_ShoppingCartItem_Quantity] DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [Sales].[ShoppingCartItem] ADD CONSTRAINT [DF_ShoppingCartItem_DateCreated] DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [Sales].[ShoppingCartItem] ADD CONSTRAINT [DF_ShoppingCartItem_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
