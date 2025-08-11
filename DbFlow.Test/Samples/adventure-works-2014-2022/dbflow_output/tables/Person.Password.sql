CREATE TABLE [Person].[Password] (
   [BusinessEntityID] [INT] NOT NULL,
   [PasswordHash] [VARCHAR](128) NOT NULL,
   [PasswordSalt] [VARCHAR](10) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_Password_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)


GO
