ALTER TABLE [SalesLT].[ProductDescription] ADD CONSTRAINT [DF_ProductDescription_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'CONSTRAINT', [DF_ProductDescription_ModifiedDate];
GO
ALTER TABLE [SalesLT].[ProductDescription] ADD CONSTRAINT [DF_ProductDescription_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()', N'SCHEMA', [SalesLT], N'TABLE', [ProductDescription], N'CONSTRAINT', [DF_ProductDescription_rowguid];
GO
