ALTER TABLE [SalesLT].[ProductModelProductDescription] ADD CONSTRAINT [DF_ProductModelProductDescription_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'CONSTRAINT', [DF_ProductModelProductDescription_ModifiedDate];
GO
ALTER TABLE [SalesLT].[ProductModelProductDescription] ADD CONSTRAINT [DF_ProductModelProductDescription_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
