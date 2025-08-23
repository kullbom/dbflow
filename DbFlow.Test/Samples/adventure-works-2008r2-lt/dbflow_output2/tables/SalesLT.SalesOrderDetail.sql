CREATE TABLE [SalesLT].[SalesOrderDetail] (
   [SalesOrderID] [INT] NOT NULL,
   [SalesOrderDetailID] [INT] NOT NULL
      IDENTITY (1,1),
   [OrderQty] [SMALLINT] NOT NULL,
   [ProductID] [INT] NOT NULL,
   [UnitPrice] [MONEY] NOT NULL,
   [UnitPriceDiscount] [MONEY] NOT NULL,
   [LineTotal] AS (isnull(([UnitPrice]*((1.0)-[UnitPriceDiscount]))*[OrderQty],(0.0))),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SalesOrderDetail_SalesOrderID_SalesOrderDetailID] PRIMARY KEY CLUSTERED ([SalesOrderID], [SalesOrderDetailID])
   ,CONSTRAINT [AK_SalesOrderDetail_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_SalesOrderDetail_ProductID] ON [SalesLT].[SalesOrderDetail] ([ProductID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Per product subtotal. Computed as UnitPrice * (1 - UnitPriceDiscount) * OrderQty.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [LineTotal];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Discount amount.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [UnitPriceDiscount];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Selling price of a single product.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [UnitPrice];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product sold to customer. Foreign key to Product.ProductID.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [ProductID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Quantity ordered per product.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [OrderQty];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. One incremental unique number per product sold.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [SalesOrderDetailID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to SalesOrderHeader.SalesOrderID.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'COLUMN', [SalesOrderID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Individual products associated with a specific sales order. See SalesOrderHeader.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], NULL, NULL;
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Clustered index created by a primary key constraint.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'INDEX', [PK_SalesOrderDetail_SalesOrderID_SalesOrderDetailID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'INDEX', [AK_SalesOrderDetail_rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Nonclustered index.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'INDEX', [IX_SalesOrderDetail_ProductID];
