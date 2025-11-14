ALTER TABLE [Person].[AddressType] ADD CONSTRAINT [DF_AddressType_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Person].[AddressType] ADD CONSTRAINT [DF_AddressType_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
