ALTER TABLE [Person].[BusinessEntityAddress] ADD CONSTRAINT [DF_BusinessEntityAddress_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Person].[BusinessEntityAddress] ADD CONSTRAINT [DF_BusinessEntityAddress_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
