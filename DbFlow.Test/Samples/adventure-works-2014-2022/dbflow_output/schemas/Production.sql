CREATE SCHEMA [Production] AUTHORIZATION [dbo]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Contains objects related to products, inventory, and manufacturing.', N'SCHEMA', [Production];
