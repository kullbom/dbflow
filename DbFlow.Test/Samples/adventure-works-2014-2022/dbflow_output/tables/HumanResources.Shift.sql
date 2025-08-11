CREATE TABLE [HumanResources].[Shift] (
   [ShiftID] [TINYINT] NOT NULL
      IDENTITY (1,1),
   [Name] [NAME] NOT NULL,
   [StartTime] [TIME] NOT NULL,
   [EndTime] [TIME] NOT NULL,
   [ModifiedDate] [DATETIME] NOT NULL
       DEFAULT (getdate())

   ,CONSTRAINT [PK_Shift_ShiftID] PRIMARY KEY CLUSTERED ([ShiftID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Shift_Name] ON [HumanResources].[Shift] ([Name])
CREATE UNIQUE NONCLUSTERED INDEX [AK_Shift_StartTime_EndTime] ON [HumanResources].[Shift] ([StartTime], [EndTime])

GO
