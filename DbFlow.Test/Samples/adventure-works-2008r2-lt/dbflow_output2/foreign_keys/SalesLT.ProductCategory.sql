ALTER TABLE [SalesLT].[ProductCategory] WITH CHECK ADD CONSTRAINT [FK_ProductCategory_ProductCategory_ParentProductCategoryID_ProductCategoryID]
   FOREIGN KEY([ParentProductCategoryID]) REFERENCES [SalesLT].[ProductCategory] ([ProductCategoryID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing ProductCategory.ProductCategoryID.', N'SCHEMA', [SalesLT], N'TABLE', [ProductCategory], N'CONSTRAINT', [FK_ProductCategory_ProductCategory_ParentProductCategoryID_ProductCategoryID];

GO
