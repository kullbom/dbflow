CREATE TABLE [Production].[TransactionHistory] (
   [TransactionID] [int] NOT NULL
      IDENTITY (100000,1),
   [ProductID] [int] NOT NULL,
   [ReferenceOrderID] [int] NOT NULL,
   [ReferenceOrderLineID] [int] NOT NULL,
   [TransactionDate] [datetime] NOT NULL,
   [TransactionType] [nchar](1) NOT NULL,
   [Quantity] [int] NOT NULL,
   [ActualCost] [money] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_TransactionHistory_TransactionID] PRIMARY KEY CLUSTERED ([TransactionID])
)

CREATE NONCLUSTERED INDEX [IX_TransactionHistory_ProductID] ON [Production].[TransactionHistory] ([ProductID])
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_ReferenceOrderID_ReferenceOrderLineID] ON [Production].[TransactionHistory] ([ReferenceOrderID], [ReferenceOrderLineID])

GO
