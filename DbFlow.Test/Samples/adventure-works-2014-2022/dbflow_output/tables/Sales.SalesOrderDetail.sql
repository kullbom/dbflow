CREATE TABLE [Sales].[SalesOrderDetail] (
   [SalesOrderID] [INT] NOT NULL,
   [SalesOrderDetailID] [INT] NOT NULL
      IDENTITY (1,1),
   [CarrierTrackingNumber] [NVARCHAR](25) NULL,
   [OrderQty] [SMALLINT] NOT NULL,
   [ProductID] [INT] NOT NULL,
   [SpecialOfferID] [INT] NOT NULL,
   [UnitPrice] [MONEY] NOT NULL,
   [UnitPriceDiscount] [MONEY] NOT NULL
       DEFAULT ((0.0)),
   [LineTotal] AS (isnull(([UnitPrice]*((1.0)-[UnitPriceDiscount]))*[OrderQty],(0.0))),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_SalesOrderDetail_SalesOrderID_SalesOrderDetailID] PRIMARY KEY CLUSTERED ([SalesOrderID], [SalesOrderDetailID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesOrderDetail_rowguid] ON [Sales].[SalesOrderDetail] ([rowguid])
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetail_ProductID] ON [Sales].[SalesOrderDetail] ([ProductID])

GO
