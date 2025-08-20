CREATE TABLE [Production].[ProductModelProductDescriptionCulture] (
   [ProductModelID] [INT] NOT NULL,
   [ProductDescriptionID] [INT] NOT NULL,
   [CultureID] [NCHAR](6) NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ProductModelProductDescriptionCulture_ProductModelID_ProductDescriptionID_CultureID] PRIMARY KEY CLUSTERED ([ProductModelID], [ProductDescriptionID], [CultureID])
)


GO
