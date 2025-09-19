CREATE TABLE [Production].[BillOfMaterials] (
   [BillOfMaterialsID] [INT] NOT NULL
      IDENTITY (1,1),
   [ProductAssemblyID] [INT] NULL,
   [ComponentID] [INT] NOT NULL,
   [StartDate] [DATETIME] NOT NULL,
   [EndDate] [DATETIME] NULL,
   [UnitMeasureCode] [NCHAR](3) NOT NULL,
   [BOMLevel] [SMALLINT] NOT NULL,
   [PerAssemblyQty] [DECIMAL](8,2) NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_BillOfMaterials_BillOfMaterialsID] PRIMARY KEY NONCLUSTERED ([BillOfMaterialsID])
)

CREATE UNIQUE CLUSTERED INDEX [AK_BillOfMaterials_ProductAssemblyID_ComponentID_StartDate] ON [Production].[BillOfMaterials] ([ProductAssemblyID], [ComponentID], [StartDate])
CREATE NONCLUSTERED INDEX [IX_BillOfMaterials_UnitMeasureCode] ON [Production].[BillOfMaterials] ([UnitMeasureCode])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Items required to make bicycles and bicycle subassemblies. It identifies the heirarchical relationship between a parent product and its components.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for BillOfMaterials records.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [BillOfMaterialsID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Parent product identification number. Foreign key to Product.ProductID.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [ProductAssemblyID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Component identification number. Foreign key to Product.ProductID.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [ComponentID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the component started being used in the assembly item.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [StartDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date the component stopped being used in the assembly item.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [EndDate];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Standard code identifying the unit of measure for the quantity.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [UnitMeasureCode];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Indicates the depth the component is from its parent (AssemblyID).', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [BOMLevel];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Quantity of the component needed to create the assembly.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [PerAssemblyQty];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [BillOfMaterials], N'COLUMN', [ModifiedDate];
