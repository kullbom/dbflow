CREATE TABLE [Sales].[CurrencyRate] (
   [CurrencyRateID] [INT] NOT NULL
      IDENTITY (1,1),
   [CurrencyRateDate] [DATETIME] NOT NULL,
   [FromCurrencyCode] [NCHAR](3) NOT NULL,
   [ToCurrencyCode] [NCHAR](3) NOT NULL,
   [AverageRate] [MONEY] NOT NULL,
   [EndOfDayRate] [MONEY] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_CurrencyRate_CurrencyRateID] PRIMARY KEY CLUSTERED ([CurrencyRateID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_CurrencyRate_CurrencyRateDate_FromCurrencyCode_ToCurrencyCode] ON [Sales].[CurrencyRate] ([CurrencyRateDate], [FromCurrencyCode], [ToCurrencyCode])

GO
