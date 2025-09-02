CREATE TABLE [Person].[AddressType] (
   [AddressTypeID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_AddressType_AddressTypeID] PRIMARY KEY CLUSTERED ([AddressTypeID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_AddressType_rowguid] ON [Person].[AddressType] ([rowguid])
CREATE UNIQUE NONCLUSTERED INDEX [AK_AddressType_Name] ON [Person].[AddressType] ([Name])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Types of addresses stored in the Address table. ', N'SCHEMA', [Person], N'TABLE', [AddressType];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for AddressType records.', N'SCHEMA', [Person], N'TABLE', [AddressType], N'COLUMN', [AddressTypeID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Address type description. For example, Billing, Home, or Shipping.', N'SCHEMA', [Person], N'TABLE', [AddressType], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Person], N'TABLE', [AddressType], N'COLUMN', [rowguid];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Person], N'TABLE', [AddressType], N'COLUMN', [ModifiedDate];
