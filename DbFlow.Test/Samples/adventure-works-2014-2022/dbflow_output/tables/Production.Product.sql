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
