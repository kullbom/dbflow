CREATE TABLE [Sales].[SpecialOffer] (
   [SpecialOfferID] [INT] NOT NULL
      IDENTITY (1,1),
   [Description] [NVARCHAR](255) NOT NULL,
   [DiscountPct] [SMALLMONEY] NOT NULL
       DEFAULT ((0.00)),
   [Type] [NVARCHAR](50) NOT NULL,
   [Category] [NVARCHAR](50) NOT NULL,
   [StartDate] [DATETIME] NOT NULL,
   [EndDate] [DATETIME] NOT NULL,
   [MinQty] [INT] NOT NULL
       DEFAULT ((0)),
   [MaxQty] [INT] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_SpecialOffer_SpecialOfferID] PRIMARY KEY CLUSTERED ([SpecialOfferID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SpecialOffer_rowguid] ON [Sales].[SpecialOffer] ([rowguid])

GO
