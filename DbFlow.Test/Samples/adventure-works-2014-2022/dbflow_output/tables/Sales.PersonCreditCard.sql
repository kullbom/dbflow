CREATE TABLE [Sales].[PersonCreditCard] (
   [BusinessEntityID] [INT] NOT NULL,
   [CreditCardID] [INT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_PersonCreditCard_BusinessEntityID_CreditCardID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [CreditCardID])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Cross-reference table mapping people to their credit card information in the CreditCard table. ', N'SCHEMA', [Sales], N'TABLE', [PersonCreditCard];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Business entity identification number. Foreign key to Person.BusinessEntityID.', N'SCHEMA', [Sales], N'TABLE', [PersonCreditCard], N'COLUMN', [BusinessEntityID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Credit card identification number. Foreign key to CreditCard.CreditCardID.', N'SCHEMA', [Sales], N'TABLE', [PersonCreditCard], N'COLUMN', [CreditCardID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Sales], N'TABLE', [PersonCreditCard], N'COLUMN', [ModifiedDate];
