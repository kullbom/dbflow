CREATE TABLE [HumanResources].[JobCandidate] (
   [JobCandidateID] [int] NOT NULL
      IDENTITY (1,1),
   [BusinessEntityID] [int] NULL,
   [Resume] [xml] NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_JobCandidate_JobCandidateID] PRIMARY KEY CLUSTERED ([JobCandidateID])
)

CREATE NONCLUSTERED INDEX [IX_JobCandidate_BusinessEntityID] ON [HumanResources].[JobCandidate] ([BusinessEntityID])

GO
