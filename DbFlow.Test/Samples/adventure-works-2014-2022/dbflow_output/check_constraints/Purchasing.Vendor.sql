ALTER TABLE [Purchasing].[Vendor] WITH CHECK ADD CONSTRAINT [CK_Vendor_CreditRating] CHECK (([CreditRating]>=(1) AND [CreditRating]<=(5)))
GO
