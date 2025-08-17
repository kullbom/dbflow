CREATE TABLE [Person].[EmailAddress] (
   [BusinessEntityID] [int] NOT NULL,
   [EmailAddressID] [int] NOT NULL
      IDENTITY (1,1),
   [EmailAddress] [nvarchar](50) NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_EmailAddress_BusinessEntityID_EmailAddressID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [EmailAddressID])
)

CREATE NONCLUSTERED INDEX [IX_EmailAddress_EmailAddress] ON [Person].[EmailAddress] ([EmailAddress])

GO
