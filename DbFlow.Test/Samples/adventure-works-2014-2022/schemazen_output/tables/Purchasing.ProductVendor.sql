CREATE TABLE [Purchasing].[ProductVendor] (
   [ProductID] [int] NOT NULL,
   [BusinessEntityID] [int] NOT NULL,
   [AverageLeadTime] [int] NOT NULL,
   [StandardPrice] [money] NOT NULL,
   [LastReceiptCost] [money] NULL,
   [LastReceiptDate] [datetime] NULL,
   [MinOrderQty] [int] NOT NULL,
   [MaxOrderQty] [int] NOT NULL,
   [OnOrderQty] [int] NULL,
   [UnitMeasureCode] [nchar](3) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductVendor_ProductID_BusinessEntityID] PRIMARY KEY CLUSTERED ([ProductID], [BusinessEntityID])
)

CREATE NONCLUSTERED INDEX [IX_ProductVendor_BusinessEntityID] ON [Purchasing].[ProductVendor] ([BusinessEntityID])
CREATE NONCLUSTERED INDEX [IX_ProductVendor_UnitMeasureCode] ON [Purchasing].[ProductVendor] ([UnitMeasureCode])

GO
