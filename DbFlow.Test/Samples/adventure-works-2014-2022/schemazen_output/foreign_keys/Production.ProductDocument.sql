ALTER TABLE [Production].[ProductDocument] WITH CHECK ADD CONSTRAINT [FK_ProductDocument_Document_DocumentNode]
   FOREIGN KEY([DocumentNode]) REFERENCES [Production].[Document] ([DocumentNode])

GO
ALTER TABLE [Production].[ProductDocument] WITH CHECK ADD CONSTRAINT [FK_ProductDocument_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
