CREATE TABLE [Production].[Culture] (
   [CultureID] [NCHAR](6) NOT NULL,
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Culture_CultureID] PRIMARY KEY CLUSTERED ([CultureID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Culture_Name] ON [Production].[Culture] ([Name])

GO
