ALTER TABLE [Person].[Person] ADD CONSTRAINT [DF_Person_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Person].[Person] ADD CONSTRAINT [DF_Person_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Person].[Person] ADD CONSTRAINT [DF_Person_NameStyle] DEFAULT ((0)) FOR [NameStyle]
GO
ALTER TABLE [Person].[Person] ADD CONSTRAINT [DF_Person_EmailPromotion] DEFAULT ((0)) FOR [EmailPromotion]
GO
