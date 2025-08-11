ALTER TABLE [Production].[TransactionHistory] WITH CHECK ADD CONSTRAINT [FK_TransactionHistory_Product_ProductID]
   FOREIGN KEY([ProductID]) REFERENCES [Production].[Product] ([ProductID])

GO
