CREATE TABLE [Sales].[SpecialOffer] (
   [SpecialOfferID] [int] NOT NULL
      IDENTITY (1,1),
   [Description] [nvarchar](255) NOT NULL,
   [DiscountPct] [smallmoney] NOT NULL,
   [Type] [nvarchar](50) NOT NULL,
   [Category] [nvarchar](50) NOT NULL,
   [StartDate] [datetime] NOT NULL,
   [EndDate] [datetime] NOT NULL,
   [MinQty] [int] NOT NULL,
   [MaxQty] [int] NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SpecialOffer_SpecialOfferID] PRIMARY KEY CLUSTERED ([SpecialOfferID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SpecialOffer_rowguid] ON [Sales].[SpecialOffer] ([rowguid])

GO
