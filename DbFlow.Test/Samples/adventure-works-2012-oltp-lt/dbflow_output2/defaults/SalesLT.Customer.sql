ALTER TABLE [SalesLT].[Customer] ADD CONSTRAINT [DF_Customer_NameStyle] DEFAULT ((0)) FOR [NameStyle]
GO
ALTER TABLE [SalesLT].[Customer] ADD CONSTRAINT [DF_Customer_rowguid] DEFAULT (newid()) FOR [rowguid]
GO
ALTER TABLE [SalesLT].[Customer] ADD CONSTRAINT [DF_Customer_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
