CREATE TABLE [Production].[ProductModelIllustration] (
   [ProductModelID] [int] NOT NULL,
   [IllustrationID] [int] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductModelIllustration_ProductModelID_IllustrationID] PRIMARY KEY CLUSTERED ([ProductModelID], [IllustrationID])
)


GO
