CREATE TABLE [Person].[PersonPhone] (
   [BusinessEntityID] [int] NOT NULL,
   [PhoneNumber] [nvarchar](25) NOT NULL,
   [PhoneNumberTypeID] [int] NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_PersonPhone_BusinessEntityID_PhoneNumber_PhoneNumberTypeID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [PhoneNumber], [PhoneNumberTypeID])
)

CREATE NONCLUSTERED INDEX [IX_PersonPhone_PhoneNumber] ON [Person].[PersonPhone] ([PhoneNumber])

GO
