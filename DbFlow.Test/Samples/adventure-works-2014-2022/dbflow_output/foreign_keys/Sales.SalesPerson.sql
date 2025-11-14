ALTER TABLE [Sales].[SalesPerson] WITH CHECK ADD CONSTRAINT [FK_SalesPerson_Employee_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [HumanResources].[Employee] ([BusinessEntityID])

GO
ALTER TABLE [Sales].[SalesPerson] WITH CHECK ADD CONSTRAINT [FK_SalesPerson_SalesTerritory_TerritoryID]
   FOREIGN KEY([TerritoryID]) REFERENCES [Sales].[SalesTerritory] ([TerritoryID])

GO
