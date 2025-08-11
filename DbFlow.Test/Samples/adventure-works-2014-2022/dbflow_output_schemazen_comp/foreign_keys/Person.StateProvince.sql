ALTER TABLE [Person].[StateProvince] WITH CHECK ADD CONSTRAINT [FK_StateProvince_SalesTerritory_TerritoryID]
   FOREIGN KEY([TerritoryID]) REFERENCES [Sales].[SalesTerritory] ([TerritoryID])

GO
ALTER TABLE [Person].[StateProvince] WITH CHECK ADD CONSTRAINT [FK_StateProvince_CountryRegion_CountryRegionCode]
   FOREIGN KEY([CountryRegionCode]) REFERENCES [Person].[CountryRegion] ([CountryRegionCode])

GO
