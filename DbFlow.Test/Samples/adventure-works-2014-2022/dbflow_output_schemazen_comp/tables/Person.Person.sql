CREATE TABLE [Person].[Person] (
   [BusinessEntityID] [int] NOT NULL,
   [PersonType] [nchar](2) NOT NULL,
   [NameStyle] [bit] NOT NULL,
   [Title] [nvarchar](8) NULL,
   [FirstName] [nvarchar](50) NOT NULL,
   [MiddleName] [nvarchar](50) NULL,
   [LastName] [nvarchar](50) NOT NULL,
   [Suffix] [nvarchar](10) NULL,
   [EmailPromotion] [int] NOT NULL,
   [AdditionalContactInfo] [xml] NULL,
   [Demographics] [xml] NULL,
   [rowguid] [uniqueidentifier] NOT NULL ROWGUIDCOL ,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Person_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE NONCLUSTERED INDEX [IX_Person_LastName_FirstName_MiddleName] ON [Person].[Person] ([LastName], [FirstName], [MiddleName])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Person_rowguid] ON [Person].[Person] ([rowguid])
CREATE XML INDEX [PXML_Person_AddContact] ON [Person].[Person] ([AdditionalContactInfo])
CREATE XML INDEX [PXML_Person_Demographics] ON [Person].[Person] ([Demographics])
CREATE XML INDEX [XMLPATH_Person_Demographics] ON [Person].[Person] ([Demographics])
CREATE XML INDEX [XMLPROPERTY_Person_Demographics] ON [Person].[Person] ([Demographics])
CREATE XML INDEX [XMLVALUE_Person_Demographics] ON [Person].[Person] ([Demographics])

GO
