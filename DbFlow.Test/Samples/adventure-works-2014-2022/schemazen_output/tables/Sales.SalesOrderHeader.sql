CREATE TABLE [Sales].[SalesOrderHeader] (
   [SalesOrderID] [int] NOT NULL
      IDENTITY (1,1),
   [RevisionNumber] [tinyint] NOT NULL,
   [OrderDate] [datetime] NOT NULL,
   [DueDate] [datetime] NOT NULL,
   [ShipDate] [datetime] NULL,
   [Status] [tinyint] NOT NULL,
   [OnlineOrderFlag] [bit] NOT NULL,
   [SalesOrderNumber] AS (isnull(N'SO'+CONVERT([nvarchar](23),[SalesOrderID]),N'*** ERROR ***')),
   [PurchaseOrderNumber] [nvarchar](25) NULL,
   [AccountNumber] [nvarchar](15) NULL,
   [CustomerID] [int] NOT NULL,
   [SalesPersonID] [int] NULL,
   [TerritoryID] [int] NULL,
   [BillToAddressID] [int] NOT NULL,
   [ShipToAddressID] [int] NOT NULL,
   [ShipMethodID] [int] NOT NULL,
   [CreditCardID] [int] NULL,
   [CreditCardApprovalCode] [varchar](15) NULL,
   [CurrencyRateID] [int] NULL,
   [SubTotal] [money] NOT NULL,
   [TaxAmt] [money] NOT NULL,
   [Freight] [money] NOT NULL,
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))),
   [Comment] [nvarchar](128) NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesOrderHeader_SalesOrderID] PRIMARY KEY CLUSTERED ([SalesOrderID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesOrderHeader_rowguid] ON [Sales].[SalesOrderHeader] ([rowguid])
CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesOrderHeader_SalesOrderNumber] ON [Sales].[SalesOrderHeader] ([SalesOrderNumber])
CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_CustomerID] ON [Sales].[SalesOrderHeader] ([CustomerID])
CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_SalesPersonID] ON [Sales].[SalesOrderHeader] ([SalesPersonID])

GO
