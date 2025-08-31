CREATE TABLE [SalesLT].[Product] (
   [ProductID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ProductNumber] [NVARCHAR](25) NOT NULL,
   [Color] [NVARCHAR](15) NULL,
   [StandardCost] [MONEY] NOT NULL,
   [ListPrice] [MONEY] NOT NULL,
   [Size] [NVARCHAR](5) NULL,
   [Weight] [DECIMAL](8,2) NULL,
   [ProductCategoryID] [INT] NULL,
   [ProductModelID] [INT] NULL,
   [SellStartDate] [DATETIME] NOT NULL,
   [SellEndDate] [DATETIME] NULL,
   [DiscontinuedDate] [DATETIME] NULL,
   [ThumbNailPhoto] [VARBINARY](MAX) NULL,
   [ThumbnailPhotoFileName] [NVARCHAR](50) NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Product_ProductID] PRIMARY KEY CLUSTERED ([ProductID])
   ,CONSTRAINT [AK_Product_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [AK_Product_Name] UNIQUE NONCLUSTERED ([Name])
   ,CONSTRAINT [AK_Product_ProductNumber] UNIQUE NONCLUSTERED ([ProductNumber])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Small image file name.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ThumbnailPhotoFileName];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Small image of the product.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ThumbNailPhoto];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the product was discontinued.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [DiscontinuedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the product was no longer available for sale.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [SellEndDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the product was available for sale.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [SellStartDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product is a member of this product model. Foreign key to ProductModel.ProductModelID.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ProductModelID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product is a member of this product category. Foreign key to ProductCategory.ProductCategoryID. ', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ProductCategoryID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product weight.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [Weight];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product size.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [Size];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Selling price.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ListPrice];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Standard cost of the product.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [StandardCost];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product color.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [Color];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique product identification number.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ProductNumber];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Name of the product.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Product records.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'COLUMN', [ProductID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Products sold or used in the manfacturing of sold products.', N'SCHEMA', [SalesLT], N'TABLE', [Product];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key (clustered) constraint', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'INDEX', [PK_Product_ProductID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'INDEX', [AK_Product_rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'INDEX', [AK_Product_Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'INDEX', [AK_Product_ProductNumber];
