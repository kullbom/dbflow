CREATE TABLE [Purchasing].[PurchaseOrderDetail] (
   [PurchaseOrderID] [int] NOT NULL,
   [PurchaseOrderDetailID] [int] NOT NULL
      IDENTITY (1,1),
   [DueDate] [datetime] NOT NULL,
   [OrderQty] [smallint] NOT NULL,
   [ProductID] [int] NOT NULL,
   [UnitPrice] [money] NOT NULL,
   [LineTotal] AS (isnull([OrderQty]*[UnitPrice],(0.00))),
   [ReceivedQty] [decimal](8,2) NOT NULL,
   [RejectedQty] [decimal](8,2) NOT NULL,
   [StockedQty] AS (isnull([ReceivedQty]-[RejectedQty],(0.00))),
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_PurchaseOrderDetail_PurchaseOrderID_PurchaseOrderDetailID] PRIMARY KEY CLUSTERED ([PurchaseOrderID], [PurchaseOrderDetailID])
)

CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetail_ProductID] ON [Purchasing].[PurchaseOrderDetail] ([ProductID])

GO
