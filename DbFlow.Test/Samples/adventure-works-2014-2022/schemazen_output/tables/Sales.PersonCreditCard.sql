CREATE TABLE [Sales].[PersonCreditCard] (
   [BusinessEntityID] [int] NOT NULL,
   [CreditCardID] [int] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_PersonCreditCard_BusinessEntityID_CreditCardID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [CreditCardID])
)


GO
