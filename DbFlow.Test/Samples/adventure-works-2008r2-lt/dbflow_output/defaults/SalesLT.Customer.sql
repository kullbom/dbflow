ALTER TABLE [SalesLT].[Customer] ADD CONSTRAINT [DF_Customer_NameStyle] DEFAULT ((0)) FOR [NameStyle]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of 0', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'CONSTRAINT', [DF_Customer_NameStyle];
GO
ALTER TABLE [SalesLT].[Customer] ADD CONSTRAINT [DF_Customer_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'CONSTRAINT', [DF_Customer_rowguid];
GO
ALTER TABLE [SalesLT].[Customer] ADD CONSTRAINT [DF_Customer_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'CONSTRAINT', [DF_Customer_ModifiedDate];
GO
