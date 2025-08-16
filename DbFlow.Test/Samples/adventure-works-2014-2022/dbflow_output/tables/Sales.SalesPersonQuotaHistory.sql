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
