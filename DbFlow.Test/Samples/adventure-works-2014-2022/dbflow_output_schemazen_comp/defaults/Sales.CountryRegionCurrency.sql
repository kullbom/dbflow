ALTER TABLE [Sales].[CountryRegionCurrency] ADD CONSTRAINT [DF_CountryRegionCurrency_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
