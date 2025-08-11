CREATE TABLE [SalesLT].[ProductCategory] (
   [ProductCategoryID] [int] NOT NULL
      IDENTITY (1,1),
   [ParentProductCategoryID] [int] NULL,
   [Name] [nvarchar](50) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [AK_ProductCategory_Name] UNIQUE NONCLUSTERED ([Name])
   ,CONSTRAINT [AK_ProductCategory_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [PK_ProductCategory_ProductCategoryID] PRIMARY KEY CLUSTERED ([ProductCategoryID])
)


GO
