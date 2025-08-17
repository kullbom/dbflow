CREATE TABLE [Production].[ProductModelProductDescriptionCulture] (
   [ProductModelID] [int] NOT NULL,
   [ProductDescriptionID] [int] NOT NULL,
   [CultureID] [nchar](6) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductModelProductDescriptionCulture_ProductModelID_ProductDescriptionID_CultureID] PRIMARY KEY CLUSTERED ([ProductModelID], [ProductDescriptionID], [CultureID])
)


GO
