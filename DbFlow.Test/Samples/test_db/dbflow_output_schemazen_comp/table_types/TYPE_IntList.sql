CREATE TYPE [dbo].[IntList] AS TABLE (
   [Id] [int] NOT NULL,
   [Value] [int] NOT NULL
       DEFAULT ((1)),
   [CplxComputed] AS ([Value]*(5)),
   [ColWithNamedDefault] [int] NOT NULL
       DEFAULT ((-1))

   ,CHECK  ([Value]>(0))
   ,PRIMARY KEY CLUSTERED ([Id])
)


GO
