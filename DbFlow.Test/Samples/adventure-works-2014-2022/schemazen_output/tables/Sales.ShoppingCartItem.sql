CREATE TABLE [Sales].[ShoppingCartItem] (
   [ShoppingCartItemID] [int] NOT NULL
      IDENTITY (1,1),
   [ShoppingCartID] [nvarchar](50) NOT NULL,
   [Quantity] [int] NOT NULL,
   [ProductID] [int] NOT NULL,
   [DateCreated] [datetime] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ShoppingCartItem_ShoppingCartItemID] PRIMARY KEY CLUSTERED ([ShoppingCartItemID])
)

CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartID_ProductID] ON [Sales].[ShoppingCartItem] ([ShoppingCartID], [ProductID])

GO
