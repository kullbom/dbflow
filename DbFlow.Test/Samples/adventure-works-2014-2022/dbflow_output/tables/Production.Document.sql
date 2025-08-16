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
