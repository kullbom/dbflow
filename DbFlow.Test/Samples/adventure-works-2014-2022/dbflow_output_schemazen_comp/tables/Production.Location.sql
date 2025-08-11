CREATE TABLE [Production].[Location] (
   [LocationID] [smallint] NOT NULL
      IDENTITY (1,1),
   [Name] [nvarchar](50) NOT NULL,
   [CostRate] [smallmoney] NOT NULL,
   [Availability] [decimal](8,2) NOT NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Location_LocationID] PRIMARY KEY CLUSTERED ([LocationID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Location_Name] ON [Production].[Location] ([Name])

GO
