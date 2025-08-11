CREATE TABLE [Production].[WorkOrderRouting] (
   [WorkOrderID] [int] NOT NULL,
   [ProductID] [int] NOT NULL,
   [OperationSequence] [smallint] NOT NULL,
   [LocationID] [smallint] NOT NULL,
   [ScheduledStartDate] [datetime] NOT NULL,
   [ScheduledEndDate] [datetime] NOT NULL,
   [ActualStartDate] [datetime] NULL,
   [ActualEndDate] [datetime] NULL,
   [ActualResourceHrs] [decimal](9,4) NULL,
   [PlannedCost] [money] NOT NULL,
   [ActualCost] [money] NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_WorkOrderRouting_WorkOrderID_ProductID_OperationSequence] PRIMARY KEY CLUSTERED ([WorkOrderID], [ProductID], [OperationSequence])
)

CREATE NONCLUSTERED INDEX [IX_WorkOrderRouting_ProductID] ON [Production].[WorkOrderRouting] ([ProductID])

GO
