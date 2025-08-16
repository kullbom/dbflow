CREATE TABLE [Production].[TransactionHistoryArchive] (
   [TransactionID] [INT] NOT NULL,
   [ProductID] [INT] NOT NULL,
   [ReferenceOrderID] [INT] NOT NULL,
   [ReferenceOrderLineID] [INT] NOT NULL,
   [TransactionDate] [DATETIME] NOT NULL,
   [TransactionType] [NCHAR](1) NOT NULL,
   [Quantity] [INT] NOT NULL,
   [ActualCost] [MONEY] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_TransactionHistoryArchive_TransactionID] PRIMARY KEY CLUSTERED ([TransactionID])
)

CREATE NONCLUSTERED INDEX [IX_TransactionHistoryArchive_ProductID] ON [Production].[TransactionHistoryArchive] ([ProductID])
CREATE NONCLUSTERED INDEX [IX_TransactionHistoryArchive_ReferenceOrderID_ReferenceOrderLineID] ON [Production].[TransactionHistoryArchive] ([ReferenceOrderID], [ReferenceOrderLineID])

GO
