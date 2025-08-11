ALTER TABLE [Person].[EmailAddress] WITH CHECK ADD CONSTRAINT [FK_EmailAddress_Person_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Person].[Person] ([BusinessEntityID])

GO
