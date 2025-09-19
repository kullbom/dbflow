ALTER TABLE [SalesLT].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [SalesLT].[Product] ([ProductID])

GO
ALTER TABLE [SalesLT].[SalesOrderDetail] WITH CHECK ADD CONSTRAINT [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID]
   FOREIGN KEY([SalesOrderID]) REFERENCES [SalesLT].[SalesOrderHeader] ([SalesOrderID])
   ON DELETE CASCADE

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Foreign key constraint referencing SalesOrderHeader.SalesOrderID.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderDetail], N'CONSTRAINT', [FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID];
GO
