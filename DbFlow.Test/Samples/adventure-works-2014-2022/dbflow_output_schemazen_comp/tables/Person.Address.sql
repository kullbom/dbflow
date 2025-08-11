CREATE TABLE [Person].[Address] (
   [AddressID] [int] NOT NULL
      IDENTITY (1,1),
   [AddressLine1] [nvarchar](60) NOT NULL,
   [AddressLine2] [nvarchar](60) NULL,
   [City] [nvarchar](30) NOT NULL,
   [StateProvinceID] [int] NOT NULL,
   [PostalCode] [nvarchar](15) NOT NULL,
   [SpatialLocation] [geography] NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Address_AddressID] PRIMARY KEY CLUSTERED ([AddressID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Address_rowguid] ON [Person].[Address] ([rowguid])
CREATE UNIQUE NONCLUSTERED INDEX [IX_Address_AddressLine1_AddressLine2_City_StateProvinceID_PostalCode] ON [Person].[Address] ([AddressLine1], [AddressLine2], [City], [StateProvinceID], [PostalCode])
CREATE NONCLUSTERED INDEX [IX_Address_StateProvinceID] ON [Person].[Address] ([StateProvinceID])

GO
