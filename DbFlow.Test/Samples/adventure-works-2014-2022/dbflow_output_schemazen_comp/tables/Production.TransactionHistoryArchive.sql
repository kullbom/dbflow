CREATE TABLE [Production].[TransactionHistoryArchive] (
   [TransactionID] [int] NOT NULL,
   [ProductID] [int] NOT NULL,
   [ReferenceOrderID] [int] NOT NULL,
   [ReferenceOrderLineID] [int] NOT NULL,
   [TransactionDate] [datetime] NOT NULL,
   [TransactionType] [nchar](1) NOT NULL,
   [Quantity] [int] NOT NULL,
   [ActualCost] [money] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_TransactionHistoryArchive_TransactionID] PRIMARY KEY CLUSTERED ([TransactionID])
)

CREATE NONCLUSTERED INDEX [IX_TransactionHistoryArchive_ProductID] ON [Production].[TransactionHistoryArchive] ([ProductID])
CREATE NONCLUSTERED INDEX [IX_TransactionHistoryArchive_ReferenceOrderID_ReferenceOrderLineID] ON [Production].[TransactionHistoryArchive] ([ReferenceOrderID], [ReferenceOrderLineID])

GO
