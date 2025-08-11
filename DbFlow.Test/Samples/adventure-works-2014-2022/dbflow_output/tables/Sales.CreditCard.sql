CREATE TABLE [Sales].[CreditCard] (
   [CreditCardID] [INT] NOT NULL
      IDENTITY (1,1),
   [CardType] [NVARCHAR](50) NOT NULL,
   [CardNumber] [NVARCHAR](25) NOT NULL,
   [ExpMonth] [TINYINT] NOT NULL,
   [ExpYear] [SMALLINT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_CreditCard_CreditCardID] PRIMARY KEY CLUSTERED ([CreditCardID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_CreditCard_CardNumber] ON [Sales].[CreditCard] ([CardNumber])

GO
