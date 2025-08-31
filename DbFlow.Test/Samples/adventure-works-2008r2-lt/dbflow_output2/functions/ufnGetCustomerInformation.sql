SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
CREATE FUNCTION [dbo].[ufnGetCustomerInformation](@CustomerID int)
RETURNS TABLE 
AS 
-- Returns the CustomerID, first name, and last name for the specified customer.
RETURN (
    SELECT 
        CustomerID, 
        FirstName, 
        LastName
    FROM [SalesLT].[Customer] 
    WHERE [CustomerID] = @CustomerID
);
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Input parameter for the table value function ufnGetCustomerInformation. Enter a valid CustomerID from the Sales.Customer table.', N'SCHEMA', [dbo], N'FUNCTION', [ufnGetCustomerInformation], N'PARAMETER', '@CustomerID';
EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Table value function returning the customer ID, first name, and last name for a given customer.', N'SCHEMA', [dbo], N'FUNCTION', [ufnGetCustomerInformation];
