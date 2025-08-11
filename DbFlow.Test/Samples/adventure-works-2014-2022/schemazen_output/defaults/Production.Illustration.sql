ALTER TABLE [Production].[Illustration] ADD CONSTRAINT [DF_Illustration_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
