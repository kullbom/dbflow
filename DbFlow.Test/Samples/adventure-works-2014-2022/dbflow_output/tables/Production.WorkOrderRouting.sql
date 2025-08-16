CREATE TABLE [Production].[WorkOrderRouting] (
   [WorkOrderID] [INT] NOT NULL,
   [ProductID] [INT] NOT NULL,
   [OperationSequence] [SMALLINT] NOT NULL,
   [LocationID] [SMALLINT] NOT NULL,
   [ScheduledStartDate] [DATETIME] NOT NULL,
   [ScheduledEndDate] [DATETIME] NOT NULL,
   [ActualStartDate] [DATETIME] NULL,
   [ActualEndDate] [DATETIME] NULL,
   [ActualResourceHrs] [DECIMAL](9,4) NULL,
   [PlannedCost] [MONEY] NOT NULL,
   [ActualCost] [MONEY] NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_WorkOrderRouting_WorkOrderID_ProductID_OperationSequence] PRIMARY KEY CLUSTERED ([WorkOrderID], [ProductID], [OperationSequence])
)

CREATE NONCLUSTERED INDEX [IX_WorkOrderRouting_ProductID] ON [Production].[WorkOrderRouting] ([ProductID])

GO
