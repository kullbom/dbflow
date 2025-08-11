CREATE TABLE [Sales].[SalesTerritory] (
   [TerritoryID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [CountryRegionCode] [NVARCHAR](3) NOT NULL,
   [Group] [NVARCHAR](50) NOT NULL,
   [SalesYTD] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [SalesLastYear] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [CostYTD] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [CostLastYear] [MONEY] NOT NULL
       DEFAULT ((0.00)),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_SalesTerritory_TerritoryID] PRIMARY KEY CLUSTERED ([TerritoryID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTerritory_Name] ON [Sales].[SalesTerritory] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTerritory_rowguid] ON [Sales].[SalesTerritory] ([rowguid])

GO
