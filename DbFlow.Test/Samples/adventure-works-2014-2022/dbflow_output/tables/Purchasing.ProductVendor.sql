CREATE TABLE [Purchasing].[ProductVendor] (
   [ProductID] [INT] NOT NULL,
   [BusinessEntityID] [INT] NOT NULL,
   [AverageLeadTime] [INT] NOT NULL,
   [StandardPrice] [MONEY] NOT NULL,
   [LastReceiptCost] [MONEY] NULL,
   [LastReceiptDate] [DATETIME] NULL,
   [MinOrderQty] [INT] NOT NULL,
   [MaxOrderQty] [INT] NOT NULL,
   [OnOrderQty] [INT] NULL,
   [UnitMeasureCode] [NCHAR](3) NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductVendor_ProductID_BusinessEntityID] PRIMARY KEY CLUSTERED ([ProductID], [BusinessEntityID])
)

CREATE NONCLUSTERED INDEX [IX_ProductVendor_UnitMeasureCode] ON [Purchasing].[ProductVendor] ([UnitMeasureCode])
CREATE NONCLUSTERED INDEX [IX_ProductVendor_BusinessEntityID] ON [Purchasing].[ProductVendor] ([BusinessEntityID])

GO
