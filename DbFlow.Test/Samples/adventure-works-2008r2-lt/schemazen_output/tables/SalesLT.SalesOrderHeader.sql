CREATE TABLE [SalesLT].[SalesOrderHeader] (
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
   [ShipToAddressID] [int] NULL,
   [BillToAddressID] [int] NULL,
   [ShipMethod] [nvarchar](50) NOT NULL,
   [CreditCardApprovalCode] [varchar](15) NULL,
   [SubTotal] [money] NOT NULL,
   [TaxAmt] [money] NOT NULL,
   [Freight] [money] NOT NULL,
   [TotalDue] AS (isnull(([SubTotal]+[TaxAmt])+[Freight],(0))),
   [Comment] [nvarchar](max) NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [AK_SalesOrderHeader_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [AK_SalesOrderHeader_SalesOrderNumber] UNIQUE NONCLUSTERED ([SalesOrderNumber])
   ,CONSTRAINT [PK_SalesOrderHeader_SalesOrderID] PRIMARY KEY CLUSTERED ([SalesOrderID])
)

CREATE NONCLUSTERED INDEX [IX_SalesOrderHeader_CustomerID] ON [SalesLT].[SalesOrderHeader] ([CustomerID])

GO
