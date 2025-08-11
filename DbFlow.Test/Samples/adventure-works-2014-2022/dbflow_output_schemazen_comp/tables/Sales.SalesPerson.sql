CREATE TABLE [Sales].[SalesPerson] (
   [BusinessEntityID] [int] NOT NULL,
   [TerritoryID] [int] NULL,
   [SalesQuota] [money] NULL,
   [Bonus] [money] NOT NULL,
   [CommissionPct] [smallmoney] NOT NULL,
   [SalesYTD] [money] NOT NULL,
   [SalesLastYear] [money] NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesPerson_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesPerson_rowguid] ON [Sales].[SalesPerson] ([rowguid])

GO
