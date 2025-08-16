ALTER TABLE [Production].[UnitMeasure] ADD CONSTRAINT [DF_UnitMeasure_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
