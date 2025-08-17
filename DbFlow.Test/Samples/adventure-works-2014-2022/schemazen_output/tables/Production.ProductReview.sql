CREATE TABLE [Production].[ProductReview] (
   [ProductReviewID] [int] NOT NULL
      IDENTITY (1,1),
   [ProductID] [int] NOT NULL,
   [ReviewerName] [nvarchar](50) NOT NULL,
   [ReviewDate] [datetime] NOT NULL,
   [EmailAddress] [nvarchar](50) NOT NULL,
   [Rating] [int] NOT NULL,
   [Comments] [nvarchar](3850) NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductReview_ProductReviewID] PRIMARY KEY CLUSTERED ([ProductReviewID])
)

CREATE NONCLUSTERED INDEX [IX_ProductReview_ProductID_Name] ON [Production].[ProductReview] ([ProductID], [ReviewerName]) INCLUDE ([Comments])

GO
