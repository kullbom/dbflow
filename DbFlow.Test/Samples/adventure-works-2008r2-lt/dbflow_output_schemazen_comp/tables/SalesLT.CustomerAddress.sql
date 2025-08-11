CREATE TABLE [SalesLT].[CustomerAddress] (
   [CustomerID] [int] NOT NULL,
   [AddressID] [int] NOT NULL,
   [AddressType] [nvarchar](50) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_CustomerAddress_CustomerID_AddressID] PRIMARY KEY CLUSTERED ([CustomerID], [AddressID])
   ,CONSTRAINT [AK_CustomerAddress_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO
