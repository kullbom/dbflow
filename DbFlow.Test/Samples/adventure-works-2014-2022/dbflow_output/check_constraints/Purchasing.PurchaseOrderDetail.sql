ALTER TABLE [Purchasing].[PurchaseOrderDetail] WITH CHECK ADD CONSTRAINT [CK_PurchaseOrderDetail_OrderQty] CHECK ([OrderQty]>(0))
GO
ALTER TABLE [Purchasing].[PurchaseOrderDetail] WITH CHECK ADD CONSTRAINT [CK_PurchaseOrderDetail_ReceivedQty] CHECK ([ReceivedQty]>=(0.00))
GO
ALTER TABLE [Purchasing].[PurchaseOrderDetail] WITH CHECK ADD CONSTRAINT [CK_PurchaseOrderDetail_RejectedQty] CHECK ([RejectedQty]>=(0.00))
GO
ALTER TABLE [Purchasing].[PurchaseOrderDetail] WITH CHECK ADD CONSTRAINT [CK_PurchaseOrderDetail_UnitPrice] CHECK ([UnitPrice]>=(0.00))
GO
