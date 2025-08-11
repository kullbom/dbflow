ALTER TABLE [Production].[WorkOrder] WITH CHECK ADD CONSTRAINT [CK_WorkOrder_EndDate] CHECK  ([EndDate]>=[StartDate] OR [EndDate] IS NULL)
GO
ALTER TABLE [Production].[WorkOrder] WITH CHECK ADD CONSTRAINT [CK_WorkOrder_OrderQty] CHECK  ([OrderQty]>(0))
GO
ALTER TABLE [Production].[WorkOrder] WITH CHECK ADD CONSTRAINT [CK_WorkOrder_ScrappedQty] CHECK  ([ScrappedQty]>=(0))
GO
