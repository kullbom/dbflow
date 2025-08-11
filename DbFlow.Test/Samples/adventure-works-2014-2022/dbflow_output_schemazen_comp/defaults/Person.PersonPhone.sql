ALTER TABLE [Person].[PersonPhone] ADD CONSTRAINT [DF_PersonPhone_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
