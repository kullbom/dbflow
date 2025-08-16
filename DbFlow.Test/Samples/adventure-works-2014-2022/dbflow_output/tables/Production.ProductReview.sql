CREATE TABLE [Production].[ProductReview] (
   [ProductReviewID] [INT] NOT NULL
      IDENTITY (1,1),
   [ProductID] [INT] NOT NULL,
   [ReviewerName] [NAME] NOT NULL,
   [ReviewDate] [DATETIME] NOT NULL,
   [EmailAddress] [NVARCHAR](50) NOT NULL,
   [Rating] [INT] NOT NULL,
   [Comments] [NVARCHAR](3850) NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductReview_ProductReviewID] PRIMARY KEY CLUSTERED ([ProductReviewID])
)

CREATE NONCLUSTERED INDEX [IX_ProductReview_ProductID_Name] ON [Production].[ProductReview] ([ProductID], [ReviewerName]) INCLUDE ([Comments])

GO
