ALTER TABLE [Sales].[SalesTaxRate] WITH CHECK ADD CONSTRAINT [CK_SalesTaxRate_TaxType] CHECK ([TaxType]>=(1) AND [TaxType]<=(3))
GO
