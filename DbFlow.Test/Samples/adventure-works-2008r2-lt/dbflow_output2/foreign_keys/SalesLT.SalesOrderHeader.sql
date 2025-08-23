ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Customer_CustomerID]
   FOREIGN KEY([CustomerID]) REFERENCES [SalesLT].[Customer] ([CustomerID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing Customer.CustomerID.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [FK_SalesOrderHeader_Customer_CustomerID];

GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Address_ShipTo_AddressID]
   FOREIGN KEY([ShipToAddressID]) REFERENCES [SalesLT].[Address] ([AddressID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing Address.AddressID for ShipTo.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [FK_SalesOrderHeader_Address_ShipTo_AddressID];

GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [FK_SalesOrderHeader_Address_BillTo_AddressID]
   FOREIGN KEY([BillToAddressID]) REFERENCES [SalesLT].[Address] ([AddressID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing Address.AddressID for BillTo.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [FK_SalesOrderHeader_Address_BillTo_AddressID];

GO
