ALTER TABLE [Purchasing].[ShipMethod] WITH CHECK ADD CONSTRAINT [CK_ShipMethod_ShipBase] CHECK ([ShipBase]>(0.00))
GO
ALTER TABLE [Purchasing].[ShipMethod] WITH CHECK ADD CONSTRAINT [CK_ShipMethod_ShipRate] CHECK ([ShipRate]>(0.00))
GO
