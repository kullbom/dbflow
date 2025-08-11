CREATE TABLE [HumanResources].[EmployeeDepartmentHistory] (
   [BusinessEntityID] [int] NOT NULL,
   [DepartmentID] [smallint] NOT NULL,
   [ShiftID] [tinyint] NOT NULL,
   [StartDate] [date] NOT NULL,
   [EndDate] [date] NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_EmployeeDepartmentHistory_BusinessEntityID_StartDate_DepartmentID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [StartDate], [DepartmentID], [ShiftID])
)

CREATE NONCLUSTERED INDEX [IX_EmployeeDepartmentHistory_DepartmentID] ON [HumanResources].[EmployeeDepartmentHistory] ([DepartmentID])
CREATE NONCLUSTERED INDEX [IX_EmployeeDepartmentHistory_ShiftID] ON [HumanResources].[EmployeeDepartmentHistory] ([ShiftID])

GO
