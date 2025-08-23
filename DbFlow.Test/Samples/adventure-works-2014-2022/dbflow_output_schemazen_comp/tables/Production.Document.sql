CREATE TABLE [Production].[Document] (
   [DocumentNode] [hierarchyid] NOT NULL,
   [DocumentLevel] AS ([DocumentNode].[GetLevel]()),
   [Title] [nvarchar](50) NOT NULL,
   [Owner] [int] NOT NULL,
   [FolderFlag] [bit] NOT NULL,
   [FileName] [nvarchar](400) NOT NULL,
   [FileExtension] [nvarchar](8) NOT NULL,
   [Revision] [nchar](5) NOT NULL,
   [ChangeNumber] [int] NOT NULL,
   [Status] [tinyint] NOT NULL,
   [DocumentSummary] [nvarchar](max) NULL,
   [Document] [varbinary](max) NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Document_DocumentNode] PRIMARY KEY CLUSTERED ([DocumentNode])
   ,CONSTRAINT [UQ__Document__F73921F728B02C82] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Document_DocumentLevel_DocumentNode] ON [Production].[Document] ([DocumentLevel], [DocumentNode])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Document_rowguid] ON [Production].[Document] ([rowguid])
CREATE NONCLUSTERED INDEX [IX_Document_FileName_Revision] ON [Production].[Document] ([FileName], [Revision])

GO
