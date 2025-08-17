CREATE TABLE [HumanResources].[EmployeePayHistory] (
   [BusinessEntityID] [int] NOT NULL,
   [RateChangeDate] [datetime] NOT NULL,
   [Rate] [money] NOT NULL,
   [PayFrequency] [tinyint] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_EmployeePayHistory_BusinessEntityID_RateChangeDate] PRIMARY KEY CLUSTERED ([BusinessEntityID], [RateChangeDate])
)


GO
