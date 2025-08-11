CREATE TABLE [SalesLT].[Address] (
   [AddressID] [INT] NOT NULL
      IDENTITY (1,1),
   [AddressLine1] [NVARCHAR](60) NOT NULL,
   [AddressLine2] [NVARCHAR](60) NULL,
   [City] [NVARCHAR](30) NOT NULL,
   [StateProvince] [NAME] NOT NULL,
   [CountryRegion] [NAME] NOT NULL,
   [PostalCode] [NVARCHAR](15) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_Address_AddressID] PRIMARY KEY CLUSTERED ([AddressID])
   ,CONSTRAINT [AK_Address_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)

CREATE NONCLUSTERED INDEX [IX_Address_AddressLine1_AddressLine2_City_StateProvince_PostalCode_CountryRegion] ON [SalesLT].[Address] ([AddressLine1], [AddressLine2], [City], [StateProvince], [PostalCode], [CountryRegion])
CREATE NONCLUSTERED INDEX [IX_Address_StateProvince] ON [SalesLT].[Address] ([StateProvince])

GO
