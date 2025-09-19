CREATE TABLE [Production].[Culture] (
   [CultureID] [NCHAR](6) NOT NULL,
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Culture_CultureID] PRIMARY KEY CLUSTERED ([CultureID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Culture_Name] ON [Production].[Culture] ([Name])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Lookup table containing the languages in which some AdventureWorks data is stored.', N'SCHEMA', [Production], N'TABLE', [Culture];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Culture records.', N'SCHEMA', [Production], N'TABLE', [Culture], N'COLUMN', [CultureID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Culture description.', N'SCHEMA', [Production], N'TABLE', [Culture], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [Culture], N'COLUMN', [ModifiedDate];
