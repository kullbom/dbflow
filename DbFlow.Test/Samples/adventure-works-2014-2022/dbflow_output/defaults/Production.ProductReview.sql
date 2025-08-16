ALTER TABLE [Production].[ProductReview] ADD CONSTRAINT [DF_ProductReview_ReviewDate] DEFAULT (getdate()) FOR [ReviewDate]
GO
ALTER TABLE [Production].[ProductReview] ADD CONSTRAINT [DF_ProductReview_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
