CREATE TABLE [Purchasing].[ShipMethod] (
   [ShipMethodID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ShipBase] [MONEY] NOT NULL,
   [ShipRate] [MONEY] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ShipMethod_ShipMethodID] PRIMARY KEY CLUSTERED ([ShipMethodID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ShipMethod_Name] ON [Purchasing].[ShipMethod] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_ShipMethod_rowguid] ON [Purchasing].[ShipMethod] ([rowguid])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Purchasing], N'TABLE', [ShipMethod], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.', N'SCHEMA', [Purchasing], N'TABLE', [ShipMethod], N'COLUMN', [rowguid];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shipping charge per pound.', N'SCHEMA', [Purchasing], N'TABLE', [ShipMethod], N'COLUMN', [ShipRate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Minimum shipping charge.', N'SCHEMA', [Purchasing], N'TABLE', [ShipMethod], N'COLUMN', [ShipBase];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shipping company name.', N'SCHEMA', [Purchasing], N'TABLE', [ShipMethod], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for ShipMethod records.', N'SCHEMA', [Purchasing], N'TABLE', [ShipMethod], N'COLUMN', [ShipMethodID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shipping company lookup table.', N'SCHEMA', [Purchasing], N'TABLE', [ShipMethod], NULL, NULL;
