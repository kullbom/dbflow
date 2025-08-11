ALTER TABLE [Sales].[SalesTerritory] WITH CHECK ADD CONSTRAINT [FK_SalesTerritory_CountryRegion_CountryRegionCode]
   FOREIGN KEY([CountryRegionCode]) REFERENCES [Person].[CountryRegion] ([CountryRegionCode])

GO
