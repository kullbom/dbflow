CREATE TABLE [Sales].[SalesTerritoryHistory] (
   [BusinessEntityID] [INT] NOT NULL,
   [TerritoryID] [INT] NOT NULL,
   [StartDate] [DATETIME] NOT NULL,
   [EndDate] [DATETIME] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SalesTerritoryHistory_BusinessEntityID_StartDate_TerritoryID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [StartDate], [TerritoryID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTerritoryHistory_rowguid] ON [Sales].[SalesTerritoryHistory] ([rowguid])

GO
