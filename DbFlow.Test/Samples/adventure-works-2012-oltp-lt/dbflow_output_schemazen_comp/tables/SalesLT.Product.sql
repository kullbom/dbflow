CREATE TABLE [SalesLT].[Product] (
   [ProductID] [int] NOT NULL
      IDENTITY (1,1),
   [Name] [nvarchar](50) NOT NULL,
   [ProductNumber] [nvarchar](25) NOT NULL,
   [Color] [nvarchar](15) NULL,
   [StandardCost] [money] NOT NULL,
   [ListPrice] [money] NOT NULL,
   [Size] [nvarchar](5) NULL,
   [Weight] [decimal](8,2) NULL,
   [ProductCategoryID] [int] NULL,
   [ProductModelID] [int] NULL,
   [SellStartDate] [datetime] NOT NULL,
   [SellEndDate] [datetime] NULL,
   [DiscontinuedDate] [datetime] NULL,
   [ThumbNailPhoto] [varbinary](max) NULL,
   [ThumbnailPhotoFileName] [nvarchar](50) NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Product_ProductID] PRIMARY KEY CLUSTERED ([ProductID])
   ,CONSTRAINT [AK_Product_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [AK_Product_Name] UNIQUE NONCLUSTERED ([Name])
   ,CONSTRAINT [AK_Product_ProductNumber] UNIQUE NONCLUSTERED ([ProductNumber])
)


GO
