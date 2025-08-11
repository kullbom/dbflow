CREATE TABLE [Production].[Illustration] (
   [IllustrationID] [INT] NOT NULL
      IDENTITY (1,1),
   [Diagram] [XML] NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_Illustration_IllustrationID] PRIMARY KEY CLUSTERED ([IllustrationID])
)


GO
