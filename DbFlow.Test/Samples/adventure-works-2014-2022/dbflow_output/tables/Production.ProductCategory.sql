CREATE TABLE [Production].[ProductCategory] (
   [ProductCategoryID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductCategory_ProductCategoryID] PRIMARY KEY CLUSTERED ([ProductCategoryID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductCategory_Name] ON [Production].[ProductCategory] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductCategory_rowguid] ON [Production].[ProductCategory] ([rowguid])

GO
