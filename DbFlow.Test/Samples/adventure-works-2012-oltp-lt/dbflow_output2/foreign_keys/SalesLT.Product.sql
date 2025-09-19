ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_ProductModel_ProductModelID]
   FOREIGN KEY([ProductModelID]) REFERENCES [SalesLT].[ProductModel] ([ProductModelID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing ProductModel.ProductModelID.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'CONSTRAINT', [FK_Product_ProductModel_ProductModelID];
GO
ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_ProductCategory_ProductCategoryID]
   FOREIGN KEY([ProductCategoryID]) REFERENCES [SalesLT].[ProductCategory] ([ProductCategoryID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing ProductCategory.ProductCategoryID.', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'CONSTRAINT', [FK_Product_ProductCategory_ProductCategoryID];
GO
