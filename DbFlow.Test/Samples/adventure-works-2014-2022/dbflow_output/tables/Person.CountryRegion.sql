CREATE TABLE [Person].[CountryRegion] (
   [CountryRegionCode] [NVARCHAR](3) NOT NULL,
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_CountryRegion_CountryRegionCode] PRIMARY KEY CLUSTERED ([CountryRegionCode])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_CountryRegion_Name] ON [Person].[CountryRegion] ([Name])

GO
