CREATE TABLE [SalesLT].[SalesOrderDetail] (
   [SalesOrderID] [int] NOT NULL,
   [SalesOrderDetailID] [int] NOT NULL
      IDENTITY (1,1),
   [OrderQty] [smallint] NOT NULL,
   [ProductID] [int] NOT NULL,
   [UnitPrice] [money] NOT NULL,
   [UnitPriceDiscount] [money] NOT NULL,
   [LineTotal] AS (isnull(([UnitPrice]*((1.0)-[UnitPriceDiscount]))*[OrderQty],(0.0))),
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [AK_SalesOrderDetail_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [PK_SalesOrderDetail_SalesOrderID_SalesOrderDetailID] PRIMARY KEY CLUSTERED ([SalesOrderID], [SalesOrderDetailID])
)

CREATE NONCLUSTERED INDEX [IX_SalesOrderDetail_ProductID] ON [SalesLT].[SalesOrderDetail] ([ProductID])

GO
