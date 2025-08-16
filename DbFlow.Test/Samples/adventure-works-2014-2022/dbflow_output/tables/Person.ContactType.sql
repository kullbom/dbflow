CREATE TABLE [Person].[ContactType] (
   [ContactTypeID] [INT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_ContactType_ContactTypeID] PRIMARY KEY CLUSTERED ([ContactTypeID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_ContactType_Name] ON [Person].[ContactType] ([Name])

GO
