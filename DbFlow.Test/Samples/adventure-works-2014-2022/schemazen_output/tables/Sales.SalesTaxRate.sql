CREATE TABLE [Sales].[SalesTaxRate] (
   [SalesTaxRateID] [int] NOT NULL
      IDENTITY (1,1),
   [StateProvinceID] [int] NOT NULL,
   [TaxType] [tinyint] NOT NULL,
   [TaxRate] [smallmoney] NOT NULL,
   [Name] [nvarchar](50) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_SalesTaxRate_SalesTaxRateID] PRIMARY KEY CLUSTERED ([SalesTaxRateID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTaxRate_rowguid] ON [Sales].[SalesTaxRate] ([rowguid])
CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTaxRate_StateProvinceID_TaxType] ON [Sales].[SalesTaxRate] ([StateProvinceID], [TaxType])

GO
