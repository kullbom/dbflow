CREATE TABLE [Sales].[ShoppingCartItem] (
   [ShoppingCartItemID] [INT] NOT NULL
      IDENTITY (1,1),
   [ShoppingCartID] [NVARCHAR](50) NOT NULL,
   [Quantity] [INT] NOT NULL
       DEFAULT ((1)),
   [ProductID] [INT] NOT NULL,
   [DateCreated] [DATETIME] NOT NULL
       DEFAULT (getdate()),
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_ShoppingCartItem_ShoppingCartItemID] PRIMARY KEY CLUSTERED ([ShoppingCartItemID])
)

CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartID_ProductID] ON [Sales].[ShoppingCartItem] ([ShoppingCartID], [ProductID])

GO
