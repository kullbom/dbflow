ALTER TABLE [Person].[CountryRegion] ADD CONSTRAINT [DF_CountryRegion_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
