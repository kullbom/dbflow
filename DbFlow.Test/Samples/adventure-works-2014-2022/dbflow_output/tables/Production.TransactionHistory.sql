CREATE TABLE [Production].[TransactionHistory] (
   [TransactionID] [INT] NOT NULL
      IDENTITY (100000,1),
   [ProductID] [INT] NOT NULL,
   [ReferenceOrderID] [INT] NOT NULL,
   [ReferenceOrderLineID] [INT] NOT NULL,
   [TransactionDate] [DATETIME] NOT NULL,
   [TransactionType] [NCHAR](1) NOT NULL,
   [Quantity] [INT] NOT NULL,
   [ActualCost] [MONEY] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_TransactionHistory_TransactionID] PRIMARY KEY CLUSTERED ([TransactionID])
)

CREATE NONCLUSTERED INDEX [IX_TransactionHistory_ProductID] ON [Production].[TransactionHistory] ([ProductID])
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_ReferenceOrderID_ReferenceOrderLineID] ON [Production].[TransactionHistory] ([ReferenceOrderID], [ReferenceOrderLineID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Record of each purchase order, sales order, or work order transaction year to date.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for TransactionHistory records.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [TransactionID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product identification number. Foreign key to Product.ProductID.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [ProductID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Purchase order, sales order, or work order identification number.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [ReferenceOrderID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Line number associated with the purchase order, sales order, or work order.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [ReferenceOrderLineID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time of the transaction.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [TransactionDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'W = WorkOrder, S = SalesOrder, P = PurchaseOrder', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [TransactionType];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product quantity.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [Quantity];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product cost.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [ActualCost];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [TransactionHistory], N'COLUMN', [ModifiedDate];
