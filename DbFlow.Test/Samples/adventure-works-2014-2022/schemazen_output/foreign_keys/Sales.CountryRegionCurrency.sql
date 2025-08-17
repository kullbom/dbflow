ALTER TABLE [Sales].[CountryRegionCurrency] WITH CHECK ADD CONSTRAINT [FK_CountryRegionCurrency_CountryRegion_CountryRegionCode]
   FOREIGN KEY([CountryRegionCode]) REFERENCES [Person].[CountryRegion] ([CountryRegionCode])

GO
ALTER TABLE [Sales].[CountryRegionCurrency] WITH CHECK ADD CONSTRAINT [FK_CountryRegionCurrency_Currency_CurrencyCode]
   FOREIGN KEY([CurrencyCode]) REFERENCES [Sales].[Currency] ([CurrencyCode])

GO
