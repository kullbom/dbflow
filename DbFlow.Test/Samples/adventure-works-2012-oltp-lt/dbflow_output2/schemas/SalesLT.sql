CREATE SCHEMA [SalesLT] AUTHORIZATION [dbo]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Contains objects related to products, customers, sales orders, and sales territories.', N'SCHEMA', [SalesLT], NULL, NULL, NULL, NULL;
