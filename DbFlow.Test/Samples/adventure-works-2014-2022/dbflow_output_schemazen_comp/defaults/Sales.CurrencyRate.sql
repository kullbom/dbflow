ALTER TABLE [Sales].[CurrencyRate] ADD CONSTRAINT [DF_CurrencyRate_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
