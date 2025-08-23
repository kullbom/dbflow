CREATE SCHEMA [Sales] AUTHORIZATION [dbo]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Contains objects related to customers, sales orders, and sales territories.', N'SCHEMA', [Sales], NULL, NULL, NULL, NULL;
