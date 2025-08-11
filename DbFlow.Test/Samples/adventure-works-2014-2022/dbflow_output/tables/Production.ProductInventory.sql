CREATE TABLE [Production].[ProductInventory] (
   [ProductID] [INT] NOT NULL,
   [LocationID] [SMALLINT] NOT NULL,
   [Shelf] [NVARCHAR](10) NOT NULL,
   [Bin] [TINYINT] NOT NULL,
   [Quantity] [SMALLINT] NOT NULL
       DEFAULT ((0)),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_ProductInventory_ProductID_LocationID] PRIMARY KEY CLUSTERED ([ProductID], [LocationID])
)


GO
