ALTER TABLE [Production].[TransactionHistory] ADD CONSTRAINT [DF_TransactionHistory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Production].[TransactionHistory] ADD CONSTRAINT [DF_TransactionHistory_ReferenceOrderLineID] DEFAULT ((0)) FOR [ReferenceOrderLineID]
GO
ALTER TABLE [Production].[TransactionHistory] ADD CONSTRAINT [DF_TransactionHistory_TransactionDate] DEFAULT (getdate()) FOR [TransactionDate]
GO
