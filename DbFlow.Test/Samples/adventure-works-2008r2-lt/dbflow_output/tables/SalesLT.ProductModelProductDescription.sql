CREATE TABLE [SalesLT].[ProductModelProductDescription] (
   [ProductModelID] [INT] NOT NULL,
   [ProductDescriptionID] [INT] NOT NULL,
   [Culture] [NCHAR](6) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_ProductModelProductDescription_ProductModelID_ProductDescriptionID_Culture] PRIMARY KEY CLUSTERED ([ProductModelID], [ProductDescriptionID], [Culture])
   ,CONSTRAINT [AK_ProductModelProductDescription_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO
