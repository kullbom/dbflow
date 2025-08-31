CREATE TABLE [SalesLT].[Customer] (
   [CustomerID] [INT] NOT NULL
      IDENTITY (1,1),
   [NameStyle] [NAMESTYLE] NOT NULL,
   [Title] [NVARCHAR](8) NULL,
   [FirstName] [NAME] NOT NULL,
   [MiddleName] [NAME] NULL,
   [LastName] [NAME] NOT NULL,
   [Suffix] [NVARCHAR](10) NULL,
   [CompanyName] [NVARCHAR](128) NULL,
   [SalesPerson] [NVARCHAR](256) NULL,
   [EmailAddress] [NVARCHAR](50) NULL,
   [Phone] [PHONE] NULL,
   [PasswordHash] [VARCHAR](128) NOT NULL,
   [PasswordSalt] [VARCHAR](10) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Customer_CustomerID] PRIMARY KEY CLUSTERED ([CustomerID])
   ,CONSTRAINT [AK_Customer_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_Customer_EmailAddress] ON [SalesLT].[Customer] ([EmailAddress])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Random value concatenated with the password string before the password is hashed.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [PasswordSalt];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Password for the e-mail account.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [PasswordHash];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Phone number associated with the person.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [Phone];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'E-mail address for the person.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [EmailAddress];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The customer''s sales person, an employee of AdventureWorks Cycles.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [SalesPerson];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The customer''s organization.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [CompanyName];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Surname suffix. For example, Sr. or Jr.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [Suffix];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Last name of the person.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [LastName];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Middle name or middle initial of the person.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [MiddleName];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'First name of the person.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [FirstName];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'A courtesy title. For example, Mr. or Ms.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [Title];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = The data in FirstName and LastName are stored in western style (first name, last name) order.  1 = Eastern style (last name, first name) order.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [NameStyle];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Customer records.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'COLUMN', [CustomerID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Customer information.', N'SCHEMA', [SalesLT], N'TABLE', [Customer];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key (clustered) constraint', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'INDEX', [PK_Customer_CustomerID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'INDEX', [AK_Customer_rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Nonclustered index.', N'SCHEMA', [SalesLT], N'TABLE', [Customer], N'INDEX', [IX_Customer_EmailAddress];
