ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_Freight] DEFAULT ((0.00)) FOR [Freight]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of 0.0', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_Freight];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_ModifiedDate];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_OnlineOrderFlag] DEFAULT ((1)) FOR [OnlineOrderFlag]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of 1 (TRUE)', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_OnlineOrderFlag];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_OrderDate] DEFAULT (getdate()) FOR [OrderDate]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of GETDATE()', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_OrderDate];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_RevisionNumber] DEFAULT ((0)) FOR [RevisionNumber]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of 0', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_RevisionNumber];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_Status] DEFAULT ((1)) FOR [Status]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of 1', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_Status];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_SubTotal] DEFAULT ((0.00)) FOR [SubTotal]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of 0.0', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_SubTotal];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_TaxAmt] DEFAULT ((0.00)) FOR [TaxAmt]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of 0.0', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_TaxAmt];
GO
ALTER TABLE [SalesLT].[SalesOrderHeader] ADD CONSTRAINT [DF_SalesOrderHeader_rowguid] DEFAULT (newid()) FOR [rowguid]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Default constraint value of NEWID()', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'CONSTRAINT', [DF_SalesOrderHeader_rowguid];
GO
