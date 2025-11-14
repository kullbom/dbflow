ALTER TABLE [Purchasing].[Vendor] ADD CONSTRAINT [DF_Vendor_ActiveFlag] DEFAULT ((1)) FOR [ActiveFlag]
GO
ALTER TABLE [Purchasing].[Vendor] ADD CONSTRAINT [DF_Vendor_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Purchasing].[Vendor] ADD CONSTRAINT [DF_Vendor_PreferredVendorStatus] DEFAULT ((1)) FOR [PreferredVendorStatus]
GO
