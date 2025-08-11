CREATE TABLE [SalesLT].[ProductDescription] (
   [ProductDescriptionID] [int] NOT NULL
      IDENTITY (1,1),
   [Description] [nvarchar](400) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [AK_ProductDescription_rowguid] UNIQUE NONCLUSTERED ([rowguid])
   ,CONSTRAINT [PK_ProductDescription_ProductDescriptionID] PRIMARY KEY CLUSTERED ([ProductDescriptionID])
)


GO
