ALTER TABLE [dbo].[TestTable01] WITH CHECK ADD CONSTRAINT [CK_TestTable01_PreventBad] CHECK ([ColWithDefault2]<>'Foobar')
GO
