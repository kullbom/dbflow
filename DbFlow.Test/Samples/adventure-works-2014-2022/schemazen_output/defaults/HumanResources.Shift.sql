ALTER TABLE [HumanResources].[Shift] ADD CONSTRAINT [DF_Shift_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
