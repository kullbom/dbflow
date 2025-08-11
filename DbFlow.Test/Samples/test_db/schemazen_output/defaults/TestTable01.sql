ALTER TABLE [dbo].[TestTable01] ADD  DEFAULT ((1)) FOR [ColWithDefault]
GO
ALTER TABLE [dbo].[TestTable01] ADD  DEFAULT (NULL) FOR [ColWithDefault2]
GO
ALTER TABLE [dbo].[TestTable01] ADD CONSTRAINT [DF_TestTable01_ColWithNamedDefault] DEFAULT ((-1)) FOR [ColWithNamedDefault]
GO
