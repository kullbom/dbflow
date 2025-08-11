ALTER TABLE [Production].[Document] ADD CONSTRAINT [DF_Document_FolderFlag] DEFAULT ((0)) FOR [FolderFlag]
GO
ALTER TABLE [Production].[Document] ADD CONSTRAINT [DF_Document_ChangeNumber] DEFAULT ((0)) FOR [ChangeNumber]
GO
ALTER TABLE [Production].[Document] ADD CONSTRAINT [DF_Document_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Production].[Document] ADD CONSTRAINT [DF_Document_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
