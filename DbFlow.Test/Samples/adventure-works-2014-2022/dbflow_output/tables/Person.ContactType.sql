CREATE TABLE [Person].[ContactType] (
   [ContactTypeID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ContactType_ContactTypeID] PRIMARY KEY CLUSTERED ([ContactTypeID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ContactType_Name] ON [Person].[ContactType] ([Name])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Person], N'TABLE', [ContactType], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Contact type description.', N'SCHEMA', [Person], N'TABLE', [ContactType], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ContactType records.', N'SCHEMA', [Person], N'TABLE', [ContactType], N'COLUMN', [ContactTypeID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Lookup table containing the types of business entity contacts.', N'SCHEMA', [Person], N'TABLE', [ContactType];
