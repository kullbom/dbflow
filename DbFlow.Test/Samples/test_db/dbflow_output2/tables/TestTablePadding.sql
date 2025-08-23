CREATE TABLE [dbo].[TestTablePadding] (
   [Id] [CHAR](7) NOT NULL,
   [Content] [NVARCHAR](16) NULL

   ,UNIQUE NONCLUSTERED ([Id])
)

CREATE NONCLUSTERED INDEX [IX_TestTablePadding_Content1] ON [dbo].[TestTablePadding] ([Content])
CREATE NONCLUSTERED INDEX [IX_TestTablePadding_Content2] ON [dbo].[TestTablePadding] ([Content])
ALTER INDEX IX_TestTablePadding_Content2 ON [dbo].[TestTablePadding] DISABLE

GO
