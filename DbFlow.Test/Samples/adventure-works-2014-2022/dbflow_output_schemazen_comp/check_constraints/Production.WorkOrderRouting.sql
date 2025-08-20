ALTER TABLE [Production].[WorkOrderRouting] WITH CHECK ADD CONSTRAINT [CK_WorkOrderRouting_ActualCost] CHECK  ([ActualCost]>(0.00))
GO
ALTER TABLE [Production].[WorkOrderRouting] WITH CHECK ADD CONSTRAINT [CK_WorkOrderRouting_PlannedCost] CHECK  ([PlannedCost]>(0.00))
GO
ALTER TABLE [Production].[WorkOrderRouting] WITH CHECK ADD CONSTRAINT [CK_WorkOrderRouting_ActualResourceHrs] CHECK  ([ActualResourceHrs]>=(0.0000))
GO
ALTER TABLE [Production].[WorkOrderRouting] WITH CHECK ADD CONSTRAINT [CK_WorkOrderRouting_ActualEndDate] CHECK  ([ActualEndDate]>=[ActualStartDate] OR [ActualEndDate] IS NULL OR [ActualStartDate] IS NULL)
GO
ALTER TABLE [Production].[WorkOrderRouting] WITH CHECK ADD CONSTRAINT [CK_WorkOrderRouting_ScheduledEndDate] CHECK  ([ScheduledEndDate]>=[ScheduledStartDate])
GO
