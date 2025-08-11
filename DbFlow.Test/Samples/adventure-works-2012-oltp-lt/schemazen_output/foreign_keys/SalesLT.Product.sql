ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_ProductCategory_ProductCategoryID]
   FOREIGN KEY([ProductCategoryID]) REFERENCES [SalesLT].[ProductCategory] ([ProductCategoryID])

GO
ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_ProductModel_ProductModelID]
   FOREIGN KEY([ProductModelID]) REFERENCES [SalesLT].[ProductModel] ([ProductModelID])

GO
