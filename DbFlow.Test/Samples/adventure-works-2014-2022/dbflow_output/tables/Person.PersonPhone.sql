CREATE TABLE [Person].[PersonPhone] (
   [BusinessEntityID] [INT] NOT NULL,
   [PhoneNumber] [PHONE] NOT NULL,
   [PhoneNumberTypeID] [INT] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_PersonPhone_BusinessEntityID_PhoneNumber_PhoneNumberTypeID] PRIMARY KEY CLUSTERED ([BusinessEntityID], [PhoneNumber], [PhoneNumberTypeID])
)

CREATE NONCLUSTERED INDEX [IX_PersonPhone_PhoneNumber] ON [Person].[PersonPhone] ([PhoneNumber])

GO
