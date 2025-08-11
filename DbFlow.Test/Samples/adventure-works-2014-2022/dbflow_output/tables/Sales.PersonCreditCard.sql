CREATE TABLE [Sales].[PersonCreditCard] (
   [BusinessEntityID] [INT] NOT NULL,
   [CreditCardID] [INT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_PersonCreditCard_BusinessEntityID_CreditCardID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [CreditCardID])
)


GO
