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
