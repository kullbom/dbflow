CREATE TABLE [SalesLT].[ProductModelProductDescription] (
   [ProductModelID] [INT] NOT NULL,
   [ProductDescriptionID] [INT] NOT NULL,
   [Culture] [NCHAR](6) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductModelProductDescription_ProductModelID_ProductDescriptionID_Culture] PRIMARY KEY CLUSTERED ([ProductModelID], [ProductDescriptionID], [Culture])
   ,CONSTRAINT [AK_ProductModelProductDescription_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The culture for which the description is written', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'COLUMN', [Culture];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to ProductDescription.ProductDescriptionID.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'COLUMN', [ProductDescriptionID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to ProductModel.ProductModelID.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'COLUMN', [ProductModelID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Cross-reference table mapping product descriptions and the language the description is written in.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], NULL, NULL;
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Clustered index created by a primary key constraint.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'INDEX', [PK_ProductModelProductDescription_ProductModelID_ProductDescriptionID_Culture];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'INDEX', [AK_ProductModelProductDescription_rowguid];
