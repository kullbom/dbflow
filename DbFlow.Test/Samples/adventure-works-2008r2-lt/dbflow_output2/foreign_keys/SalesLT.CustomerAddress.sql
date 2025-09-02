ALTER TABLE [SalesLT].[CustomerAddress] WITH CHECK ADD CONSTRAINT [FK_CustomerAddress_Customer_CustomerID]
   FOREIGN KEY([CustomerID]) REFERENCES [SalesLT].[Customer] ([CustomerID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing Customer.CustomerID.', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'CONSTRAINT', [FK_CustomerAddress_Customer_CustomerID];
GO
ALTER TABLE [SalesLT].[CustomerAddress] WITH CHECK ADD CONSTRAINT [FK_CustomerAddress_Address_AddressID]
   FOREIGN KEY([AddressID]) REFERENCES [SalesLT].[Address] ([AddressID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing Address.AddressID.', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'CONSTRAINT', [FK_CustomerAddress_Address_AddressID];
GO
