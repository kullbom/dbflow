CREATE TABLE [Production].[Product] (
   [ProductID] [int] NOT NULL
      IDENTITY (1,1),
   [Name] [nvarchar](50) NOT NULL,
   [ProductNumber] [nvarchar](25) NOT NULL,
   [MakeFlag] [bit] NOT NULL,
   [FinishedGoodsFlag] [bit] NOT NULL,
   [Color] [nvarchar](15) NULL,
   [SafetyStockLevel] [smallint] NOT NULL,
   [ReorderPoint] [smallint] NOT NULL,
   [StandardCost] [money] NOT NULL,
   [ListPrice] [money] NOT NULL,
   [Size] [nvarchar](5) NULL,
   [SizeUnitMeasureCode] [nchar](3) NULL,
   [WeightUnitMeasureCode] [nchar](3) NULL,
   [Weight] [decimal](8,2) NULL,
   [DaysToManufacture] [int] NOT NULL,
   [ProductLine] [nchar](2) NULL,
   [Class] [nchar](2) NULL,
   [Style] [nchar](2) NULL,
   [ProductSubcategoryID] [int] NULL,
   [ProductModelID] [int] NULL,
   [SellStartDate] [datetime] NOT NULL,
   [SellEndDate] [datetime] NULL,
   [DiscontinuedDate] [datetime] NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Product_ProductID] PRIMARY KEY CLUSTERED ([ProductID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Product_Name] ON [Production].[Product] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Product_ProductNumber] ON [Production].[Product] ([ProductNumber])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Product_rowguid] ON [Production].[Product] ([rowguid])

GO
