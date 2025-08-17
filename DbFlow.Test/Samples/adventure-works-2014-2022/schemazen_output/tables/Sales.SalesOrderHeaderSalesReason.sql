CREATE TABLE [Sales].[SalesOrderHeaderSalesReason] (
   [SalesOrderID] [int] NOT NULL,
   [SalesReasonID] [int] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesOrderHeaderSalesReason_SalesOrderID_SalesReasonID] PRIMARY KEY CLUSTERED ([SalesOrderID], [SalesReasonID])
)


GO
