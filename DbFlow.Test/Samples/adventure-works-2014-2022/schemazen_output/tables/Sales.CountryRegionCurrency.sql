CREATE TABLE [Sales].[CountryRegionCurrency] (
   [CountryRegionCode] [nvarchar](3) NOT NULL,
   [CurrencyCode] [nchar](3) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_CountryRegionCurrency_CountryRegionCode_CurrencyCode] PRIMARY KEY CLUSTERED ([CountryRegionCode], [CurrencyCode])
)

CREATE NONCLUSTERED INDEX [IX_CountryRegionCurrency_CurrencyCode] ON [Sales].[CountryRegionCurrency] ([CurrencyCode])

GO
