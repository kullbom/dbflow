ALTER TABLE [Production].[Document] WITH CHECK ADD CONSTRAINT [CK_Document_Status] CHECK (([Status]>=(1) AND [Status]<=(3)))
GO
