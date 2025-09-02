ALTER TABLE [SalesLT].[Address] ADD CONSTRAINT [DF_Address_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()', N'SCHEMA', [SalesLT], N'TABLE', [Address], N'CONSTRAINT', [DF_Address_rowguid];
GO
ALTER TABLE [SalesLT].[Address] ADD CONSTRAINT [DF_Address_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [Address], N'CONSTRAINT', [DF_Address_ModifiedDate];
GO
