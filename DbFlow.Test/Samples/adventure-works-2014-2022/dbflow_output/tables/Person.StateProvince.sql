CREATE TABLE [Person].[StateProvince] (
   [StateProvinceID] [INT] NOT NULL
      IDENTITY (1,1),
   [StateProvinceCode] [NCHAR](3) NOT NULL,
   [CountryRegionCode] [NVARCHAR](3) NOT NULL,
   [IsOnlyStateProvinceFlag] [FLAG] NOT NULL,
   [Name] [NAME] NOT NULL,
   [TerritoryID] [INT] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_StateProvince_StateProvinceID] PRIMARY KEY CLUSTERED ([StateProvinceID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_StateProvince_Name] ON [Person].[StateProvince] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_StateProvince_StateProvinceCode_CountryRegionCode] ON [Person].[StateProvince] ([StateProvinceCode], [CountryRegionCode])
CREATE UNIQUE NONCLUSTERED INDEX [AK_StateProvince_rowguid] ON [Person].[StateProvince] ([rowguid])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'State and province lookup table.', N'SCHEMA', [Person], N'TABLE', [StateProvince];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for StateProvince records.', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [StateProvinceID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ISO standard state or province code.', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [StateProvinceCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ISO standard country or region code. Foreign key to CountryRegion.CountryRegionCode. ', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [CountryRegionCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = StateProvinceCode exists. 1 = StateProvinceCode unavailable, using CountryRegionCode.', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [IsOnlyStateProvinceFlag];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'State or province description.', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ID of the territory in which the state or province is located. Foreign key to SalesTerritory.SalesTerritoryID.', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [TerritoryID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Person], N'TABLE', [StateProvince], N'COLUMN', [ModifiedDate];
