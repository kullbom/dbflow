CREATE TABLE [dbo].[ErrorLog] (
   [ErrorLogID] [INT] NOT NULL
      IDENTITY (1,1),
   [ErrorTime] [DATETIME] NOT NULL,
   [UserName] [SYSNAME] NOT NULL,
   [ErrorNumber] [INT] NOT NULL,
   [ErrorSeverity] [INT] NULL,
   [ErrorState] [INT] NULL,
   [ErrorProcedure] [NVARCHAR](126) NULL,
   [ErrorLine] [INT] NULL,
   [ErrorMessage] [NVARCHAR](4000) NOT NULL

   ,CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED ([ErrorLogID])
)


GO
