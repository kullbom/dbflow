ALTER TABLE [Production].[Location] WITH CHECK ADD CONSTRAINT [CK_Location_Availability] CHECK  ([Availability]>=(0.00))
GO
ALTER TABLE [Production].[Location] WITH CHECK ADD CONSTRAINT [CK_Location_CostRate] CHECK  ([CostRate]>=(0.00))
GO
