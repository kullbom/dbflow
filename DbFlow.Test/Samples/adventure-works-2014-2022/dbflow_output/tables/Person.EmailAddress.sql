CREATE TABLE [Person].[EmailAddress] (
   [BusinessEntityID] [INT] NOT NULL,
   [EmailAddressID] [INT] NOT NULL
      IDENTITY (1,1),
   [EmailAddress] [NVARCHAR](50) NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_EmailAddress_BusinessEntityID_EmailAddressID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [EmailAddressID])
)

CREATE NONCLUSTERED INDEX [IX_EmailAddress_EmailAddress] ON [Person].[EmailAddress] ([EmailAddress])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Where to send a person email.', N'SCHEMA', [Person], N'TABLE', [EmailAddress];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Person associated with this email address.  Foreign key to Person.BusinessEntityID', N'SCHEMA', [Person], N'TABLE', [EmailAddress], N'COLUMN', [BusinessEntityID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. ID of this email address.', N'SCHEMA', [Person], N'TABLE', [EmailAddress], N'COLUMN', [EmailAddressID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'E-mail address for the person.', N'SCHEMA', [Person], N'TABLE', [EmailAddress], N'COLUMN', [EmailAddress];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Person], N'TABLE', [EmailAddress], N'COLUMN', [rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Person], N'TABLE', [EmailAddress], N'COLUMN', [ModifiedDate];
