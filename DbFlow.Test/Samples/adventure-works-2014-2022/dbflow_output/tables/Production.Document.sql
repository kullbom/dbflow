CREATE TABLE [Production].[Document] (
   [DocumentNode] [HIERARCHYID] NOT NULL,
   [DocumentLevel] AS ([DocumentNode].[GetLevel]()),
   [Title] [NVARCHAR](50) NOT NULL,
   [Owner] [INT] NOT NULL,
   [FolderFlag] [BIT] NOT NULL,
   [FileName] [NVARCHAR](400) NOT NULL,
   [FileExtension] [NVARCHAR](8) NOT NULL,
   [Revision] [NCHAR](5) NOT NULL,
   [ChangeNumber] [INT] NOT NULL,
   [Status] [TINYINT] NOT NULL,
   [DocumentSummary] [NVARCHAR](MAX) NULL,
   [Document] [VARBINARY](MAX) NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Document_DocumentNode] PRIMARY KEY CLUSTERED ([DocumentNode])
   ,UNIQUE NONCLUSTERED ([rowguid])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Document_DocumentLevel_DocumentNode] ON [Production].[Document] ([DocumentLevel], [DocumentNode])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Document_rowguid] ON [Production].[Document] ([rowguid])
CREATE NONCLUSTERED INDEX [IX_Document_FileName_Revision] ON [Production].[Document] ([FileName], [Revision])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Required for FileStream.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Complete document.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [Document];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Document abstract.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [DocumentSummary];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'1 = Pending approval, 2 = Approved, 3 = Obsolete', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [Status];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Engineering change approval number.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [ChangeNumber];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Revision number of the document. ', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [Revision];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'File extension indicating the document type. For example, .doc or .txt.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [FileExtension];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'File name of the document', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [FileName];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = This is a folder, 1 = This is a document.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [FolderFlag];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Employee who controls the document.  Foreign key to Employee.BusinessEntityID', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [Owner];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Title of the document.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [Title];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Depth in the document hierarchy.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [DocumentLevel];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Document records.', N'SCHEMA', [Production], N'TABLE', [Document], N'COLUMN', [DocumentNode];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product maintenance documents.', N'SCHEMA', [Production], N'TABLE', [Document], NULL, NULL;
