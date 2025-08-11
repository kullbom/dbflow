CREATE TABLE [SalesLT].[ProductDescription] (
   [ProductDescriptionID] [INT] NOT NULL
      IDENTITY (1,1),
   [Description] [NVARCHAR](400) NOT NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL
       DEFAULT (newid()) ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_ProductDescription_ProductDescriptionID] PRIMARY KEY CLUSTERED ([ProductDescriptionID])
   ,CONSTRAINT [AK_ProductDescription_rowguid] UNIQUE NONCLUSTERED ([rowguid])
)


GO
