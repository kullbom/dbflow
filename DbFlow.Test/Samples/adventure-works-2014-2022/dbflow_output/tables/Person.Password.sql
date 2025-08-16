CREATE TABLE [Person].[Password] (
   [BusinessEntityID] [INT] NOT NULL,
   [PasswordHash] [VARCHAR](128) NOT NULL,
   [PasswordSalt] [VARCHAR](10) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Password_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)


GO
