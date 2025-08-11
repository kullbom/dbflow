ALTER TABLE [SalesLT].[ProductCategory] WITH CHECK ADD CONSTRAINT [FK_ProductCategory_ProductCategory_ParentProductCategoryID_ProductCategoryID]
   FOREIGN KEY([ParentProductCategoryID]) REFERENCES [SalesLT].[ProductCategory] ([ProductCategoryID])

GO
