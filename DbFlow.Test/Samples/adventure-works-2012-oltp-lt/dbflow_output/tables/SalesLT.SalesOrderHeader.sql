CREATE TABLE [SalesLT].[SalesOrderHeader] (
   [SalesOrderID] [INT] NOT NULL
      IDENTITY (1,1),
   [RevisionNumber] [TINYINT] NOT NULL,
   [OrderDate] [DATETIME] NOT NULL,
   [DueDate] [DATETIME] NOT NULL,
   [ShipDate] [DATETIME] NULL,
   [Status] [TINYINT] NOT NULL,
   [OnlineOrderFlag] [FLAG] NOT NULL,
   [SalesOrderNumber] AS (isnull(N'SO'+CONVERT([nvarchar](23),[SalesOrderID]),N'*** ERROR ***')),
   [PurchaseOrderNumber] [ORDERNUMBER] NULL,
   [AccountNumber] [ACCOUNTNUMBER] NULL,
   [CustomerID] [INT] NOT NULL,
   [ShipToAddressID] [INT] NULL,
   [BillToAddressID] [INT] NULL,
   [ShipMethod] [NVARCHAR](50) NOT NULL,
   [CreditCardApprovalCode] [VARCHAR](15) NULL,
   [SubTotal] [MONEY] NOT NULL,
   [TaxAmt] [MONEY] NOT NULL,
   [Freight] [MONEY] NOT NULL,
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))),
   [Comment] [NVARCHAR](MAX) NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [AK_SalesOrderHeader_SalesOrderNumber] UNIQUE NONCLUSTERED ([SalesOrderNumber])
   ,CONSTRAINT [AK_SalesOrderHeader_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [PK_SalesOrderHeader_SalesOrderID] PRIMARY KEY CLUSTERED ([SalesOrderID])
)

CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_CustomerID] ON [SalesLT].[SalesOrderHeader] ([CustomerID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'General sales order information.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [SalesOrderID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Incremental number to track changes to the sales order over time.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [RevisionNumber];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Dates the sales order was created.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [OrderDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the order is due to the customer.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [DueDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the order was shipped to the customer.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [ShipDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Order current status. 1 = In process; 2 = Approved; 3 = Backordered; 4 = Rejected; 5 = Shipped; 6 = Cancelled', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [Status];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = Order placed by sales person. 1 = Order placed online by customer.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [OnlineOrderFlag];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique sales order identification number.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [SalesOrderNumber];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Customer purchase order number reference. ', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [PurchaseOrderNumber];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Financial accounting number reference.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [AccountNumber];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Customer identification number. Foreign key to Customer.CustomerID.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [CustomerID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The ID of the location to send goods.  Foreign key to the Address table.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [ShipToAddressID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The ID of the location to send invoices.  Foreign key to the Address table.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [BillToAddressID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shipping method. Foreign key to ShipMethod.ShipMethodID.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [ShipMethod];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Approval code provided by the credit card company.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [CreditCardApprovalCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales subtotal. Computed as SUM(SalesOrderDetail.LineTotal)for the appropriate SalesOrderID.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [SubTotal];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Tax amount.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [TaxAmt];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shipping cost.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [Freight];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Total due from customer. Computed as Subtotal + TaxAmt + Freight.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [TotalDue];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales representative comments.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [Comment];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'COLUMN', [ModifiedDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key (clustered) constraint', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'INDEX', [PK_SalesOrderHeader_SalesOrderID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'INDEX', [AK_SalesOrderHeader_rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'INDEX', [AK_SalesOrderHeader_SalesOrderNumber];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Nonclustered index.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'INDEX', [IX_SalesOrderHeader_CustomerID];
