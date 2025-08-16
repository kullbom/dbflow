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
