CREATE TABLE [Purchasing].[PurchaseOrderHeader] (
   [PurchaseOrderID] [INT] NOT NULL
      IDENTITY (1,1),
   [RevisionNumber] [TINYINT] NOT NULL
       DEFAULT ((0)),
   [Status] [TINYINT] NOT NULL
       DEFAULT ((1)),
   [EmployeeID] [INT] NOT NULL,
   [VendorID] [INT] NOT NULL,
   [ShipMethodID] [INT] NOT NULL,
   [OrderDate] [DATETIME] NOT NULL
       DEFAULT (getdate()),
   [ShipDate] [DATETIME] NULL,
   [SubTotal] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [TaxAmt] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [Freight] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))) PERSISTED,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_PurchaseOrderHeader_PurchaseOrderID] PRIMARY KEY CLUSTERED ([PurchaseOrderID])
)

CREATE NONCLUSTERED INDEX [IX_PurchaseOrderHeader_VendorID] ON [Purchasing].[PurchaseOrderHeader] ([VendorID])
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderHeader_EmployeeID] ON [Purchasing].[PurchaseOrderHeader] ([EmployeeID])

GO
