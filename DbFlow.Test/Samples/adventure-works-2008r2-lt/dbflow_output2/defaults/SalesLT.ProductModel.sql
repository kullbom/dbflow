ALTER TABLE [SalesLT].[ProductModel] ADD CONSTRAINT [DF_ProductModel_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [ProductModel], N'CONSTRAINT', [DF_ProductModel_ModifiedDate];
GO
ALTER TABLE [SalesLT].[ProductModel] ADD CONSTRAINT [DF_ProductModel_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()', N'SCHEMA', [SalesLT], N'TABLE', [ProductModel], N'CONSTRAINT', [DF_ProductModel_rowguid];
GO
