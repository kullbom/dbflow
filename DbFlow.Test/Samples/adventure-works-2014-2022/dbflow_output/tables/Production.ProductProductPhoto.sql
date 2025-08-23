CREATE TABLE [Production].[ProductProductPhoto] (
   [ProductID] [INT] NOT NULL,
   [ProductPhotoID] [INT] NOT NULL,
   [Primary] [FLAG] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductProductPhoto_ProductID_ProductPhotoID] PRIMARY KEY NONCLUSTERED ([ProductID], [ProductPhotoID])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [ProductProductPhoto], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = Photo is not the principal image. 1 = Photo is the principal image.', N'SCHEMA', [Production], N'TABLE', [ProductProductPhoto], N'COLUMN', [Primary];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product photo identification number. Foreign key to ProductPhoto.ProductPhotoID.', N'SCHEMA', [Production], N'TABLE', [ProductProductPhoto], N'COLUMN', [ProductPhotoID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product identification number. Foreign key to Product.ProductID.', N'SCHEMA', [Production], N'TABLE', [ProductProductPhoto], N'COLUMN', [ProductID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Cross-reference table mapping products and product photos.', N'SCHEMA', [Production], N'TABLE', [ProductProductPhoto], NULL, NULL;
