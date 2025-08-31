CREATE TABLE [SalesLT].[CustomerAddress] (
   [CustomerID] [INT] NOT NULL,
   [AddressID] [INT] NOT NULL,
   [AddressType] [NAME] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_CustomerAddress_CustomerID_AddressID] PRIMARY KEY CLUSTERED ([CustomerID], [AddressID])
   ,CONSTRAINT [AK_CustomerAddress_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'The kind of Address. One of: Archive, Billing, Home, Main Office, Primary, Shipping', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'COLUMN', [AddressType];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to Address.AddressID.', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'COLUMN', [AddressID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key. Foreign key to Customer.CustomerID.', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'COLUMN', [CustomerID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Cross-reference table mapping customers to their address(es).', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], NULL, NULL;

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key (clustered) constraint', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'INDEX', [PK_CustomerAddress_CustomerID_AddressID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unique nonclustered constraint. Used to support replication samples.', N'SCHEMA', [SalesLT], N'TABLE', [CustomerAddress], N'INDEX', [AK_CustomerAddress_rowguid];
