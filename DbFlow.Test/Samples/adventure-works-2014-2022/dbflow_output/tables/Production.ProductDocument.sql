CREATE TABLE [Production].[ProductDocument] (
   [ProductID] [INT] NOT NULL,
   [DocumentNode] [HIERARCHYID] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductDocument_ProductID_DocumentNode] PRIMARY KEY CLUSTERED ([ProductID], [DocumentNode])
)


GO
