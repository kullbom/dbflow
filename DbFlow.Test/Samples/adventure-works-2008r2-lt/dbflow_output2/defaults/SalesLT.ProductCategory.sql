ALTER TABLE [SalesLT].[ProductCategory] ADD CONSTRAINT [DF_ProductCategory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'CONSTRAINT', [DF_ProductCategory_ModifiedDate];
GO
ALTER TABLE [SalesLT].[ProductCategory] ADD CONSTRAINT [DF_ProductCategory_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()()', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'CONSTRAINT', [DF_ProductCategory_rowguid];
GO
