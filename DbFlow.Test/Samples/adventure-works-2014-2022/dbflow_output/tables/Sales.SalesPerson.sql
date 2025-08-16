CREATE TABLE [Sales].[SalesPerson] (
   [BusinessEntityID] [INT] NOT NULL,
   [TerritoryID] [INT] NULL,
   [SalesQuota] [MONEY] NULL,
   [Bonus] [MONEY] NOT NULL,
   [CommissionPct] [SMALLMONEY] NOT NULL,
   [SalesYTD] [MONEY] NOT NULL,
   [SalesLastYear] [MONEY] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SalesPerson_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesPerson_rowguid] ON [Sales].[SalesPerson] ([rowguid])

GO
