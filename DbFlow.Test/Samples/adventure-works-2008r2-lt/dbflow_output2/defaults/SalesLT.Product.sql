ALTER TABLE [SalesLT].[Product] ADD CONSTRAINT [DF_Product_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'CONSTRAINT', [DF_Product_rowguid];

GO
ALTER TABLE [SalesLT].[Product] ADD CONSTRAINT [DF_Product_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'CONSTRAINT', [DF_Product_ModifiedDate];

GO
