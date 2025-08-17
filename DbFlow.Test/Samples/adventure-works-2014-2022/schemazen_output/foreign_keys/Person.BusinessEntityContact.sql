ALTER TABLE [Person].[BusinessEntityContact] WITH CHECK ADD CONSTRAINT [FK_BusinessEntityContact_BusinessEntity_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Person].[BusinessEntity] ([BusinessEntityID])

GO
ALTER TABLE [Person].[BusinessEntityContact] WITH CHECK ADD CONSTRAINT [FK_BusinessEntityContact_ContactType_ContactTypeID]
   FOREIGN KEY([ContactTypeID]) REFERENCES [Person].[ContactType] ([ContactTypeID])

GO
ALTER TABLE [Person].[BusinessEntityContact] WITH CHECK ADD CONSTRAINT [FK_BusinessEntityContact_Person_PersonID]
   FOREIGN KEY([PersonID]) REFERENCES [Person].[Person] ([BusinessEntityID])

GO
