CREATE TABLE [Sales].[SalesTerritory] (
   [TerritoryID] [int] NOT NULL
      IDENTITY (1,1),
   [Name] [nvarchar](50) NOT NULL,
   [CountryRegionCode] [nvarchar](3) NOT NULL,
   [Group] [nvarchar](50) NOT NULL,
   [SalesYTD] [money] NOT NULL,
   [SalesLastYear] [money] NOT NULL,
   [CostYTD] [money] NOT NULL,
   [CostLastYear] [money] NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesTerritory_TerritoryID] PRIMARY KEY CLUSTERED ([TerritoryID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTerritory_Name] ON [Sales].[SalesTerritory] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTerritory_rowguid] ON [Sales].[SalesTerritory] ([rowguid])

GO
