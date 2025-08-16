ALTER TABLE [Production].[Document] WITH CHECK ADD CONSTRAINT [FK_Document_Employee_Owner]
   FOREIGN KEY([Owner]) REFERENCES [HumanResources].[Employee] ([BusinessEntityID])

GO
