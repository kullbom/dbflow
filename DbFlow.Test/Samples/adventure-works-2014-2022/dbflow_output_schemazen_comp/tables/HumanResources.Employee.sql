CREATE TABLE [HumanResources].[Employee] (
   [BusinessEntityID] [int] NOT NULL,
   [NationalIDNumber] [nvarchar](15) NOT NULL,
   [LoginID] [nvarchar](256) NOT NULL,
   [OrganizationNode] [hierarchyid] NULL,
   [OrganizationLevel] AS ([OrganizationNode].[GetLevel]()),
   [JobTitle] [nvarchar](50) NOT NULL,
   [BirthDate] [date] NOT NULL,
   [MaritalStatus] [nchar](1) NOT NULL,
   [Gender] [nchar](1) NOT NULL,
   [HireDate] [date] NOT NULL,
   [SalariedFlag] [bit] NOT NULL,
   [VacationHours] [smallint] NOT NULL,
   [SickLeaveHours] [smallint] NOT NULL,
   [CurrentFlag] [bit] NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Employee_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE NONCLUSTERED INDEX [IX_Employee_OrganizationNode] ON [HumanResources].[Employee] ([OrganizationNode])
CREATE NONCLUSTERED INDEX [IX_Employee_OrganizationLevel_OrganizationNode] ON [HumanResources].[Employee] ([OrganizationLevel], [OrganizationNode])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_LoginID] ON [HumanResources].[Employee] ([LoginID])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_NationalIDNumber] ON [HumanResources].[Employee] ([NationalIDNumber])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_rowguid] ON [HumanResources].[Employee] ([rowguid])

GO
