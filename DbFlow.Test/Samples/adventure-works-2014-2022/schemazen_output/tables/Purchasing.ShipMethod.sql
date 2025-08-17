CREATE TABLE [Purchasing].[ShipMethod] (
   [ShipMethodID] [int] NOT NULL
      IDENTITY (1,1),
   [Name] [nvarchar](50) NOT NULL,
   [ShipBase] [money] NOT NULL,
   [ShipRate] [money] NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ShipMethod_ShipMethodID] PRIMARY KEY CLUSTERED ([ShipMethodID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ShipMethod_Name] ON [Purchasing].[ShipMethod] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_ShipMethod_rowguid] ON [Purchasing].[ShipMethod] ([rowguid])

GO
