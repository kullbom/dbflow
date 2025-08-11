CREATE TABLE [SalesLT].[Address] (
   [AddressID] [int] NOT NULL
      IDENTITY (1,1),
   [AddressLine1] [nvarchar](60) NOT NULL,
   [AddressLine2] [nvarchar](60) NULL,
   [City] [nvarchar](30) NOT NULL,
   [StateProvince] [nvarchar](50) NOT NULL,
   [CountryRegion] [nvarchar](50) NOT NULL,
   [PostalCode] [nvarchar](15) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Address_AddressID] PRIMARY KEY CLUSTERED ([AddressID])
   ,CONSTRAINT [AK_Address_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_Address_AddressLine1_AddressLine2_City_StateProvince_PostalCode_CountryRegion] ON [SalesLT].[Address] ([AddressLine1], [AddressLine2], [City], [StateProvince], [PostalCode], [CountryRegion])
CREATE NONCLUSTERED INDEX [IX_Address_StateProvince] ON [SalesLT].[Address] ([StateProvince])

GO
