ALTER TABLE [Person].[Password] WITH CHECK ADD CONSTRAINT [FK_Password_Person_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Person].[Person] ([BusinessEntityID])

GO
