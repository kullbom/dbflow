CREATE TABLE [Person].[StateProvince] (
   [StateProvinceID] [int] NOT NULL
      IDENTITY (1,1),
   [StateProvinceCode] [nchar](3) NOT NULL,
   [CountryRegionCode] [nvarchar](3) NOT NULL,
   [IsOnlyStateProvinceFlag] [bit] NOT NULL,
   [Name] [nvarchar](50) NOT NULL,
   [TerritoryID] [int] NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_StateProvince_StateProvinceID] PRIMARY KEY CLUSTERED ([StateProvinceID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_StateProvince_Name] ON [Person].[StateProvince] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_StateProvince_StateProvinceCode_CountryRegionCode] ON [Person].[StateProvince] ([StateProvinceCode], [CountryRegionCode])
CREATE UNIQUE NONCLUSTERED INDEX [AK_StateProvince_rowguid] ON [Person].[StateProvince] ([rowguid])

GO
