ALTER TABLE [HumanResources].[EmployeeDepartmentHistory] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartmentHistory_Department_DepartmentID]
   FOREIGN KEY([DepartmentID]) REFERENCES [HumanResources].[Department] ([DepartmentID])

GO
ALTER TABLE [HumanResources].[EmployeeDepartmentHistory] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartmentHistory_Employee_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [HumanResources].[Employee] ([BusinessEntityID])

GO
ALTER TABLE [HumanResources].[EmployeeDepartmentHistory] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartmentHistory_Shift_ShiftID]
   FOREIGN KEY([ShiftID]) REFERENCES [HumanResources].[Shift] ([ShiftID])

GO
