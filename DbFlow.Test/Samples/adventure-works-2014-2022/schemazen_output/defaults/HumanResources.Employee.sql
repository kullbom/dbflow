ALTER TABLE [HumanResources].[Employee] ADD CONSTRAINT [DF_Employee_SalariedFlag] DEFAULT ((1)) FOR [SalariedFlag]
GO
ALTER TABLE [HumanResources].[Employee] ADD CONSTRAINT [DF_Employee_VacationHours] DEFAULT ((0)) FOR [VacationHours]
GO
ALTER TABLE [HumanResources].[Employee] ADD CONSTRAINT [DF_Employee_SickLeaveHours] DEFAULT ((0)) FOR [SickLeaveHours]
GO
ALTER TABLE [HumanResources].[Employee] ADD CONSTRAINT [DF_Employee_CurrentFlag] DEFAULT ((1)) FOR [CurrentFlag]
GO
ALTER TABLE [HumanResources].[Employee] ADD CONSTRAINT [DF_Employee_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [HumanResources].[Employee] ADD CONSTRAINT [DF_Employee_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
