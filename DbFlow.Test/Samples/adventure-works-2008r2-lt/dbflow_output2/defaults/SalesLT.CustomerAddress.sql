ALTER TABLE [SalesLT].[CustomerAddress] ADD CONSTRAINT [DF_CustomerAddress_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [SalesLT].[CustomerAddress] ADD CONSTRAINT [DF_CustomerAddress_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'CONSTRAINT', [DF_CustomerAddress_rowguid];
GO
