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
