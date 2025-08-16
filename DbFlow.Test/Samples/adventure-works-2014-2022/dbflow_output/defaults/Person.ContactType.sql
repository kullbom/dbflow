ALTER TABLE [Person].[ContactType] ADD CONSTRAINT [DF_ContactType_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
