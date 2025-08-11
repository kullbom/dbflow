CREATE TABLE [Sales].[CurrencyRate] (
   [CurrencyRateID] [int] NOT NULL
      IDENTITY (1,1),
   [CurrencyRateDate] [datetime] NOT NULL,
   [FromCurrencyCode] [nchar](3) NOT NULL,
   [ToCurrencyCode] [nchar](3) NOT NULL,
   [AverageRate] [money] NOT NULL,
   [EndOfDayRate] [money] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_CurrencyRate_CurrencyRateID] PRIMARY KEY CLUSTERED ([CurrencyRateID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_CurrencyRate_CurrencyRateDate_FromCurrencyCode_ToCurrencyCode] ON [Sales].[CurrencyRate] ([CurrencyRateDate], [FromCurrencyCode], [ToCurrencyCode])

GO
