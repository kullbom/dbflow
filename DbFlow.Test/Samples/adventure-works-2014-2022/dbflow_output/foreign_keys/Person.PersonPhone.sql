ALTER TABLE [Person].[PersonPhone] WITH CHECK ADD CONSTRAINT [FK_PersonPhone_Person_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Person].[Person] ([BusinessEntityID])

GO
ALTER TABLE [Person].[PersonPhone] WITH CHECK ADD CONSTRAINT [FK_PersonPhone_PhoneNumberType_PhoneNumberTypeID]
   FOREIGN KEY([PhoneNumberTypeID]) REFERENCES [Person].[PhoneNumberType] ([PhoneNumberTypeID])

GO
