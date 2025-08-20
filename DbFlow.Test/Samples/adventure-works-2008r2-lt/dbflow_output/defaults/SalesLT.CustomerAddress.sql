ALTER TABLE [SalesLT].[CustomerAddress] ADD CONSTRAINT [DF_CustomerAddress_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [SalesLT].[CustomerAddress] ADD CONSTRAINT [DF_CustomerAddress_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
