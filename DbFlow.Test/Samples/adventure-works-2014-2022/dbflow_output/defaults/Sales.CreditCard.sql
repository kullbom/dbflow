ALTER TABLE [Sales].[CreditCard] ADD CONSTRAINT [DF_CreditCard_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
