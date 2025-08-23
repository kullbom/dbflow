SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
CREATE TRIGGER [SalesLT].[uSalesOrderHeader] ON [SalesLT].[SalesOrderHeader] 
AFTER UPDATE AS 
BEGIN
    DECLARE @Count int;

    SET @Count = @@ROWCOUNT;
    IF @Count = 0 
        RETURN;

    SET NOCOUNT ON;

    BEGIN TRY
        -- Update RevisionNumber for modification of any field EXCEPT the Status.
        IF NOT (UPDATE([Status]) OR UPDATE([RevisionNumber]))
        BEGIN
            UPDATE [SalesLT].[SalesOrderHeader]
            SET [SalesLT].[SalesOrderHeader].[RevisionNumber] = 
                [SalesLT].[SalesOrderHeader].[RevisionNumber] + 1
            WHERE [SalesLT].[SalesOrderHeader].[SalesOrderID] IN 
                (SELECT inserted.[SalesOrderID] FROM inserted);
        END;
    END TRY
    BEGIN CATCH
        EXECUTE [dbo].[uspPrintError];

        -- Rollback any active or uncommittable transactions before
        -- inserting information in the ErrorLog
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        EXECUTE [dbo].[uspLogError];
    END CATCH;
END;
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

ENABLE TRIGGER [SalesLT].[uSalesOrderHeader] ON [SalesLT].[SalesOrderHeader]
GO

GO
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'AFTER UPDATE trigger that updates the RevisionNumber and ModifiedDate columns in the SalesOrderHeader table.', N'SCHEMA', [SalesLT], N'TABLE', [SalesOrderHeader], N'TRIGGER', [uSalesOrderHeader];
