CREATE TABLE [Production].[Product] (
   [ProductID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ProductNumber] [NVARCHAR](25) NOT NULL,
   [MakeFlag] [FLAG] NOT NULL,
   [FinishedGoodsFlag] [FLAG] NOT NULL,
   [Color] [NVARCHAR](15) NULL,
   [SafetyStockLevel] [SMALLINT] NOT NULL,
   [ReorderPoint] [SMALLINT] NOT NULL,
   [StandardCost] [MONEY] NOT NULL,
   [ListPrice] [MONEY] NOT NULL,
   [Size] [NVARCHAR](5) NULL,
   [SizeUnitMeasureCode] [NCHAR](3) NULL,
   [WeightUnitMeasureCode] [NCHAR](3) NULL,
   [Weight] [DECIMAL](8,2) NULL,
   [DaysToManufacture] [INT] NOT NULL,
   [ProductLine] [NCHAR](2) NULL,
   [Class] [NCHAR](2) NULL,
   [Style] [NCHAR](2) NULL,
   [ProductSubcategoryID] [INT] NULL,
   [ProductModelID] [INT] NULL,
   [SellStartDate] [DATETIME] NOT NULL,
   [SellEndDate] [DATETIME] NULL,
   [DiscontinuedDate] [DATETIME] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Product_ProductID] PRIMARY KEY CLUSTERED ([ProductID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Product_ProductNumber] ON [Production].[Product] ([ProductNumber])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Product_Name] ON [Production].[Product] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Product_rowguid] ON [Production].[Product] ([rowguid])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Products sold or used in the manfacturing of sold products.', N'SCHEMA', [Production], N'TABLE', [Product];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Product records.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ProductID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Name of the product.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique product identification number.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ProductNumber];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = Product is purchased, 1 = Product is manufactured in-house.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [MakeFlag];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = Product is not a salable item. 1 = Product is salable.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [FinishedGoodsFlag];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product color.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [Color];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Minimum inventory quantity. ', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [SafetyStockLevel];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Inventory level that triggers a purchase order or work order. ', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ReorderPoint];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Standard cost of the product.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [StandardCost];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Selling price.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ListPrice];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product size.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [Size];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unit of measure for Size column.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [SizeUnitMeasureCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unit of measure for Weight column.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [WeightUnitMeasureCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product weight.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [Weight];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Number of days required to manufacture the product.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [DaysToManufacture];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'R = Road, M = Mountain, T = Touring, S = Standard', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ProductLine];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'H = High, M = Medium, L = Low', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [Class];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'W = Womens, M = Mens, U = Universal', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [Style];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product is a member of this product subcategory. Foreign key to ProductSubCategory.ProductSubCategoryID. ', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ProductSubcategoryID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product is a member of this product model. Foreign key to ProductModel.ProductModelID.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ProductModelID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the product was available for sale.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [SellStartDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the product was no longer available for sale.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [SellEndDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the product was discontinued.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [DiscontinuedDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [Product], N'COLUMN', [ModifiedDate];
