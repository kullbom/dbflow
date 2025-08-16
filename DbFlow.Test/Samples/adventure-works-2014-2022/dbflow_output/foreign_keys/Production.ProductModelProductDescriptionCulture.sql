ALTER TABLE [Production].[ProductModelProductDescriptionCulture] WITH CHECK ADD CONSTRAINT [FK_ProductModelProductDescriptionCulture_ProductModel_ProductModelID]
   FOREIGN KEY([ProductModelID]) REFERENCES [Production].[ProductModel] ([ProductModelID])

GO
ALTER TABLE [Production].[ProductModelProductDescriptionCulture] WITH CHECK ADD CONSTRAINT [FK_ProductModelProductDescriptionCulture_Culture_CultureID]
   FOREIGN KEY([CultureID]) REFERENCES [Production].[Culture] ([CultureID])

GO
ALTER TABLE [Production].[ProductModelProductDescriptionCulture] WITH CHECK ADD CONSTRAINT [FK_ProductModelProductDescriptionCulture_ProductDescription_ProductDescriptionID]
   FOREIGN KEY([ProductDescriptionID]) REFERENCES [Production].[ProductDescription] ([ProductDescriptionID])

GO
