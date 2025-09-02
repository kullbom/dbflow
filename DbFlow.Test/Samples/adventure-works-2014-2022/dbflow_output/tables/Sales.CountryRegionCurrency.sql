CREATE TABLE [Sales].[CountryRegionCurrency] (
   [CountryRegionCode] [NVARCHAR](3) NOT NULL,
   [CurrencyCode] [NCHAR](3) NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_CountryRegionCurrency_CountryRegionCode_CurrencyCode] PRIMARY KEY CLUSTERED ([CountryRegionCode], [CurrencyCode])
)

CREATE NONCLUSTERED INDEX [IX_CountryRegionCurrency_CurrencyCode] ON [Sales].[CountryRegionCurrency] ([CurrencyCode])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Cross-reference table mapping ISO currency codes to a country or region.', N'SCHEMA', [Sales], N'TABLE', [CountryRegionCurrency];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ISO code for countries and regions. Foreign key to CountryRegion.CountryRegionCode.', N'SCHEMA', [Sales], N'TABLE', [CountryRegionCurrency], N'COLUMN', [CountryRegionCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ISO standard currency code. Foreign key to Currency.CurrencyCode.', N'SCHEMA', [Sales], N'TABLE', [CountryRegionCurrency], N'COLUMN', [CurrencyCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Sales], N'TABLE', [CountryRegionCurrency], N'COLUMN', [ModifiedDate];
