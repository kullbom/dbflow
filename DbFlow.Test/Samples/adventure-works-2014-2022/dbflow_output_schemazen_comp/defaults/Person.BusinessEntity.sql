ALTER TABLE [Person].[BusinessEntity] ADD CONSTRAINT [DF_BusinessEntity_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Person].[BusinessEntity] ADD CONSTRAINT [DF_BusinessEntity_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
