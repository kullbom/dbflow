CREATE UNIQUE CLUSTERED INDEX [IX_vProductAndDescription] ON [SalesLT].[vProductAndDescription] ([Culture], [ProductID])
GO
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Clustered index on the view vProductAndDescription.', N'SCHEMA', [SalesLT], N'VIEW', [vProductAndDescription], N'INDEX', [IX_vProductAndDescription];
