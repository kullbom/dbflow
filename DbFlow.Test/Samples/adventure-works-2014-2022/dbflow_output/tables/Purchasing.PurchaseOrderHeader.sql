CREATE TABLE [Purchasing].[PurchaseOrderHeader] (
   [PurchaseOrderID] [INT] NOT NULL
      IDENTITY (1,1),
   [RevisionNumber] [TINYINT] NOT NULL,
   [Status] [TINYINT] NOT NULL,
   [EmployeeID] [INT] NOT NULL,
   [VendorID] [INT] NOT NULL,
   [ShipMethodID] [INT] NOT NULL,
   [OrderDate] [DATETIME] NOT NULL,
   [ShipDate] [DATETIME] NULL,
   [SubTotal] [MONEY] NOT NULL,
   [TaxAmt] [MONEY] NOT NULL,
   [Freight] [MONEY] NOT NULL,
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))) PERSISTED NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_PurchaseOrderHeader_PurchaseOrderID] PRIMARY KEY CLUSTERED ([PurchaseOrderID])
)

CREATE NONCLUSTERED INDEX [IX_PurchaseOrderHeader_VendorID] ON [Purchasing].[PurchaseOrderHeader] ([VendorID])
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderHeader_EmployeeID] ON [Purchasing].[PurchaseOrderHeader] ([EmployeeID])

GO
