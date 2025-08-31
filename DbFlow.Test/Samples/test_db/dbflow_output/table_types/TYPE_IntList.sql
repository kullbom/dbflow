CREATE TYPE [dbo].[IntList] AS TABLE (
   [Id] [INT] NOT NULL
      IDENTITY (1,1),
   [Value] [INT] NOT NULL
       DEFAULT ((1)),
   [CplxComputed] AS ([Value]*(5)),
   [ColWithNamedDefault] [INT] NOT NULL
       DEFAULT ((-1))

   ,CHECK ([Value]>(0))
   ,PRIMARY KEY CLUSTERED ([Id])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'Foobar2', N'The value 456', N'SCHEMA', [dbo], N'TYPE', [IntList];
