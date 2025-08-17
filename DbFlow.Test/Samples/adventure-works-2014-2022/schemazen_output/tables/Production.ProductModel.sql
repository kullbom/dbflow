CREATE TABLE [Production].[ProductModel] (
   [ProductModelID] [int] NOT NULL
      IDENTITY (1,1),
   [Name] [nvarchar](50) NOT NULL,
   [CatalogDescription] [xml] NULL,
   [Instructions] [xml] NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductModel_ProductModelID] PRIMARY KEY CLUSTERED ([ProductModelID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductModel_Name] ON [Production].[ProductModel] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductModel_rowguid] ON [Production].[ProductModel] ([rowguid])
CREATE XML INDEX [PXML_ProductModel_CatalogDescription] ON [Production].[ProductModel] ([CatalogDescription])
CREATE XML INDEX [PXML_ProductModel_Instructions] ON [Production].[ProductModel] ([Instructions])

GO
