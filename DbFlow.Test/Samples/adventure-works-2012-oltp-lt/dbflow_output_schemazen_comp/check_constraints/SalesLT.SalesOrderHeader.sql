ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [CK_SalesOrderHeader_Freight] CHECK  ([Freight]>=(0.00))
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [CK_SalesOrderHeader_TaxAmt] CHECK  ([TaxAmt]>=(0.00))
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [CK_SalesOrderHeader_SubTotal] CHECK  ([SubTotal]>=(0.00))
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [CK_SalesOrderHeader_ShipDate] CHECK  ([ShipDate]>=[OrderDate] OR [ShipDate] IS NULL)
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [CK_SalesOrderHeader_DueDate] CHECK  ([DueDate]>=[OrderDate])
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] WITH CHECK ADD CONSTRAINT [CK_SalesOrderHeader_Status] CHECK  ([Status]>=(0) AND [Status]<=(8))
GO
