ALTER TABLE [Sales].[Currency] ADD CONSTRAINT [DF_Currency_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
