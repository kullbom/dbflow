CREATE TABLE [Sales].[SalesReason] (
   [SalesReasonID] [int] NOT NULL
      IDENTITY (1,1),
   [Name] [nvarchar](50) NOT NULL,
   [ReasonType] [nvarchar](50) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesReason_SalesReasonID] PRIMARY KEY CLUSTERED ([SalesReasonID])
)


GO
