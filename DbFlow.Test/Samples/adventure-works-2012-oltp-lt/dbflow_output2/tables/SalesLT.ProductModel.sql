CREATE TABLE [SalesLT].[ProductModel] (
   [ProductModelID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [CatalogDescription] [XML] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductModel_ProductModelID] PRIMARY KEY CLUSTERED ([ProductModelID])
   ,CONSTRAINT [AK_ProductModel_Name] UNIQUE NONCLUSTERED ([Name])
   ,CONSTRAINT [AK_ProductModel_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE PRIMARY XML INDEX [PXML_ProductModel_CatalogDescription] ON [SalesLT].[ProductModel] ([CatalogDescription])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key (clustered) constraint', N'SCHEMA', [SalesLT], N'TABLE', [ProductModel], N'INDEX', [PK_ProductModel_ProductModelID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModel], N'INDEX', [AK_ProductModel_Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModel], N'INDEX', [AK_ProductModel_rowguid];
