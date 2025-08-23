CREATE TABLE [Production].[ProductModel] (
   [ProductModelID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [CatalogDescription] [XML] NULL,
   [Instructions] [XML] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductModel_ProductModelID] PRIMARY KEY CLUSTERED ([ProductModelID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductModel_Name] ON [Production].[ProductModel] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductModel_rowguid] ON [Production].[ProductModel] ([rowguid])
CREATE PRIMARY XML INDEX [PXML_ProductModel_CatalogDescription] ON [Production].[ProductModel] ([CatalogDescription])
CREATE PRIMARY XML INDEX [PXML_ProductModel_Instructions] ON [Production].[ProductModel] ([Instructions])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [ProductModel], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Production], N'TABLE', [ProductModel], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Manufacturing instructions in xml format.', N'SCHEMA', [Production], N'TABLE', [ProductModel], N'COLUMN', [Instructions];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Detailed product catalog information in xml format.', N'SCHEMA', [Production], N'TABLE', [ProductModel], N'COLUMN', [CatalogDescription];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product model description.', N'SCHEMA', [Production], N'TABLE', [ProductModel], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ProductModel records.', N'SCHEMA', [Production], N'TABLE', [ProductModel], N'COLUMN', [ProductModelID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product model classification.', N'SCHEMA', [Production], N'TABLE', [ProductModel], NULL, NULL;
