CREATE TABLE [dbo].[ErrorLog] (
   [ErrorLogID] [int] NOT NULL
      IDENTITY (1,1),
   [ErrorTime] [datetime] NOT NULL,
   [UserName] [nvarchar](128) NOT NULL,
   [ErrorNumber] [int] NOT NULL,
   [ErrorSeverity] [int] NULL,
   [ErrorState] [int] NULL,
   [ErrorProcedure] [nvarchar](126) NULL,
   [ErrorLine] [int] NULL,
   [ErrorMessage] [nvarchar](4000) NOT NULL

   ,CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED ([ErrorLogID])
)


GO
