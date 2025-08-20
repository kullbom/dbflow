CREATE TABLE [Sales].[SalesTaxRate] (
   [SalesTaxRateID] [INT] NOT NULL
      IDENTITY (1,1),
   [StateProvinceID] [INT] NOT NULL,
   [TaxType] [TINYINT] NOT NULL,
   [TaxRate] [SMALLMONEY] NOT NULL,
   [Name] [NAME] NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_SalesTaxRate_SalesTaxRateID] PRIMARY KEY CLUSTERED ([SalesTaxRateID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTaxRate_StateProvinceID_TaxType] ON [Sales].[SalesTaxRate] ([StateProvinceID], [TaxType])
CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesTaxRate_rowguid] ON [Sales].[SalesTaxRate] ([rowguid])

GO
