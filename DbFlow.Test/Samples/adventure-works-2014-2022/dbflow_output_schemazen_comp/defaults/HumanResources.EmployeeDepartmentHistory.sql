ALTER TABLE [HumanResources].[EmployeeDepartmentHistory] ADD CONSTRAINT [DF_EmployeeDepartmentHistory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
