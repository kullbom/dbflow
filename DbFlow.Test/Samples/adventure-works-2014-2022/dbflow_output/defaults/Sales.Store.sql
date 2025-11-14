ALTER TABLE [Sales].[Store] ADD CONSTRAINT [DF_Store_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[Store] ADD CONSTRAINT [DF_Store_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
