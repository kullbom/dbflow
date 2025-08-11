CREATE TABLE [SalesLT].[SalesOrderDetail] (
   [SalesOrderID] [INT] NOT NULL,
   [SalesOrderDetailID] [INT] NOT NULL
      IDENTITY (1,1),
   [OrderQty] [SMALLINT] NOT NULL,
   [ProductID] [INT] NOT NULL,
   [UnitPrice] [MONEY] NOT NULL,
   [UnitPriceDiscount] [MONEY] NOT NULL
       DEFAULT ((0.0)),
   [LineTotal] AS (isnull(([UnitPrice]*((1.0)-[UnitPriceDiscount]))*[OrderQty],(0.0))),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_SalesOrderDetail_SalesOrderID_SalesOrderDetailID] PRIMARY KEY CLUSTERED ([SalesOrderID], [SalesOrderDetailID])
   ,CONSTRAINT [AK_SalesOrderDetail_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_SalesOrderDetail_ProductID] ON [SalesLT].[SalesOrderDetail] ([ProductID])

GO
