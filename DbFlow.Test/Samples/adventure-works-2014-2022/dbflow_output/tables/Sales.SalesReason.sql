CREATE TABLE [Sales].[SalesReason] (
   [SalesReasonID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ReasonType] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SalesReason_SalesReasonID] PRIMARY KEY CLUSTERED ([SalesReasonID])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Sales], N'TABLE', [SalesReason], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Category the sales reason belongs to.', N'SCHEMA', [Sales], N'TABLE', [SalesReason], N'COLUMN', [ReasonType];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales reason description.', N'SCHEMA', [Sales], N'TABLE', [SalesReason], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for SalesReason records.', N'SCHEMA', [Sales], N'TABLE', [SalesReason], N'COLUMN', [SalesReasonID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Lookup table of customer purchase reasons.', N'SCHEMA', [Sales], N'TABLE', [SalesReason], NULL, NULL;
