CREATE TABLE [Person].[PhoneNumberType] (
   [PhoneNumberTypeID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_PhoneNumberType_PhoneNumberTypeID] PRIMARY KEY CLUSTERED ([PhoneNumberTypeID])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Type of phone number of a person.', N'SCHEMA', [Person], N'TABLE', [PhoneNumberType];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for telephone number type records.', N'SCHEMA', [Person], N'TABLE', [PhoneNumberType], N'COLUMN', [PhoneNumberTypeID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Name of the telephone number type', N'SCHEMA', [Person], N'TABLE', [PhoneNumberType], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Person], N'TABLE', [PhoneNumberType], N'COLUMN', [ModifiedDate];
