ALTER TABLE [Person].[Password] ADD CONSTRAINT [DF_Password_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Person].[Password] ADD CONSTRAINT [DF_Password_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
