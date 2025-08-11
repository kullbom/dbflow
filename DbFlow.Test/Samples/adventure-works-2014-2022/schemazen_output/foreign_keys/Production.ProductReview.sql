ALTER TABLE [Production].[ProductReview] WITH CHECK ADD CONSTRAINT [FK_ProductReview_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
