CREATE TABLE [Sales].[SalesTerritoryHistory] (
   [BusinessEntityID] [int] NOT NULL,
   [TerritoryID] [int] NOT NULL,
   [StartDate] [datetime] NOT NULL,
   [EndDate] [datetime] NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesTerritoryHistory_BusinessEntityID_StartDate_TerritoryID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [StartDate], [TerritoryID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTerritoryHistory_rowguid] ON [Sales].[SalesTerritoryHistory] ([rowguid])

GO
