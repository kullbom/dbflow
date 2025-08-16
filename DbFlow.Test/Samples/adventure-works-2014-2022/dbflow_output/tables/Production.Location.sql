CREATE TABLE [Production].[Location] (
   [LocationID] [SMALLINT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [CostRate] [SMALLMONEY] NOT NULL,
   [Availability] [DECIMAL](8,2) NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Location_LocationID] PRIMARY KEY CLUSTERED ([LocationID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Location_Name] ON [Production].[Location] ([Name])

GO
