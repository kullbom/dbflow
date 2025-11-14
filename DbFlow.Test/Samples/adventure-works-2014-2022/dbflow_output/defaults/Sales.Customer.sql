ALTER TABLE [Sales].[Customer] ADD CONSTRAINT [DF_Customer_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[Customer] ADD CONSTRAINT [DF_Customer_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
