ALTER TABLE [Person].[StateProvince] ADD CONSTRAINT [DF_StateProvince_IsOnlyStateProvinceFlag] DEFAULT ((1)) FOR [IsOnlyStateProvinceFlag]
GO
ALTER TABLE [Person].[StateProvince] ADD CONSTRAINT [DF_StateProvince_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [Person].[StateProvince] ADD CONSTRAINT [DF_StateProvince_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
