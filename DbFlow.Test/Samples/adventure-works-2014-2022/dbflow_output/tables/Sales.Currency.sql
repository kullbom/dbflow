CREATE TABLE [Sales].[Currency] (
   [CurrencyCode] [NCHAR](3) NOT NULL,
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Currency_CurrencyCode] PRIMARY KEY CLUSTERED ([CurrencyCode])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Currency_Name] ON [Sales].[Currency] ([Name])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Lookup table containing standard ISO currencies.', N'SCHEMA', [Sales], N'TABLE', [Currency];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The ISO code for the Currency.', N'SCHEMA', [Sales], N'TABLE', [Currency], N'COLUMN', [CurrencyCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Currency name.', N'SCHEMA', [Sales], N'TABLE', [Currency], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Sales], N'TABLE', [Currency], N'COLUMN', [ModifiedDate];
