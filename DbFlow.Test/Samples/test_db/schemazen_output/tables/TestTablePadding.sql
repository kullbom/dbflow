CREATE TABLE [dbo].[TestTablePadding] (
   [Id] [char](7) NOT NULL,
   [Content] [nvarchar](16) NULL

   ,CONSTRAINT [UQ__TestTabl__3214EC062A817975] UNIQUE NONCLUSTERED ([Id])
)

CREATE NONCLUSTERED INDEX [IX_TestTablePadding_Content1] ON [dbo].[TestTablePadding] ([Content])
CREATE NONCLUSTERED INDEX [IX_TestTablePadding_Content2] ON [dbo].[TestTablePadding] ([Content])

GO
