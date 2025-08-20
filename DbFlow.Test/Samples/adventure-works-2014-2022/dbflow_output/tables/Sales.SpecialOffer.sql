CREATE TABLE [Sales].[SpecialOffer] (
   [SpecialOfferID] [INT] NOT NULL
      IDENTITY (1,1),
   [Description] [NVARCHAR](255) NOT NULL,
   [DiscountPct] [SMALLMONEY] NOT NULL,
   [Type] [NVARCHAR](50) NOT NULL,
   [Category] [NVARCHAR](50) NOT NULL,
   [StartDate] [DATETIME] NOT NULL,
   [EndDate] [DATETIME] NOT NULL,
   [MinQty] [INT] NOT NULL,
   [MaxQty] [INT] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SpecialOffer_SpecialOfferID] PRIMARY KEY CLUSTERED ([SpecialOfferID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SpecialOffer_rowguid] ON [Sales].[SpecialOffer] ([rowguid])

GO
