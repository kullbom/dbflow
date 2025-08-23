CREATE TABLE [Production].[ProductModelIllustration] (
   [ProductModelID] [INT] NOT NULL,
   [IllustrationID] [INT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductModelIllustration_ProductModelID_IllustrationID] PRIMARY KEY CLUSTERED ([ProductModelID], [IllustrationID])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [ProductModelIllustration], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to Illustration.IllustrationID.', N'SCHEMA', [Production], N'TABLE', [ProductModelIllustration], N'COLUMN', [IllustrationID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to ProductModel.ProductModelID.', N'SCHEMA', [Production], N'TABLE', [ProductModelIllustration], N'COLUMN', [ProductModelID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Cross-reference table mapping product models and illustrations.', N'SCHEMA', [Production], N'TABLE', [ProductModelIllustration], NULL, NULL;
