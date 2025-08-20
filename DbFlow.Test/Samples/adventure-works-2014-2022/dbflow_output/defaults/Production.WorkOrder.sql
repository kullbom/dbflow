ALTER TABLE [Production].[WorkOrder] ADD CONSTRAINT [DF_WorkOrder_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
