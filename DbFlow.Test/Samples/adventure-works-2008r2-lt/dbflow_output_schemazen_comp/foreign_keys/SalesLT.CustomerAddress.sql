ALTER TABLE [SalesLT].[CustomerAddress] WITH CHECK ADD CONSTRAINT [FK_CustomerAddress_Address_AddressID]
   FOREIGN KEY([AddressID]) REFERENCES [SalesLT].[Address] ([AddressID])

GO
ALTER TABLE [SalesLT].[CustomerAddress] WITH CHECK ADD CONSTRAINT [FK_CustomerAddress_Customer_CustomerID]
   FOREIGN KEY([CustomerID]) REFERENCES [SalesLT].[Customer] ([CustomerID])

GO
