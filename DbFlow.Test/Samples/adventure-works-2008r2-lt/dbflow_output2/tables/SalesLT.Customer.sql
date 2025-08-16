CREATE TABLE [SalesLT].[Customer] (
   [CustomerID] [INT] NOT NULL
      IDENTITY (1,1),
   [NameStyle] [NAMESTYLE] NOT NULL,
   [Title] [NVARCHAR](8) NULL,
   [FirstName] [NAME] NOT NULL,
   [MiddleName] [NAME] NULL,
   [LastName] [NAME] NOT NULL,
   [Suffix] [NVARCHAR](10) NULL,
   [CompanyName] [NVARCHAR](128) NULL,
   [SalesPerson] [NVARCHAR](256) NULL,
   [EmailAddress] [NVARCHAR](50) NULL,
   [Phone] [PHONE] NULL,
   [PasswordHash] [VARCHAR](128) NOT NULL,
   [PasswordSalt] [VARCHAR](10) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Customer_CustomerID] PRIMARY KEY CLUSTERED ([CustomerID])
   ,CONSTRAINT [AK_Customer_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_Customer_EmailAddress] ON [SalesLT].[Customer] ([EmailAddress])

GO
