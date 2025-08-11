CREATE TABLE [Production].[WorkOrder] (
   [WorkOrderID] [int] NOT NULL
      IDENTITY (1,1),
   [ProductID] [int] NOT NULL,
   [OrderQty] [int] NOT NULL,
   [StockedQty] AS (isnull([OrderQty]-[ScrappedQty],(0))),
   [ScrappedQty] [smallint] NOT NULL,
   [StartDate] [datetime] NOT NULL,
   [EndDate] [datetime] NULL,
   [DueDate] [datetime] NOT NULL,
   [ScrapReasonID] [smallint] NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_WorkOrder_WorkOrderID] PRIMARY KEY CLUSTERED ([WorkOrderID])
)

CREATE NONCLUSTERED INDEX [IX_WorkOrder_ProductID] ON [Production].[WorkOrder] ([ProductID])
CREATE NONCLUSTERED INDEX [IX_WorkOrder_ScrapReasonID] ON [Production].[WorkOrder] ([ScrapReasonID])

GO
