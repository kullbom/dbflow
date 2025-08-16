ALTER TABLE [Person].[EmailAddress] ADD CONSTRAINT [DF_EmailAddress_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Person].[EmailAddress] ADD CONSTRAINT [DF_EmailAddress_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
