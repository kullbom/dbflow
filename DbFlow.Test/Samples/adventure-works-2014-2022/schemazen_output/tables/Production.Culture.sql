CREATE TABLE [Production].[Culture] (
   [CultureID] [nchar](6) NOT NULL,
   [Name] [nvarchar](50) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Culture_CultureID] PRIMARY KEY CLUSTERED ([CultureID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Culture_Name] ON [Production].[Culture] ([Name])

GO
