CREATE TABLE [Production].[UnitMeasure] (
   [UnitMeasureCode] [NCHAR](3) NOT NULL,
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_UnitMeasure_UnitMeasureCode] PRIMARY KEY CLUSTERED ([UnitMeasureCode])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_UnitMeasure_Name] ON [Production].[UnitMeasure] ([Name])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unit of measure lookup table.', N'SCHEMA', [Production], N'TABLE', [UnitMeasure];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key.', N'SCHEMA', [Production], N'TABLE', [UnitMeasure], N'COLUMN', [UnitMeasureCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Unit of measure description.', N'SCHEMA', [Production], N'TABLE', [UnitMeasure], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [UnitMeasure], N'COLUMN', [ModifiedDate];
