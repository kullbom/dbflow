ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_SalesTerritory_TerritoryID]
   FOREIGN KEY([TerritoryID]) REFERENCES [Sales].[SalesTerritory] ([TerritoryID])

GO
ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_ShipMethod_ShipMethodID]
   FOREIGN KEY([ShipMethodID]) REFERENCES [Purchasing].[ShipMethod] ([ShipMethodID])

GO
ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_SalesPerson_SalesPersonID]
   FOREIGN KEY([SalesPersonID]) REFERENCES [Sales].[SalesPerson] ([BusinessEntityID])

GO
ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Customer_CustomerID]
   FOREIGN KEY([CustomerID]) REFERENCES [Sales].[Customer] ([CustomerID])

GO
ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_CurrencyRate_CurrencyRateID]
   FOREIGN KEY([CurrencyRateID]) REFERENCES [Sales].[CurrencyRate] ([CurrencyRateID])

GO
ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_CreditCard_CreditCardID]
   FOREIGN KEY([CreditCardID]) REFERENCES [Sales].[CreditCard] ([CreditCardID])

GO
ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Address_ShipToAddressID]
   FOREIGN KEY([ShipToAddressID]) REFERENCES [Person].[Address] ([AddressID])

GO
ALTER TABLE [Sales].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Address_BillToAddressID]
   FOREIGN KEY([BillToAddressID]) REFERENCES [Person].[Address] ([AddressID])

GO
