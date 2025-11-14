ALTER TABLE [Purchasing].[ShipMethod] ADD CONSTRAINT [DF_ShipMethod_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Purchasing].[ShipMethod] ADD CONSTRAINT [DF_ShipMethod_ShipBase] DEFAULT ((0.00)) FOR [ShipBase]
GO
ALTER TABLE [Purchasing].[ShipMethod] ADD CONSTRAINT [DF_ShipMethod_ShipRate] DEFAULT ((0.00)) FOR [ShipRate]
GO
ALTER TABLE [Purchasing].[ShipMethod] ADD CONSTRAINT [DF_ShipMethod_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
