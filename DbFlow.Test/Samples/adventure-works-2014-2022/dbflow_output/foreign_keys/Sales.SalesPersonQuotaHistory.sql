ALTER TABLE [Sales].[SalesPersonQuotaHistory] WITH CHECK ADD CONSTRAINT [FK_SalesPersonQuotaHistory_SalesPerson_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Sales].[SalesPerson] ([BusinessEntityID])

GO
