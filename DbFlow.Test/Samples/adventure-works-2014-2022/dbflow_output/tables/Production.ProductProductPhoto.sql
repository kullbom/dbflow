CREATE TABLE [Production].[ProductProductPhoto] (
   [ProductID] [INT] NOT NULL,
   [ProductPhotoID] [INT] NOT NULL,
   [Primary] [FLAG] NOT NULL
       DEFAULT ((0)),
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_ProductProductPhoto_ProductID_ProductPhotoID] PRIMARY KEY NONCLUSTERED ([ProductID], [ProductPhotoID])
)


GO
