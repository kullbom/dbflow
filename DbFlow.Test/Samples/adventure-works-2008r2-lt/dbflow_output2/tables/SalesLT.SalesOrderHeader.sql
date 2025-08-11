CREATE TABLE [SalesLT].[SalesOrderHeader] (
   [SalesOrderID] [INT] NOT NULL
      IDENTITY (1,1),
   [RevisionNumber] [TINYINT] NOT NULL
       DEFAULT ((0)),
   [OrderDate] [DATETIME] NOT NULL
       DEFAULT (getdate()),
   [DueDate] [DATETIME] NOT NULL,
   [ShipDate] [DATETIME] NULL,
   [Status] [TINYINT] NOT NULL
       DEFAULT ((1)),
   [OnlineOrderFlag] [FLAG] NOT NULL
       DEFAULT ((1)),
   [SalesOrderNumber] AS (isnull(N'SO'+CONVERT([nvarchar](23),[SalesOrderID]),N'*** ERROR ***')),
   [PurchaseOrderNumber] [ORDERNUMBER] NULL,
   [AccountNumber] [ACCOUNTNUMBER] NULL,
   [CustomerID] [INT] NOT NULL,
   [ShipToAddressID] [INT] NULL,
   [BillToAddressID] [INT] NULL,
   [ShipMethod] [NVARCHAR](50) NOT NULL,
   [CreditCardApprovalCode] [VARCHAR](15) NULL,
   [SubTotal] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [TaxAmt] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [Freight] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))),
   [Comment] [NVARCHAR](MAX) NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_SalesOrderHeader_SalesOrderID] PRIMARY KEY CLUSTERED ([SalesOrderID])
   ,CONSTRAINT [AK_SalesOrderHeader_SalesOrderNumber] UNIQUE NONCLUSTERED ([SalesOrderNumber])
   ,CONSTRAINT [AK_SalesOrderHeader_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_CustomerID] ON [SalesLT].[SalesOrderHeader] ([CustomerID])

GO
