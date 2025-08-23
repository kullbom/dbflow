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

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [HumanResources], N'TABLE', [JobCandidate], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Résumé in XML format.', N'SCHEMA', [HumanResources], N'TABLE', [JobCandidate], N'COLUMN', [Resume];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Employee identification number if applicant was hired. Foreign key to Employee.BusinessEntityID.', N'SCHEMA', [HumanResources], N'TABLE', [JobCandidate], N'COLUMN', [BusinessEntityID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for JobCandidate records.', N'SCHEMA', [HumanResources], N'TABLE', [JobCandidate], N'COLUMN', [JobCandidateID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Résumés submitted to Human Resources by job applicants.', N'SCHEMA', [HumanResources], N'TABLE', [JobCandidate], NULL, NULL;
