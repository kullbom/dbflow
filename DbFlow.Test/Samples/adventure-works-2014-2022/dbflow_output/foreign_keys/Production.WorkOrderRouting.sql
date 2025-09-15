ALTER TABLE [Production].[WorkOrderRouting] WITH CHECK ADD CONSTRAINT [FK_WorkOrderRouting_WorkOrder_WorkOrderID]
   FOREIGN KEY([WorkOrderID]) REFERENCES [Production].[WorkOrder] ([WorkOrderID])

GO
ALTER TABLE [Production].[WorkOrderRouting] WITH CHECK ADD CONSTRAINT [FK_WorkOrderRouting_Location_LocationID]
   FOREIGN KEY([LocationID]) REFERENCES [Production].[Location] ([LocationID])

GO
