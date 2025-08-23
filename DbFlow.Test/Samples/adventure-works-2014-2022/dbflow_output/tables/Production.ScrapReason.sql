CREATE TABLE [Production].[ScrapReason] (
   [ScrapReasonID] [SMALLINT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ScrapReason_ScrapReasonID] PRIMARY KEY CLUSTERED ([ScrapReasonID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ScrapReason_Name] ON [Production].[ScrapReason] ([Name])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [ScrapReason], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Failure description.', N'SCHEMA', [Production], N'TABLE', [ScrapReason], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ScrapReason records.', N'SCHEMA', [Production], N'TABLE', [ScrapReason], N'COLUMN', [ScrapReasonID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Manufacturing failure reasons lookup table.', N'SCHEMA', [Production], N'TABLE', [ScrapReason], NULL, NULL;
