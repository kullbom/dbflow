CREATE TABLE [dbo].[TestTable02] (
   [RefId] [INT] NOT NULL,
   [Id] [INT] NOT NULL
      IDENTITY (1,1),
   [FirstName] [VARCHAR](100) MASKED WITH ( FUNCTION = 'partial(1, "xxxxx", 1)' ) NULL

   ,PRIMARY KEY CLUSTERED ([RefId])
   ,UNIQUE NONCLUSTERED ([Id])
)

CREATE NONCLUSTERED INDEX [IX_TestTable02_RefId] ON [dbo].[TestTable02] ([RefId]) INCLUDE ([FirstName])
CREATE NONCLUSTERED INDEX [IX_TestTable02_FirstName_56] ON [dbo].[TestTable02] ([FirstName]) WHERE ([FirstName]='56')

GO
