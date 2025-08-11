ALTER TABLE [SalesLT].[ProductModelProductDescription] WITH CHECK ADD CONSTRAINT [FK_ProductModelProductDescription_ProductDescription_ProductDescriptionID]
   FOREIGN KEY([ProductDescriptionID]) REFERENCES [SalesLT].[ProductDescription] ([ProductDescriptionID])

GO
ALTER TABLE [SalesLT].[ProductModelProductDescription] WITH CHECK ADD CONSTRAINT [FK_ProductModelProductDescription_ProductModel_ProductModelID]
   FOREIGN KEY([ProductModelID]) REFERENCES [SalesLT].[ProductModel] ([ProductModelID])

GO
