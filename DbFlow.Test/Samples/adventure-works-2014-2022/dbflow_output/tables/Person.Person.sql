CREATE TABLE [Person].[Person] (
   [BusinessEntityID] [INT] NOT NULL,
   [PersonType] [NCHAR](2) NOT NULL,
   [NameStyle] [NAMESTYLE] NOT NULL,
   [Title] [NVARCHAR](8) NULL,
   [FirstName] [NAME] NOT NULL,
   [MiddleName] [NAME] NULL,
   [LastName] [NAME] NOT NULL,
   [Suffix] [NVARCHAR](10) NULL,
   [EmailPromotion] [INT] NOT NULL,
   [AdditionalContactInfo] [XML] NULL,
   [Demographics] [XML] NULL,
   [rowguid] [UNIQUEIDENTIFIER] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Person_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE NONCLUSTERED INDEX [IX_Person_LastName_FirstName_MiddleName] ON [Person].[Person] ([LastName], [FirstName], [MiddleName])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Person_rowguid] ON [Person].[Person] ([rowguid])
CREATE PRIMARY XML INDEX [PXML_Person_AddContact] ON [Person].[Person] ([AdditionalContactInfo])
CREATE PRIMARY XML INDEX [PXML_Person_Demographics] ON [Person].[Person] ([Demographics])
CREATE PRIMARY XML INDEX [XMLPATH_Person_Demographics] ON [Person].[Person] ([Demographics])
CREATE PRIMARY XML INDEX [XMLPROPERTY_Person_Demographics] ON [Person].[Person] ([Demographics])
CREATE PRIMARY XML INDEX [XMLVALUE_Person_Demographics] ON [Person].[Person] ([Demographics])

GO
