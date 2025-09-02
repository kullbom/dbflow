CREATE TABLE [Production].[ProductPhoto] (
   [ProductPhotoID] [INT] NOT NULL
      IDENTITY (1,1),
   [ThumbNailPhoto] [VARBINARY](MAX) NULL,
   [ThumbnailPhotoFileName] [NVARCHAR](50) NULL,
   [LargePhoto] [VARBINARY](MAX) NULL,
   [LargePhotoFileName] [NVARCHAR](50) NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductPhoto_ProductPhotoID] PRIMARY KEY CLUSTERED ([ProductPhotoID])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product images.', N'SCHEMA', [Production], N'TABLE', [ProductPhoto];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ProductPhoto records.', N'SCHEMA', [Production], N'TABLE', [ProductPhoto], N'COLUMN', [ProductPhotoID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Small image of the product.', N'SCHEMA', [Production], N'TABLE', [ProductPhoto], N'COLUMN', [ThumbNailPhoto];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Small image file name.', N'SCHEMA', [Production], N'TABLE', [ProductPhoto], N'COLUMN', [ThumbnailPhotoFileName];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Large image of the product.', N'SCHEMA', [Production], N'TABLE', [ProductPhoto], N'COLUMN', [LargePhoto];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Large image file name.', N'SCHEMA', [Production], N'TABLE', [ProductPhoto], N'COLUMN', [LargePhotoFileName];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [ProductPhoto], N'COLUMN', [ModifiedDate];
