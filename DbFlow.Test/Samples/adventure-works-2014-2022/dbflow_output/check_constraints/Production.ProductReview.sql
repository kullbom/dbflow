ALTER TABLE [Production].[ProductReview] WITH CHECK ADD CONSTRAINT [CK_ProductReview_Rating] CHECK (([Rating]>=(1) AND [Rating]<=(5)))
GO
