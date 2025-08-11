CREATE TABLE [Production].[UnitMeasure] (
   [UnitMeasureCode] [nchar](3) NOT NULL,
   [Name] [nvarchar](50) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_UnitMeasure_UnitMeasureCode] PRIMARY KEY CLUSTERED ([UnitMeasureCode])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_UnitMeasure_Name] ON [Production].[UnitMeasure] ([Name])

GO
