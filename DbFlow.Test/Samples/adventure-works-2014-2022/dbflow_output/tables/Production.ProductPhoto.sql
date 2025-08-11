CREATE TABLE [Production].[ProductPhoto] (
   [ProductPhotoID] [INT] NOT NULL
      IDENTITY (1,1),
   [ThumbNailPhoto] [VARBINARY](MAX) NULL,
   [ThumbnailPhotoFileName] [NVARCHAR](50) NULL,
   [LargePhoto] [VARBINARY](MAX) NULL,
   [LargePhotoFileName] [NVARCHAR](50) NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_ProductPhoto_ProductPhotoID] PRIMARY KEY CLUSTERED ([ProductPhotoID])
)


GO
