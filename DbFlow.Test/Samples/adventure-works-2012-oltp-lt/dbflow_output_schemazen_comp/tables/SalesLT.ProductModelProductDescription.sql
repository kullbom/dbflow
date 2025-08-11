CREATE TABLE [SalesLT].[ProductModelProductDescription] (
   [ProductModelID] [int] NOT NULL,
   [ProductDescriptionID] [int] NOT NULL,
   [Culture] [nchar](6) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductModelProductDescription_ProductModelID_ProductDescriptionID_Culture] PRIMARY KEY CLUSTERED ([ProductModelID], [ProductDescriptionID], [Culture])
   ,CONSTRAINT [AK_ProductModelProductDescription_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO
