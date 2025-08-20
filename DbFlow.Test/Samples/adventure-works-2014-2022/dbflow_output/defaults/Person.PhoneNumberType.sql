ALTER TABLE [Person].[PhoneNumberType] ADD CONSTRAINT [DF_PhoneNumberType_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate]
GO
