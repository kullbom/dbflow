ALTER TABLE [SalesLT].[Address] ADD CONSTRAINT [DF_Address_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [SalesLT].[Address] ADD CONSTRAINT [DF_Address_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
