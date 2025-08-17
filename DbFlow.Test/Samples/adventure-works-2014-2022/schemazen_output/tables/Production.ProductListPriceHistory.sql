CREATE TABLE [Production].[ProductListPriceHistory] (
   [ProductID] [int] NOT NULL,
   [StartDate] [datetime] NOT NULL,
   [EndDate] [datetime] NULL,
   [ListPrice] [money] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductListPriceHistory_ProductID_StartDate] PRIMARY KEY CLUSTERED ([ProductID], [StartDate])
)


GO
