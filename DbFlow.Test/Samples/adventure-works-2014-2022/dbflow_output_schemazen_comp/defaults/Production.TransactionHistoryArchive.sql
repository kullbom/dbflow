ALTER TABLE [Production].[TransactionHistoryArchive] ADD CONSTRAINT [DF_TransactionHistoryArchive_ReferenceOrderLineID] DEFAULT ((0)) FOR [ReferenceOrderLineID]
GO
ALTER TABLE [Production].[TransactionHistoryArchive] ADD CONSTRAINT [DF_TransactionHistoryArchive_TransactionDate] DEFAULT (getdate()) FOR [TransactionDate]
GO
ALTER TABLE [Production].[TransactionHistoryArchive] ADD CONSTRAINT [DF_TransactionHistoryArchive_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
