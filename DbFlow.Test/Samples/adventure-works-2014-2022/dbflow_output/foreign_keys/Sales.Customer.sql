ALTER TABLE [Sales].[Customer] WITH CHECK ADD CONSTRAINT [FK_Customer_Person_PersonID]
   FOREIGN KEY([PersonID]) REFERENCES [Person].[Person] ([BusinessEntityID])

GO
ALTER TABLE [Sales].[Customer] WITH CHECK ADD CONSTRAINT [FK_Customer_SalesTerritory_TerritoryID]
   FOREIGN KEY([TerritoryID]) REFERENCES [Sales].[SalesTerritory] ([TerritoryID])

GO
ALTER TABLE [Sales].[Customer] WITH CHECK ADD CONSTRAINT [FK_Customer_Store_StoreID]
   FOREIGN KEY([StoreID]) REFERENCES [Sales].[Store] ([BusinessEntityID])

GO
