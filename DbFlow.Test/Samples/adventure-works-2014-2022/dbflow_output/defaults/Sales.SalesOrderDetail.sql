ALTER TABLE [Sales].[SalesOrderDetail] ADD CONSTRAINT [DF_SalesOrderDetail_UnitPriceDiscount] DEFAULT ((0.0)) FOR [UnitPriceDiscount]
GO
ALTER TABLE [Sales].[SalesOrderDetail] ADD CONSTRAINT [DF_SalesOrderDetail_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [Sales].[SalesOrderDetail] ADD CONSTRAINT [DF_SalesOrderDetail_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
