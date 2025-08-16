CREATE TABLE [Sales].[SalesOrderHeader] (
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
   [SalesPersonID] [INT] NULL,
   [TerritoryID] [INT] NULL,
   [BillToAddressID] [INT] NOT NULL,
   [ShipToAddressID] [INT] NOT NULL,
   [ShipMethodID] [INT] NOT NULL,
   [CreditCardID] [INT] NULL,
   [CreditCardApprovalCode] [VARCHAR](15) NULL,
   [CurrencyRateID] [INT] NULL,
   [SubTotal] [MONEY] NOT NULL,
   [TaxAmt] [MONEY] NOT NULL,
   [Freight] [MONEY] NOT NULL,
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))),
   [Comment] [NVARCHAR](128) NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SalesOrderHeader_SalesOrderID] PRIMARY KEY CLUSTERED ([SalesOrderID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesOrderHeader_rowguid] ON [Sales].[SalesOrderHeader] ([rowguid])
CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesOrderHeader_SalesOrderNumber] ON [Sales].[SalesOrderHeader] ([SalesOrderNumber])
CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_CustomerID] ON [Sales].[SalesOrderHeader] ([CustomerID])
CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_SalesPersonID] ON [Sales].[SalesOrderHeader] ([SalesPersonID])

GO
