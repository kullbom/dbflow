ALTER TABLE [Sales].[CurrencyRate] WITH CHECK ADD CONSTRAINT [FK_CurrencyRate_Currency_ToCurrencyCode]
   FOREIGN KEY([ToCurrencyCode]) REFERENCES [Sales].[Currency] ([CurrencyCode])

GO
ALTER TABLE [Sales].[CurrencyRate] WITH CHECK ADD CONSTRAINT [FK_CurrencyRate_Currency_FromCurrencyCode]
   FOREIGN KEY([FromCurrencyCode]) REFERENCES [Sales].[Currency] ([CurrencyCode])

GO
