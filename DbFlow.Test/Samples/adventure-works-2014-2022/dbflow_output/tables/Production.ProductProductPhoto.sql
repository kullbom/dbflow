CREATE TABLE [Production].[ProductProductPhoto] (
   [ProductID] [INT] NOT NULL,
   [ProductPhotoID] [INT] NOT NULL,
   [Primary] [FLAG] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductProductPhoto_ProductID_ProductPhotoID] PRIMARY KEY NONCLUSTERED ([ProductID], [ProductPhotoID])
)


GO
