CREATE TABLE [Sales].[Store] (
   [BusinessEntityID] [INT] NOT NULL,
   [Name] [NAME] NOT NULL,
   [SalesPersonID] [INT] NULL,
   [Demographics] [XML] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Store_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Store_rowguid] ON [Sales].[Store] ([rowguid])
CREATE NONCLUSTERED INDEX [IX_Store_SalesPersonID] ON [Sales].[Store] ([SalesPersonID])
CREATE PRIMARY XML INDEX [PXML_Store_Demographics] ON [Sales].[Store] ([Demographics])

GO
