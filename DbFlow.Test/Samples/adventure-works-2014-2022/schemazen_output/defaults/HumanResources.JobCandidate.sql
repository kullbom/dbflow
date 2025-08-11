ALTER TABLE [HumanResources].[JobCandidate] ADD CONSTRAINT [DF_JobCandidate_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
