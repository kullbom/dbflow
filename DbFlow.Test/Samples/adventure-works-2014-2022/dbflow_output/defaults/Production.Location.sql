ALTER TABLE [Production].[Location] ADD CONSTRAINT [DF_Location_CostRate] DEFAULT ((0.00)) FOR [CostRate]
GO
ALTER TABLE [Production].[Location] ADD CONSTRAINT [DF_Location_Availability] DEFAULT ((0.00)) FOR [Availability]
GO
ALTER TABLE [Production].[Location] ADD CONSTRAINT [DF_Location_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
