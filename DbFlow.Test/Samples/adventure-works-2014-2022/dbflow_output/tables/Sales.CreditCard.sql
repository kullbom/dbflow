CREATE TABLE [Sales].[CreditCard] (
   [CreditCardID] [INT] NOT NULL
      IDENTITY (1,1),
   [CardType] [NVARCHAR](50) NOT NULL,
   [CardNumber] [NVARCHAR](25) NOT NULL,
   [ExpMonth] [TINYINT] NOT NULL,
   [ExpYear] [SMALLINT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_CreditCard_CreditCardID] PRIMARY KEY CLUSTERED ([CreditCardID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_CreditCard_CardNumber] ON [Sales].[CreditCard] ([CardNumber])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Sales], N'TABLE', [CreditCard], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Credit card expiration year.', N'SCHEMA', [Sales], N'TABLE', [CreditCard], N'COLUMN', [ExpYear];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Credit card expiration month.', N'SCHEMA', [Sales], N'TABLE', [CreditCard], N'COLUMN', [ExpMonth];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Credit card number.', N'SCHEMA', [Sales], N'TABLE', [CreditCard], N'COLUMN', [CardNumber];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Credit card name.', N'SCHEMA', [Sales], N'TABLE', [CreditCard], N'COLUMN', [CardType];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for CreditCard records.', N'SCHEMA', [Sales], N'TABLE', [CreditCard], N'COLUMN', [CreditCardID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Customer credit card information.', N'SCHEMA', [Sales], N'TABLE', [CreditCard], NULL, NULL;
