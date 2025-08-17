ALTER TABLE [Production].[ProductProductPhoto] WITH CHECK ADD CONSTRAINT [FK_ProductProductPhoto_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
ALTER TABLE [Production].[ProductProductPhoto] WITH CHECK ADD CONSTRAINT [FK_ProductProductPhoto_ProductPhoto_ProductPhotoID]
   FOREIGN KEY([ProductPhotoID]) REFERENCES [Production].[ProductPhoto] ([ProductPhotoID])

GO
