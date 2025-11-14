ALTER TABLE [Sales].[SalesTaxRate] ADD CONSTRAINT [DF_SalesTaxRate_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[SalesTaxRate] ADD CONSTRAINT [DF_SalesTaxRate_TaxRate] DEFAULT ((0.00)) FOR [TaxRate]
GO
ALTER TABLE [Sales].[SalesTaxRate] ADD CONSTRAINT [DF_SalesTaxRate_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
