CREATE TABLE [Production].[ProductDocument] (
   [ProductID] [int] NOT NULL,
   [DocumentNode] [hierarchyid] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductDocument_ProductID_DocumentNode] PRIMARY KEY CLUSTERED ([ProductID], [DocumentNode])
)


GO
