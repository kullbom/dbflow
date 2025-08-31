CREATE TABLE [dbo].[TestTable02] (
   [RefId] [int] NOT NULL,
   [Id] [int] NOT NULL
      IDENTITY (1,1),
   [FirstName] [varchar](100) NULL

   ,CONSTRAINT [PK__TestTabl__2D2A2CF11A4ABC86] PRIMARY KEY CLUSTERED ([RefId])
   ,CONSTRAINT [UQ__TestTabl__3214EC067BF14958] UNIQUE NONCLUSTERED ([Id])
)

CREATE NONCLUSTERED INDEX [IX_TestTable02_FirstName_56] ON [dbo].[TestTable02] ([FirstName]) WHERE ([FirstName]='56')
CREATE NONCLUSTERED INDEX [IX_TestTable02_RefId] ON [dbo].[TestTable02] ([RefId]) INCLUDE ([FirstName])

GO
