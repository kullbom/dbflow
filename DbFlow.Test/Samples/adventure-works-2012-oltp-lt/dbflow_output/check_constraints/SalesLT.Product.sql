ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_SellEndDate] CHECK ([SellEndDate]>=[SellStartDate] OR [SellEndDate] IS NULL)
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Check constraint [SellEndDate] >= [SellStartDate] OR [SellEndDate] IS NULL', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'CONSTRAINT', [CK_Product_SellEndDate];

GO
ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_Weight] CHECK ([Weight]>(0.00))
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Check constraint [Weight] > (0.00)', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'CONSTRAINT', [CK_Product_Weight];

GO
ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_ListPrice] CHECK ([ListPrice]>=(0.00))
GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Check constraint [ListPrice] >= (0.00)', N'SCHEMA', [SalesLT], N'TABLE', [Product], N'CONSTRAINT', [CK_Product_ListPrice];

GO
ALTER TABLE [SalesLT].[Product] WITH CHECK ADD CONSTRAINT [CK_Product_StandardCost] CHECK ([StandardCost]>=(0.00))
GO
