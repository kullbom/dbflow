ALTER TABLE [Production].[ProductModelIllustration] WITH CHECK ADD CONSTRAINT [FK_ProductModelIllustration_Illustration_IllustrationID]
   FOREIGN KEY([IllustrationID]) REFERENCES [Production].[Illustration] ([IllustrationID])

GO
ALTER TABLE [Production].[ProductModelIllustration] WITH CHECK ADD CONSTRAINT [FK_ProductModelIllustration_ProductModel_ProductModelID]
   FOREIGN KEY([ProductModelID]) REFERENCES [Production].[ProductModel] ([ProductModelID])

GO
