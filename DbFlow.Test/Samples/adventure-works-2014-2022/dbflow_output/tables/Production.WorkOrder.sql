CREATE TABLE [Production].[WorkOrder] (
   [WorkOrderID] [INT] NOT NULL
      IDENTITY (1,1),
   [ProductID] [INT] NOT NULL,
   [OrderQty] [INT] NOT NULL,
   [StockedQty] AS (isnull([OrderQty]-[ScrappedQty],(0))),
   [ScrappedQty] [SMALLINT] NOT NULL,
   [StartDate] [DATETIME] NOT NULL,
   [EndDate] [DATETIME] NULL,
   [DueDate] [DATETIME] NOT NULL,
   [ScrapReasonID] [SMALLINT] NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_WorkOrder_WorkOrderID] PRIMARY KEY CLUSTERED ([WorkOrderID])
)

CREATE NONCLUSTERED INDEX [IX_WorkOrder_ScrapReasonID] ON [Production].[WorkOrder] ([ScrapReasonID])
CREATE NONCLUSTERED INDEX [IX_WorkOrder_ProductID] ON [Production].[WorkOrder] ([ProductID])

GO
