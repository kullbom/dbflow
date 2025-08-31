CREATE SCHEMA [Person] AUTHORIZATION [dbo]
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Contains objects related to names and addresses of customers, vendors, and employees', N'SCHEMA', [Person];
