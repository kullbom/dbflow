CREATE TABLE [Production].[BillOfMaterials] (
   [BillOfMaterialsID] [int] NOT NULL
      IDENTITY (1,1),
   [ProductAssemblyID] [int] NULL,
   [ComponentID] [int] NOT NULL,
   [StartDate] [datetime] NOT NULL,
   [EndDate] [datetime] NULL,
   [UnitMeasureCode] [nchar](3) NOT NULL,
   [BOMLevel] [smallint] NOT NULL,
   [PerAssemblyQty] [decimal](8,2) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_BillOfMaterials_BillOfMaterialsID] PRIMARY KEY NONCLUSTERED ([BillOfMaterialsID])
)

CREATE UNIQUE CLUSTERED INDEX [AK_BillOfMaterials_ProductAssemblyID_ComponentID_StartDate] ON [Production].[BillOfMaterials] ([ProductAssemblyID], [ComponentID], [StartDate])
CREATE NONCLUSTERED INDEX [IX_BillOfMaterials_UnitMeasureCode] ON [Production].[BillOfMaterials] ([UnitMeasureCode])

GO
