ALTER TABLE [Sales].[SpecialOfferProduct] ADD CONSTRAINT [DF_SpecialOfferProduct_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Sales].[SpecialOfferProduct] ADD CONSTRAINT [DF_SpecialOfferProduct_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
