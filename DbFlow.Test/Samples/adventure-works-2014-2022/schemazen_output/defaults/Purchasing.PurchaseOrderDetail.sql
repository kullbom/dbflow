ALTER TABLE [Purchasing].[PurchaseOrderDetail] ADD CONSTRAINT [DF_PurchaseOrderDetail_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
