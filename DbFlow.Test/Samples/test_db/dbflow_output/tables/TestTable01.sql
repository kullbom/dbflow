CREATE TABLE [dbo].[TestTable01] (
   [Id] [INT] NOT NULL
      IDENTITY (1,1),
   [ColWithDefault] [INT] NOT NULL
       DEFAULT ((1)),
   [ColWithDefault2] [INT] NULL
       DEFAULT (NULL),
   [ColWithNamedDefault] [INT] NOT NULL
       DEFAULT ((-1))

   ,PRIMARY KEY CLUSTERED ([Id])
)


GO
