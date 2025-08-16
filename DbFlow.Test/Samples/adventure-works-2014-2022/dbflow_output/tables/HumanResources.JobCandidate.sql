CREATE TABLE [HumanResources].[JobCandidate] (
   [JobCandidateID] [INT] NOT NULL
      IDENTITY (1,1),
   [BusinessEntityID] [INT] NULL,
   [Resume] [XML] NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_JobCandidate_JobCandidateID] PRIMARY KEY CLUSTERED ([JobCandidateID])
)

CREATE NONCLUSTERED INDEX [IX_JobCandidate_BusinessEntityID] ON [HumanResources].[JobCandidate] ([BusinessEntityID])

GO
