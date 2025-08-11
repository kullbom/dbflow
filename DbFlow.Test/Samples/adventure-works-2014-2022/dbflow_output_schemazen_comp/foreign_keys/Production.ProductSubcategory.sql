ALTER TABLE [Production].[ProductSubcategory] WITH CHECK ADD CONSTRAINT [FK_ProductSubcategory_ProductCategory_ProductCategoryID]
   FOREIGN KEY([ProductCategoryID]) REFERENCES [Production].[ProductCategory] ([ProductCategoryID])

GO
