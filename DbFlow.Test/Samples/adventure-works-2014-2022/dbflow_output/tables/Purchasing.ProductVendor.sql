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

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The product''s unit of measure.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [UnitMeasureCode];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The quantity currently on order.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [OnOrderQty];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The minimum quantity that should be ordered.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [MaxOrderQty];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The maximum quantity that should be ordered.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [MinOrderQty];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the product was last received by the vendor.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [LastReceiptDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The selling price when last purchased.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [LastReceiptCost];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The vendor''s usual selling price.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [StandardPrice];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The average span of time (in days) between placing an order with the vendor and receiving the purchased product.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [AverageLeadTime];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to Vendor.BusinessEntityID.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [BusinessEntityID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to Product.ProductID.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], N'COLUMN', [ProductID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Cross-reference table mapping vendors with the products they supply.', N'SCHEMA', [Purchasing], N'TABLE', [ProductVendor], NULL, NULL;
