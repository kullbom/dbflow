SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE VIEW [Person].[vStateProvinceCountryRegion]
WITH SCHEMABINDING
AS
SELECT
    sp.[StateProvinceID]
    ,sp.[StateProvinceCode]
    ,sp.[IsOnlyStateProvinceFlag]
    ,sp.[Name] AS [StateProvinceName]
    ,sp.[TerritoryID]
    ,cr.[CountryRegionCode]
    ,cr.[Name] AS [CountryRegionName]
FROM [Person].[StateProvince] sp
    INNER JOIN [Person].[CountryRegion] cr
    ON sp.[CountryRegionCode] = cr.[CountryRegionCode];
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO
