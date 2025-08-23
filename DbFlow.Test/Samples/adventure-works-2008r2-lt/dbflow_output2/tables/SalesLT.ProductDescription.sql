CREATE TABLE [SalesLT].[ProductDescription] (
   [ProductDescriptionID] [INT] NOT NULL
      IDENTITY (1,1),
   [Description] [NVARCHAR](400) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductDescription_ProductDescriptionID] PRIMARY KEY CLUSTERED ([ProductDescriptionID])
   ,CONSTRAINT [AK_ProductDescription_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Description of the product.', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'COLUMN', [Description];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ProductDescription records.', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'COLUMN', [ProductDescriptionID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product descriptions in several languages.', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], NULL, NULL;
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Clustered index created by a primary key constraint.', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'INDEX', [PK_ProductDescription_ProductDescriptionID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'INDEX', [AK_ProductDescription_rowguid];
