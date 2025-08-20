CREATE TABLE [Production].[ProductModelIllustration] (
   [ProductModelID] [INT] NOT NULL,
   [IllustrationID] [INT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductModelIllustration_ProductModelID_IllustrationID] PRIMARY KEY CLUSTERED ([ProductModelID], [IllustrationID])
)


GO
