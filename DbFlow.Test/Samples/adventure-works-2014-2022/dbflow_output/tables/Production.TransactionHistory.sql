CREATE TABLE [Production].[TransactionHistory] (
   [TransactionID] [INT] NOT NULL
      IDENTITY (100000,1),
   [ProductID] [INT] NOT NULL,
   [ReferenceOrderID] [INT] NOT NULL,
   [ReferenceOrderLineID] [INT] NOT NULL
       DEFAULT ((0)),
   [TransactionDate] [DATETIME] NOT NULL
       DEFAULT (getdate()),
   [TransactionType] [NCHAR](1) NOT NULL,
   [Quantity] [INT] NOT NULL,
   [ActualCost] [MONEY] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_TransactionHistory_TransactionID] PRIMARY KEY CLUSTERED ([TransactionID])
)

CREATE NONCLUSTERED INDEX [IX_TransactionHistory_ProductID] ON [Production].[TransactionHistory] ([ProductID])
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_ReferenceOrderID_ReferenceOrderLineID] ON [Production].[TransactionHistory] ([ReferenceOrderID], [ReferenceOrderLineID])

GO
