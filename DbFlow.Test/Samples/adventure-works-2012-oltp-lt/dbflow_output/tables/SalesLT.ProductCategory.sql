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

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Category description.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product category identification number of immediate ancestor category. Foreign key to ProductCategory.ProductCategoryID.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'COLUMN', [ParentProductCategoryID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ProductCategory records.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'COLUMN', [ProductCategoryID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'High-level product categorization.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key (clustered) constraint', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'INDEX', [PK_ProductCategory_ProductCategoryID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'INDEX', [AK_ProductCategory_rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'INDEX', [AK_ProductCategory_Name];
