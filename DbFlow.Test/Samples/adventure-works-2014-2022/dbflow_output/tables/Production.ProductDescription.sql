CREATE TABLE [Production].[ProductDescription] (
   [ProductDescriptionID] [INT] NOT NULL
      IDENTITY (1,1),
   [Description] [NVARCHAR](400) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductDescription_ProductDescriptionID] PRIMARY KEY CLUSTERED ([ProductDescriptionID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductDescription_rowguid] ON [Production].[ProductDescription] ([rowguid])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [ProductDescription], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Production], N'TABLE', [ProductDescription], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Description of the product.', N'SCHEMA', [Production], N'TABLE', [ProductDescription], N'COLUMN', [Description];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ProductDescription records.', N'SCHEMA', [Production], N'TABLE', [ProductDescription], N'COLUMN', [ProductDescriptionID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product descriptions in several languages.', N'SCHEMA', [Production], N'TABLE', [ProductDescription], NULL, NULL;
