ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [CK_ProductVendor_AverageLeadTime] CHECK ([AverageLeadTime]>=(1))
GO
ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [CK_ProductVendor_LastReceiptCost] CHECK ([LastReceiptCost]>(0.00))
GO
ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [CK_ProductVendor_MaxOrderQty] CHECK ([MaxOrderQty]>=(1))
GO
ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [CK_ProductVendor_MinOrderQty] CHECK ([MinOrderQty]>=(1))
GO
ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [CK_ProductVendor_OnOrderQty] CHECK ([OnOrderQty]>=(0))
GO
ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [CK_ProductVendor_StandardPrice] CHECK ([StandardPrice]>(0.00))
GO
