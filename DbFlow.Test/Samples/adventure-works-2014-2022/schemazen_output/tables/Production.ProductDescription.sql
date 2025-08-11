CREATE TABLE [Production].[ProductDescription] (
   [ProductDescriptionID] [int] NOT NULL
      IDENTITY (1,1),
   [Description] [nvarchar](400) NOT NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_ProductDescription_ProductDescriptionID] PRIMARY KEY CLUSTERED ([ProductDescriptionID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductDescription_rowguid] ON [Production].[ProductDescription] ([rowguid])

GO
