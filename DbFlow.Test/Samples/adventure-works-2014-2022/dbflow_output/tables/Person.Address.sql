CREATE TABLE [Person].[Address] (
   [AddressID] [INT] NOT NULL
      IDENTITY (1,1),
   [AddressLine1] [NVARCHAR](60) NOT NULL,
   [AddressLine2] [NVARCHAR](60) NULL,
   [City] [NVARCHAR](30) NOT NULL,
   [StateProvinceID] [INT] NOT NULL,
   [PostalCode] [NVARCHAR](15) NOT NULL,
   [SpatialLocation] [GEOGRAPHY] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_Address_AddressID] PRIMARY KEY CLUSTERED ([AddressID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Address_rowguid] ON [Person].[Address] ([rowguid])
CREATE UNIQUE NONCLUSTERED INDEX [IX_Address_AddressLine1_AddressLine2_City_StateProvinceID_PostalCode] ON [Person].[Address] ([AddressLine1], [AddressLine2], [City], [StateProvinceID], [PostalCode])
CREATE NONCLUSTERED INDEX [IX_Address_StateProvinceID] ON [Person].[Address] ([StateProvinceID])

GO
