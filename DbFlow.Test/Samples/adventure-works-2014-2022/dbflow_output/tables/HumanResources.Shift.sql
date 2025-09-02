CREATE TABLE [HumanResources].[Shift] (
   [ShiftID] [TINYINT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [StartTime] [TIME] NOT NULL,
   [EndTime] [TIME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Shift_ShiftID] PRIMARY KEY CLUSTERED ([ShiftID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Shift_Name] ON [HumanResources].[Shift] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Shift_StartTime_EndTime] ON [HumanResources].[Shift] ([StartTime], [EndTime])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Work shift lookup table.', N'SCHEMA', [HumanResources], N'TABLE', [Shift];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Shift records.', N'SCHEMA', [HumanResources], N'TABLE', [Shift], N'COLUMN', [ShiftID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shift description.', N'SCHEMA', [HumanResources], N'TABLE', [Shift], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shift start time.', N'SCHEMA', [HumanResources], N'TABLE', [Shift], N'COLUMN', [StartTime];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Shift end time.', N'SCHEMA', [HumanResources], N'TABLE', [Shift], N'COLUMN', [EndTime];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Date and time the record was last updated.', N'SCHEMA', [HumanResources], N'TABLE', [Shift], N'COLUMN', [ModifiedDate];
