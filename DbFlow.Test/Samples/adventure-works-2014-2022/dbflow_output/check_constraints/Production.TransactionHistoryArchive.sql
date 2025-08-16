ALTER TABLE [Production].[TransactionHistoryArchive] WITH CHECK ADD CONSTRAINT [CK_TransactionHistoryArchive_TransactionType] CHECK (upper([TransactionType])='P' OR upper([TransactionType])='S' OR upper([TransactionType])='W')
GO
