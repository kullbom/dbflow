CREATE TABLE [Production].[ProductPhoto] (
   [ProductPhotoID] [int] NOT NULL
      IDENTITY (1,1),
   [ThumbNailPhoto] [varbinary](max) NULL,
   [ThumbnailPhotoFileName] [nvarchar](50) NULL,
   [LargePhoto] [varbinary](max) NULL,
   [LargePhotoFileName] [nvarchar](50) NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductPhoto_ProductPhotoID] PRIMARY KEY CLUSTERED ([ProductPhotoID])
)


GO
