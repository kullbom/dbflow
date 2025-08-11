ALTER TABLE [Production].[Culture] ADD CONSTRAINT [DF_Culture_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
