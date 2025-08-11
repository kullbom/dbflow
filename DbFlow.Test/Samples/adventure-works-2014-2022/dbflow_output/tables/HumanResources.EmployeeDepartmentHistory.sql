CREATE TABLE [HumanResources].[EmployeeDepartmentHistory] (
   [BusinessEntityID] [INT] NOT NULL,
   [DepartmentID] [SMALLINT] NOT NULL,
   [ShiftID] [TINYINT] NOT NULL,
   [StartDate] [DATE] NOT NULL,
   [EndDate] [DATE] NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_EmployeeDepartmentHistory_BusinessEntityID_StartDate_DepartmentID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [StartDate], [DepartmentID], [ShiftID])
)

CREATE NONCLUSTERED INDEX [IX_EmployeeDepartmentHistory_DepartmentID] ON [HumanResources].[EmployeeDepartmentHistory] ([DepartmentID])
CREATE NONCLUSTERED INDEX [IX_EmployeeDepartmentHistory_ShiftID] ON [HumanResources].[EmployeeDepartmentHistory] ([ShiftID])

GO
