CREATE TABLE [dbo].[TestTable01] (
   [Id] [INT] NOT NULL
      IDENTITY (1,1),
   [ColWithDefault] [INT] NOT NULL,
   [ColWithDefault2] [INT] NULL,
   [ColWithNamedDefault] [INT] NOT NULL

   ,PRIMARY KEY CLUSTERED ([Id])
)


GO
