ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [FK_ProductVendor_Vendor_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Purchasing].[Vendor] ([BusinessEntityID])

GO
ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [FK_ProductVendor_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
ALTER TABLE [Purchasing].[ProductVendor] WITH CHECK ADD CONSTRAINT [FK_ProductVendor_UnitMeasure_UnitMeasureCode]
   FOREIGN KEY([UnitMeasureCode]) REFERENCES [Production].[UnitMeasure] ([UnitMeasureCode])

GO
