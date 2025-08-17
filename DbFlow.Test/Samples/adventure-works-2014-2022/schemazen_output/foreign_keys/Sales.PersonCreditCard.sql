ALTER TABLE [Sales].[PersonCreditCard] WITH CHECK ADD CONSTRAINT [FK_PersonCreditCard_Person_BusinessEntityID]
   FOREIGN KEY([BusinessEntityID]) REFERENCES [Person].[Person] ([BusinessEntityID])

GO
ALTER TABLE [Sales].[PersonCreditCard] WITH CHECK ADD CONSTRAINT [FK_PersonCreditCard_CreditCard_CreditCardID]
   FOREIGN KEY([CreditCardID]) REFERENCES [Sales].[CreditCard] ([CreditCardID])

GO
