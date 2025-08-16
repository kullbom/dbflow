CREATE TABLE [Production].[ProductModel] (
   [ProductModelID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [CatalogDescription] [XML] NULL,
   [Instructions] [XML] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductModel_ProductModelID] PRIMARY KEY CLUSTERED ([ProductModelID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductModel_Name] ON [Production].[ProductModel] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductModel_rowguid] ON [Production].[ProductModel] ([rowguid])
CREATE PRIMARY XML INDEX [PXML_ProductModel_CatalogDescription] ON [Production].[ProductModel] ([CatalogDescription])
CREATE PRIMARY XML INDEX [PXML_ProductModel_Instructions] ON [Production].[ProductModel] ([Instructions])

GO
