CREATE TABLE [Production].[ScrapReason] (
   [ScrapReasonID] [SMALLINT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ScrapReason_ScrapReasonID] PRIMARY KEY CLUSTERED ([ScrapReasonID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ScrapReason_Name] ON [Production].[ScrapReason] ([Name])

GO
