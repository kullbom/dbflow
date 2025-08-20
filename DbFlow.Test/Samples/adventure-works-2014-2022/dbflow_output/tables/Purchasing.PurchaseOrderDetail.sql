CREATE TABLE [Purchasing].[PurchaseOrderDetail] (
   [PurchaseOrderID] [INT] NOT NULL,
   [PurchaseOrderDetailID] [INT] NOT NULL
      IDENTITY (1,1),
   [DueDate] [DATETIME] NOT NULL,
   [OrderQty] [SMALLINT] NOT NULL,
   [ProductID] [INT] NOT NULL,
   [UnitPrice] [MONEY] NOT NULL,
   [LineTotal] AS (isnull([OrderQty]*[UnitPrice],(0.00))),
   [ReceivedQty] [DECIMAL](8,2) NOT NULL,
   [RejectedQty] [DECIMAL](8,2) NOT NULL,
   [StockedQty] AS (isnull([ReceivedQty]-[RejectedQty],(0.00))),
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_PurchaseOrderDetail_PurchaseOrderID_PurchaseOrderDetailID] PRIMARY KEY CLUSTERED ([PurchaseOrderID], [PurchaseOrderDetailID])
)

CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetail_ProductID] ON [Purchasing].[PurchaseOrderDetail] ([ProductID])

GO
