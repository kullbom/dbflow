ALTER TABLE [SalesLT].[ProductModelProductDescription] WITH CHECK ADD CONSTRAINT [FK_ProductModelProductDescription_ProductModel_ProductModelID]
   FOREIGN KEY([ProductModelID]) REFERENCES [SalesLT].[ProductModel] ([ProductModelID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing ProductModel.ProductModelID.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'CONSTRAINT', [FK_ProductModelProductDescription_ProductModel_ProductModelID];
GO
ALTER TABLE [SalesLT].[ProductModelProductDescription] WITH CHECK ADD CONSTRAINT [FK_ProductModelProductDescription_ProductDescription_ProductDescriptionID]
   FOREIGN KEY([ProductDescriptionID]) REFERENCES [SalesLT].[ProductDescription] ([ProductDescriptionID])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing ProductDescription.ProductDescriptionID.', N'SCHEMA', [SalesLT], N'TABLE', [ProductModelProductDescription], N'CONSTRAINT', [FK_ProductModelProductDescription_ProductDescription_ProductDescriptionID];
GO
