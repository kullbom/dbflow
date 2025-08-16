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

   ,CONSTRAINT [PK_SalesOrderHeader_SalesOrderID] PRIMARY KEY CLUSTERED ([SalesOrderID])
   ,CONSTRAINT [AK_SalesOrderHeader_SalesOrderNumber] UNIQUE NONCLUSTERED ([SalesOrderNumber])
   ,CONSTRAINT [AK_SalesOrderHeader_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_CustomerID] ON [SalesLT].[SalesOrderHeader] ([CustomerID])

GO
