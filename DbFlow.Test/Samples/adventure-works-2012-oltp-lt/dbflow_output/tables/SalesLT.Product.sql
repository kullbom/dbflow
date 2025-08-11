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
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_Product_ProductID] PRIMARY KEY CLUSTERED ([ProductID])
   ,CONSTRAINT [AK_Product_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [AK_Product_Name] UNIQUE NONCLUSTERED ([Name])
   ,CONSTRAINT [AK_Product_ProductNumber] UNIQUE NONCLUSTERED ([ProductNumber])
)


GO
