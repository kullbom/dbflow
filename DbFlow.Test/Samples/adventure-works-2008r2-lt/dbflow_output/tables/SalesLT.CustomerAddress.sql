CREATE TABLE [SalesLT].[CustomerAddress] (
   [CustomerID] [INT] NOT NULL,
   [AddressID] [INT] NOT NULL,
   [AddressType] [NAME] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_CustomerAddress_CustomerID_AddressID] PRIMARY KEY CLUSTERED ([CustomerID], [AddressID])
   ,CONSTRAINT [AK_CustomerAddress_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO
