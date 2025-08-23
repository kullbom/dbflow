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

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [Production], N'TABLE', [Location], N'COLUMN', [ModifiedDate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Work capacity (in hours) of the manufacturing location.', N'SCHEMA', [Production], N'TABLE', [Location], N'COLUMN', [Availability];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Standard hourly cost of the manufacturing location.', N'SCHEMA', [Production], N'TABLE', [Location], N'COLUMN', [CostRate];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Location description.', N'SCHEMA', [Production], N'TABLE', [Location], N'COLUMN', [Name];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Location records.', N'SCHEMA', [Production], N'TABLE', [Location], N'COLUMN', [LocationID];
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Product inventory and manufacturing locations.', N'SCHEMA', [Production], N'TABLE', [Location], NULL, NULL;
