CREATE TABLE [Sales].[Store] (
   [BusinessEntityID] [int] NOT NULL,
   [Name] [nvarchar](50) NOT NULL,
   [SalesPersonID] [int] NULL,
   [Demographics] [xml] NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Store_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Store_rowguid] ON [Sales].[Store] ([rowguid])
CREATE NONCLUSTERED INDEX [IX_Store_SalesPersonID] ON [Sales].[Store] ([SalesPersonID])
CREATE XML INDEX [PXML_Store_Demographics] ON [Sales].[Store] ([Demographics])

GO
