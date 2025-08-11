ALTER TABLE [Production].[ProductModelIllustration] ADD CONSTRAINT [DF_ProductModelIllustration_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
