CREATE TABLE [Sales].[SalesPerson] (
   [BusinessEntityID] [INT] NOT NULL,
   [TerritoryID] [INT] NULL,
   [SalesQuota] [MONEY] NULL,
   [Bonus] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [CommissionPct] [SMALLMONEY] NOT NULL
       DEFAULT ((0.00)),
   [SalesYTD] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [SalesLastYear] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_SalesPerson_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesPerson_rowguid] ON [Sales].[SalesPerson] ([rowguid])

GO
