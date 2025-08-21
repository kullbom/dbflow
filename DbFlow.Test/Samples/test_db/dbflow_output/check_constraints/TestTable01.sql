ALTER TABLE [dbo].[TestTable01] WITH NOCHECK ADD CONSTRAINT [CK_TestTable01_PreventBad] CHECK ([ColWithDefault2]<>'Foobar')
GO
