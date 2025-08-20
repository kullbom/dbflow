ALTER TABLE [HumanResources].[Department] ADD CONSTRAINT [DF_Department_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
