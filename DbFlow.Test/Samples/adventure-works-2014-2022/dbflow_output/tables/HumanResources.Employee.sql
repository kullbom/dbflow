CREATE TABLE [HumanResources].[Employee] (
   [BusinessEntityID] [INT] NOT NULL,
   [NationalIDNumber] [NVARCHAR](15) NOT NULL,
   [LoginID] [NVARCHAR](256) NOT NULL,
   [OrganizationNode] [HIERARCHYID] NULL,
   [OrganizationLevel] AS ([OrganizationNode].[GetLevel]()),
   [JobTitle] [NVARCHAR](50) NOT NULL,
   [BirthDate] [DATE] NOT NULL,
   [MaritalStatus] [NCHAR](1) NOT NULL,
   [Gender] [NCHAR](1) NOT NULL,
   [HireDate] [DATE] NOT NULL,
   [SalariedFlag] [FLAG] NOT NULL
       DEFAULT ((1)),
   [VacationHours] [SMALLINT] NOT NULL
       DEFAULT ((0)),
   [SickLeaveHours] [SMALLINT] NOT NULL
       DEFAULT ((0)),
   [CurrentFlag] [FLAG] NOT NULL
       DEFAULT ((1)),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_Employee_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE NONCLUSTERED INDEX [IX_Employee_OrganizationNode] ON [HumanResources].[Employee] ([OrganizationNode])
CREATE NONCLUSTERED INDEX [IX_Employee_OrganizationLevel_OrganizationNode] ON [HumanResources].[Employee] ([OrganizationLevel], [OrganizationNode])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_LoginID] ON [HumanResources].[Employee] ([LoginID])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_NationalIDNumber] ON [HumanResources].[Employee] ([NationalIDNumber])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_rowguid] ON [HumanResources].[Employee] ([rowguid])

GO
