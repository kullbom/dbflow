CREATE TABLE [Production].[ProductProductPhoto] (
   [ProductID] [int] NOT NULL,
   [ProductPhotoID] [int] NOT NULL,
   [Primary] [bit] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductProductPhoto_ProductID_ProductPhotoID] PRIMARY KEY NONCLUSTERED ([ProductID], [ProductPhotoID])
)


GO
