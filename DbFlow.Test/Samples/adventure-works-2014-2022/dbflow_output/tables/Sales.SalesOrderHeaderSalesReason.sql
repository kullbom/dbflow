CREATE TABLE [Sales].[SalesOrderHeaderSalesReason] (
   [SalesOrderID] [INT] NOT NULL,
   [SalesReasonID] [INT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_SalesOrderHeaderSalesReason_SalesOrderID_SalesReasonID] PRIMARY KEY CLUSTERED ([SalesOrderID], [SalesReasonID])
)


GO
