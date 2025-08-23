CREATE SCHEMA [Purchasing] AUTHORIZATION [dbo]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Contains objects related to vendors and purchase orders.', N'SCHEMA', [Purchasing], NULL, NULL, NULL, NULL;
