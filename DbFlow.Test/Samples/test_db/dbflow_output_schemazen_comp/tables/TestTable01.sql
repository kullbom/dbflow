CREATE TABLE [dbo].[TestTable01] (
   [Id] [int] NOT NULL
      IDENTITY (1,1),
   [ColWithDefault] [int] NOT NULL,
   [ColWithDefault2] [int] NULL,
   [ColWithNamedDefault] [int] NOT NULL

   ,CONSTRAINT [PK__TestTabl__3214EC076F912F5F] PRIMARY KEY CLUSTERED ([Id])
)


GO
