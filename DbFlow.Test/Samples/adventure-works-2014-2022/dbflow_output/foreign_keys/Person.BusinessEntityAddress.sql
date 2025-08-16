ALTER TABLE [Person].[BusinessEntityAddress] WITH CHECK ADD CONSTRAINT [FK_BusinessEntityAddress_BusinessEntity_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Person].[BusinessEntity] ([BusinessEntityID])

GO
ALTER TABLE [Person].[BusinessEntityAddress] WITH CHECK ADD CONSTRAINT [FK_BusinessEntityAddress_AddressType_AddressTypeID]
   FOREIGN KEY([AddressTypeID]) REFERENCES [Person].[AddressType] ([AddressTypeID])

GO
ALTER TABLE [Person].[BusinessEntityAddress] WITH CHECK ADD CONSTRAINT [FK_BusinessEntityAddress_Address_AddressID]
   FOREIGN KEY([AddressID]) REFERENCES [Person].[Address] ([AddressID])

GO
