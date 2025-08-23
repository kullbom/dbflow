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

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Sales], N'TABLE', [SalesTerritoryHistory], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Sales], N'TABLE', [SalesTerritoryHistory], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the sales representative left work in the territory.', N'SCHEMA', [Sales], N'TABLE', [SalesTerritoryHistory], N'COLUMN', [EndDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Date the sales representive started work in the territory.', N'SCHEMA', [Sales], N'TABLE', [SalesTerritoryHistory], N'COLUMN', [StartDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Territory identification number. Foreign key to SalesTerritory.SalesTerritoryID.', N'SCHEMA', [Sales], N'TABLE', [SalesTerritoryHistory], N'COLUMN', [TerritoryID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. The sales rep.  Foreign key to SalesPerson.BusinessEntityID.', N'SCHEMA', [Sales], N'TABLE', [SalesTerritoryHistory], N'COLUMN', [BusinessEntityID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales representative transfers to other sales territories.', N'SCHEMA', [Sales], N'TABLE', [SalesTerritoryHistory], NULL, NULL;
