CREATE TABLE [Sales].[SalesOrderDetail] (
   [SalesOrderID] [int] NOT NULL,
   [SalesOrderDetailID] [int] NOT NULL
      IDENTITY (1,1),
   [CarrierTrackingNumber] [nvarchar](25) NULL,
   [OrderQty] [smallint] NOT NULL,
   [ProductID] [int] NOT NULL,
   [SpecialOfferID] [int] NOT NULL,
   [UnitPrice] [money] NOT NULL,
   [UnitPriceDiscount] [money] NOT NULL,
   [LineTotal] AS (isnull(([UnitPrice]*((1.0)-[UnitPriceDiscount]))*[OrderQty],(0.0))),
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesOrderDetail_SalesOrderID_SalesOrderDetailID] PRIMARY KEY CLUSTERED ([SalesOrderID], [SalesOrderDetailID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesOrderDetail_rowguid] ON [Sales].[SalesOrderDetail] ([rowguid])
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetail_ProductID] ON [Sales].[SalesOrderDetail] ([ProductID])

GO
