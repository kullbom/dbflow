ALTER TABLE [Person].[Address] ADD CONSTRAINT [DF_Address_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Person].[Address] ADD CONSTRAINT [DF_Address_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
