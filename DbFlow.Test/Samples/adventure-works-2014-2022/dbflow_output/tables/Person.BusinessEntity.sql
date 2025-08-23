CREATE TABLE [Person].[BusinessEntity] (
   [BusinessEntityID] [INT] NOT NULL
      IDENTITY (1,1),
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_BusinessEntity_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_BusinessEntity_rowguid] ON [Person].[BusinessEntity] ([rowguid])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Person], N'TABLE', [BusinessEntity], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Person], N'TABLE', [BusinessEntity], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for all customers, vendors, and employees.', N'SCHEMA', [Person], N'TABLE', [BusinessEntity], N'COLUMN', [BusinessEntityID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Source of the ID that connects vendors, customers, and employees with address and contact information.', N'SCHEMA', [Person], N'TABLE', [BusinessEntity], NULL, NULL;
