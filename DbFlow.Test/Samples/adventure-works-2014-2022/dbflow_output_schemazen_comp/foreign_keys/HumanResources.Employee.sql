ALTER TABLE [HumanResources].[Employee] WITH CHECK ADD CONSTRAINT [FK_Employee_Person_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Person].[Person] ([BusinessEntityID])

GO
