CREATE TABLE [Person].[Password] (
   [BusinessEntityID] [int] NOT NULL,
   [PasswordHash] [varchar](128) NOT NULL,
   [PasswordSalt] [varchar](10) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Password_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)


GO
