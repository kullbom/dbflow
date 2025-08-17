CREATE TABLE [Sales].[SalesPersonQuotaHistory] (
   [BusinessEntityID] [int] NOT NULL,
   [QuotaDate] [datetime] NOT NULL,
   [SalesQuota] [money] NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesPersonQuotaHistory_BusinessEntityID_QuotaDate] PRIMARY KEY CLUSTERED ([BusinessEntityID], [QuotaDate])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesPersonQuotaHistory_rowguid] ON [Sales].[SalesPersonQuotaHistory] ([rowguid])

GO
