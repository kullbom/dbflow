ALTER TABLE [Purchasing].[PurchaseOrderHeader] WITH CHECK ADD CONSTRAINT [FK_PurchaseOrderHeader_Employee_EmployeeID]
   FOREIGN KEY([EmployeeID]) REFERENCES [HumanResources].[Employee] ([BusinessEntityID])

GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] WITH CHECK ADD CONSTRAINT [FK_PurchaseOrderHeader_ShipMethod_ShipMethodID]
   FOREIGN KEY([ShipMethodID]) REFERENCES [Purchasing].[ShipMethod] ([ShipMethodID])

GO
ALTER TABLE [Purchasing].[PurchaseOrderHeader] WITH CHECK ADD CONSTRAINT [FK_PurchaseOrderHeader_Vendor_VendorID]
   FOREIGN KEY([VendorID]) REFERENCES [Purchasing].[Vendor] ([BusinessEntityID])

GO
