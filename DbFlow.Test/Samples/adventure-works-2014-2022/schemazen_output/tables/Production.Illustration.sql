CREATE TABLE [Production].[Illustration] (
   [IllustrationID] [int] NOT NULL
      IDENTITY (1,1),
   [Diagram] [xml] NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Illustration_IllustrationID] PRIMARY KEY CLUSTERED ([IllustrationID])
)


GO
