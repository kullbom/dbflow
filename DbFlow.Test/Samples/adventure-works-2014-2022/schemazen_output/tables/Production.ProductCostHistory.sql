CREATE TABLE [Production].[ProductCostHistory] (
   [ProductID] [int] NOT NULL,
   [StartDate] [datetime] NOT NULL,
   [EndDate] [datetime] NULL,
   [StandardCost] [money] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductCostHistory_ProductID_StartDate] PRIMARY KEY CLUSTERED ([ProductID], [StartDate])
)


GO
