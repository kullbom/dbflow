ALTER TABLE [Production].[WorkOrderRouting] ADD CONSTRAINT [DF_WorkOrderRouting_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
