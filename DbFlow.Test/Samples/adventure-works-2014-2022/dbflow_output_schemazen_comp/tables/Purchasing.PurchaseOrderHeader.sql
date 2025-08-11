CREATE TABLE [Purchasing].[PurchaseOrderHeader] (
   [PurchaseOrderID] [int] NOT NULL
      IDENTITY (1,1),
   [RevisionNumber] [tinyint] NOT NULL,
   [Status] [tinyint] NOT NULL,
   [EmployeeID] [int] NOT NULL,
   [VendorID] [int] NOT NULL,
   [ShipMethodID] [int] NOT NULL,
   [OrderDate] [datetime] NOT NULL,
   [ShipDate] [datetime] NULL,
   [SubTotal] [money] NOT NULL,
   [TaxAmt] [money] NOT NULL,
   [Freight] [money] NOT NULL,
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))) PERSISTED,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_PurchaseOrderHeader_PurchaseOrderID] PRIMARY KEY CLUSTERED ([PurchaseOrderID])
)

CREATE NONCLUSTERED INDEX [IX_PurchaseOrderHeader_VendorID] ON [Purchasing].[PurchaseOrderHeader] ([VendorID])
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderHeader_EmployeeID] ON [Purchasing].[PurchaseOrderHeader] ([EmployeeID])

GO
