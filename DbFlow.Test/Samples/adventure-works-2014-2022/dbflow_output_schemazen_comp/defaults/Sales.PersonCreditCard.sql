ALTER TABLE [Sales].[PersonCreditCard] ADD CONSTRAINT [DF_PersonCreditCard_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
