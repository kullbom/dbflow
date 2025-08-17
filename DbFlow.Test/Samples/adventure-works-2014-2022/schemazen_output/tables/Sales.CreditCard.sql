CREATE TABLE [Sales].[CreditCard] (
   [CreditCardID] [int] NOT NULL
      IDENTITY (1,1),
   [CardType] [nvarchar](50) NOT NULL,
   [CardNumber] [nvarchar](25) NOT NULL,
   [ExpMonth] [tinyint] NOT NULL,
   [ExpYear] [smallint] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_CreditCard_CreditCardID] PRIMARY KEY CLUSTERED ([CreditCardID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_CreditCard_CardNumber] ON [Sales].[CreditCard] ([CardNumber])

GO
