CREATE TABLE [SalesLT].[ProductCategory] (
   [ProductCategoryID] [INT] NOT NULL
      IDENTITY (1,1),
   [ParentProductCategoryID] [INT] NULL,
   [Name] [NAME] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductCategory_ProductCategoryID] PRIMARY KEY CLUSTERED ([ProductCategoryID])
   ,CONSTRAINT [AK_ProductCategory_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [AK_ProductCategory_Name] UNIQUE NONCLUSTERED ([Name])
)


GO
