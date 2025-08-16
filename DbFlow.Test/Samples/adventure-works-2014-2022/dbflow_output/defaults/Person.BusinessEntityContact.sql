ALTER TABLE [Person].[BusinessEntityContact] ADD CONSTRAINT [DF_BusinessEntityContact_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Person].[BusinessEntityContact] ADD CONSTRAINT [DF_BusinessEntityContact_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
