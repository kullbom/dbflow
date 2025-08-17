CREATE TABLE [Sales].[SpecialOfferProduct] (
   [SpecialOfferID] [int] NOT NULL,
   [ProductID] [int] NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SpecialOfferProduct_SpecialOfferID_ProductID] PRIMARY KEY CLUSTERED ([SpecialOfferID], [ProductID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SpecialOfferProduct_rowguid] ON [Sales].[SpecialOfferProduct] ([rowguid])
CREATE NONCLUSTERED INDEX [IX_SpecialOfferProduct_ProductID] ON [Sales].[SpecialOfferProduct] ([ProductID])

GO
