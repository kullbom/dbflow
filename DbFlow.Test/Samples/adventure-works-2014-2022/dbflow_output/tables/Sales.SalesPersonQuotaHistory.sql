CREATE TABLE [Sales].[SalesPersonQuotaHistory] (
   [BusinessEntityID] [INT] NOT NULL,
   [QuotaDate] [DATETIME] NOT NULL,
   [SalesQuota] [MONEY] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SalesPersonQuotaHistory_BusinessEntityID_QuotaDate] PRIMARY KEY CLUSTERED ([BusinessEntityID], [QuotaDate])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesPersonQuotaHistory_rowguid] ON [Sales].[SalesPersonQuotaHistory] ([rowguid])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Sales], N'TABLE', [SalesPersonQuotaHistory], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Sales], N'TABLE', [SalesPersonQuotaHistory], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales quota amount.', N'SCHEMA', [Sales], N'TABLE', [SalesPersonQuotaHistory], N'COLUMN', [SalesQuota];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales quota date.', N'SCHEMA', [Sales], N'TABLE', [SalesPersonQuotaHistory], N'COLUMN', [QuotaDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales person identification number. Foreign key to SalesPerson.BusinessEntityID.', N'SCHEMA', [Sales], N'TABLE', [SalesPersonQuotaHistory], N'COLUMN', [BusinessEntityID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Sales performance tracking.', N'SCHEMA', [Sales], N'TABLE', [SalesPersonQuotaHistory], NULL, NULL;
