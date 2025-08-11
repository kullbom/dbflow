ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Address_BillTo_AddressID]
   FOREIGN KEY([BillToAddressID]) REFERENCES [SalesLT].[Address] ([AddressID])

GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Address_ShipTo_AddressID]
   FOREIGN KEY([ShipToAddressID]) REFERENCES [SalesLT].[Address] ([AddressID])

GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Customer_CustomerID]
   FOREIGN KEY([CustomerID]) REFERENCES [SalesLT].[Customer] ([CustomerID])

GO
