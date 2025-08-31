CREATE TYPE [dbo].[DateTime2Utc1] FROM [DATETIME2](3) NULL
GO

EXECUTE [sys].[sp_addextendedproperty] N'Foobar1', N'The value 123', N'SCHEMA', [dbo], N'TYPE', [DateTime2Utc1];
