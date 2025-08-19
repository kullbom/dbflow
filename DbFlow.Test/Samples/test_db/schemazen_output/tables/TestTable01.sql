CREATE TABLE [dbo].[TestTable01] (
   [Id] [int] NOT NULL
      IDENTITY (1,1),
   [ColWithDefault] [int] NOT NULL,
   [ColWithDefault2] [int] NULL,
   [ColWithNamedDefault] [int] NOT NULL

   ,CONSTRAINT [PK__TestTabl__3214EC07B54A56E7] PRIMARY KEY CLUSTERED ([Id])
)


GO
