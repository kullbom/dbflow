ALTER TABLE [Purchasing].[PurchaseOrderHeader] ADD CONSTRAINT [DF_PurchaseOrderHeader_RevisionNumber] DEFAULT ((0)) FOR [RevisionNumber]
GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] ADD CONSTRAINT [DF_PurchaseOrderHeader_Status] DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] ADD CONSTRAINT [DF_PurchaseOrderHeader_OrderDate] DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] ADD CONSTRAINT [DF_PurchaseOrderHeader_SubTotal] DEFAULT ((0.00)) FOR [SubTotal]
GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] ADD CONSTRAINT [DF_PurchaseOrderHeader_TaxAmt] DEFAULT ((0.00)) FOR [TaxAmt]
GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] ADD CONSTRAINT [DF_PurchaseOrderHeader_Freight] DEFAULT ((0.00)) FOR [Freight]
GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] ADD CONSTRAINT [DF_PurchaseOrderHeader_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
