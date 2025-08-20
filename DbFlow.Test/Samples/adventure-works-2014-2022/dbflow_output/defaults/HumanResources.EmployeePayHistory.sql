ALTER TABLE [HumanResources].[EmployeePayHistory] ADD CONSTRAINT [DF_EmployeePayHistory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
