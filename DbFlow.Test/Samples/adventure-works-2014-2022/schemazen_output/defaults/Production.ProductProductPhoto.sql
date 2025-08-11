ALTER TABLE [Production].[ProductProductPhoto] ADD CONSTRAINT [DF_ProductProductPhoto_Primary] DEFAULT ((0)) FOR [Primary]
GO
ALTER TABLE [Production].[ProductProductPhoto] ADD CONSTRAINT [DF_ProductProductPhoto_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
