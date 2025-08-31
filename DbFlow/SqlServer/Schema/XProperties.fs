namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/extended-properties-catalog-views-sys-extended-properties?view=sql-server-ver17

(*
To generate a script from the data in sys.extended_properties in SQL Server, you can create T-SQL statements that recreate the extended properties using the sp_addextendedproperty stored procedure. The sys.extended_properties system view contains metadata about extended properties, including their names, values, and the objects they are associated with (e.g., databases, tables, columns). Below is a step-by-step guide to generate a script that recreates these extended properties.Steps to Generate a Script from sys.extended_propertiesQuery sys.extended_properties:Use the sys.extended_properties view to retrieve the extended property details.
Join with other system views (e.g., sys.objects, sys.schemas, sys.columns) to get object names and hierarchy (schema, table, column).
Filter based on the class_desc column to identify the object type (e.g., DATABASE, OBJECT_OR_COLUMN).

Construct sp_addextendedproperty Statements:For each extended property, generate an EXEC sp_addextendedproperty statement with the appropriate parameters:@name: The name of the extended property.
@value: The value of the extended property.
@level0type, @level0name: Schema-level information (e.g., SCHEMA, dbo).
@level1type, @level1name: Object-level information (e.g., TABLE, MyTable).
@level2type, @level2name: Sub-object-level information (e.g., COLUMN, MyColumn).

Handle Different Object Types:Extended properties can apply to various objects (e.g., databases, tables, columns, stored procedures). The script must account for the object hierarchy and ensure correct parameter specification.

Output the Script:Concatenate the generated statements into a single script that can be executed to recreate the extended properties.

T-SQL Script to Generate Extended Property ScriptsBelow is a T-SQL script that generates sp_addextendedproperty statements for all extended properties in the current database, covering database-level, schema-level, and column-level properties:sql

SET NOCOUNT ON;

DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql = @sql + 
    CASE 
        -- Database-level properties (class = 0)
        WHEN ep.class = 0 THEN 
            'EXEC sp_addextendedproperty @name = N''' + 
            REPLACE(ep.name, '''', '''''') + ''', @value = N''' + 
            REPLACE(CAST(ep.value AS NVARCHAR(7500)), '''', '''''') + ''';' + CHAR(13) + CHAR(10)
        -- Object or column-level properties (class = 1)
        WHEN ep.class = 1 THEN 
            'EXEC sp_addextendedproperty @name = N''' + 
            REPLACE(ep.name, '''', '''''') + ''', @value = N''' + 
            REPLACE(CAST(ep.value AS NVARCHAR(7500)), '''', '''''') + ''', ' +
            '@level0type = N''SCHEMA'', @level0name = N''' + 
            REPLACE(OBJECT_SCHEMA_NAME(ep.major_id), '''', '''''') + ''', ' +
            '@level1type = N''' + 
            CASE 
                WHEN o.type = 'U' THEN 'TABLE'
                WHEN o.type = 'P' THEN 'PROCEDURE'
                WHEN o.type = 'V' THEN 'VIEW'
                ELSE o.type_desc 
            END + ''', @level1name = N''' + 
            REPLACE(OBJECT_NAME(ep.major_id), '''', '''''') + '''' +
            -- Add column-level details if applicable
            ISNULL(', @level2type = N''COLUMN'', @level2name = N''' + 
            REPLACE(c.name, '''', '''''') + '''', '') + ';' + CHAR(13) + CHAR(10)
        ELSE ''
    END
FROM sys.extended_properties ep
LEFT JOIN sys.objects o ON ep.major_id = o.object_id
LEFT JOIN sys.columns c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
WHERE ep.class IN (0, 1); -- Limit to database (0) and object/column (1) properties

-- Print or execute the generated script
PRINT @sql;
-- To execute the script directly, uncomment the following line:
-- EXEC sp_executesql @sql;

Explanation of the ScriptJoins:sys.objects provides object names (e.g., tables, views, procedures) for major_id.
sys.columns provides column names for minor_id when the property is on a column.

CASE Logic:Handles database-level properties (class = 0), which don’t require @level0type or @level1type.
Handles object/column-level properties (class = 1), specifying schema, object type, and optional column details.

REPLACE: Escapes single quotes in names and values to prevent SQL injection or syntax errors.
Output: The script generates EXEC sp_addextendedproperty statements, with each statement terminated by a semicolon and a newline for readability.

Example OutputFor a database with the following extended properties:Database-level: Name = 'Description', Value = 'Employee Database'
Table-level: Name = 'Purpose', Value = 'Stores employee data', Schema = 'dbo', Table = 'Employees'
Column-level: Name = 'Description', Value = 'Employee ID', Schema = 'dbo', Table = 'Employees', Column = 'EmpID'

The script might output:sql

EXEC sp_addextendedproperty @name = N'Description', @value = N'Employee Database';
EXEC sp_addextendedproperty @name = N'Purpose', @value = N'Stores employee data', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Employees';
EXEC sp_addextendedproperty @name = N'Description', @value = N'Employee ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Employees', @level2type = N'COLUMN', @level2name = N'EmpID';

NotesPermissions: You need db_ddladmin or higher permissions to add extended properties, but only SELECT permissions on sys.extended_properties to query them.

Limitations: Extended property values are limited to 7,500 bytes and stored as sql_variant.

Object Types: The script handles tables, views, procedures, and columns. To include other objects (e.g., triggers, indexes), modify the CASE statement to map o.type to the appropriate @level1type (e.g., TRIGGER, INDEX).

Dynamic SQL: The script uses dynamic SQL to build the statements. For large databases, you may need to handle the output in smaller batches due to PRINT limitations (8,000 characters max). Alternatively, save the output to a file using SSMS.
Existing Properties: To avoid errors when running the generated script, you may want to add checks to drop existing properties using sp_dropextendedproperty before adding them.

EnhancementsFilter Specific Objects: Add a WHERE clause to filter by specific schemas, tables, or property names (e.g., WHERE OBJECT_SCHEMA_NAME(ep.major_id) = 'dbo').
Include Drop Statements: To make the script idempotent, prepend sp_dropextendedproperty statements for each property:sql

'EXEC sp_dropextendedproperty @name = N''' + REPLACE(ep.name, '''', '''''') + ''', ' +
'@level0type = N''SCHEMA'', @level0name = N''' + REPLACE(OBJECT_SCHEMA_NAME(ep.major_id), '''', '''''') + ''', ' +
'@level1type = N''TABLE'', @level1name = N''' + REPLACE(OBJECT_NAME(ep.major_id), '''', '''''') + '''' +
ISNULL(', @level2type = N''COLUMN'', @level2name = N''' + REPLACE(c.name, '''', '''''') + '''', '') + ';' + CHAR(13) + CHAR(10)

Save to File: Use SSMS’s “Results to File” option or a tool like bcp to save the script to a .sql file.
Third-Party Tools: Tools like ApexSQL Search or Red Gate SQL Doc can simplify managing and scripting extended properties but may require licensing.

Testing the ScriptRun the script in SSMS with PRINT @sql to review the output.
Save the output to a .sql file or execute it directly (uncomment EXEC sp_executesql @sql).
Verify the recreated properties by querying sys.extended_properties again.

This script provides a robust way to generate sp_addextendedproperty statements from sys.extended_properties, making it easy to replicate extended properties across databases or environments. If you need help customizing the script (e.g., for specific objects or to include drop statements), let me know


*)

[<RequireQualifiedAccess>]
type XPropertyClass = 
    | Database = 0uy
    | ObjectOrColumn = 1uy
    | Parameter = 2uy
    | Schema = 3uy
    | DatabasePrincipal = 4uy
    | Assembly = 5uy
    | Type = 6uy
    | Index = 7uy
    // 8 = User defined table type column
    | XmlSchemaCollection = 10uy
    | MessageType = 15uy
    | ServiceContract = 16uy
    | Service = 17uy
    | RemoteServiceBinding = 18uy
    | Route = 19uy
    | Dataspace = 20uy
    | PartitionFunction = 21uy
    | DatabaseFile = 22uy
    | PlanGuide = 27uy

module XPROPERTY_CLASS = 
    let findClass classCode =
        match classCode with
        |  0uy -> XPropertyClass.Database
        |  1uy -> XPropertyClass.ObjectOrColumn
        |  2uy -> XPropertyClass.Parameter
        |  3uy -> XPropertyClass.Schema
        |  4uy -> XPropertyClass.DatabasePrincipal
        |  5uy -> XPropertyClass.Assembly
        |  6uy -> XPropertyClass.Type
        |  7uy -> XPropertyClass.Index
        | 10uy -> XPropertyClass.XmlSchemaCollection 
        | 15uy -> XPropertyClass.MessageType          
        | 16uy -> XPropertyClass.ServiceContract      
        | 17uy -> XPropertyClass.Service
        | 18uy -> XPropertyClass.RemoteServiceBinding 
        | 19uy -> XPropertyClass.Route
        | 20uy -> XPropertyClass.Dataspace
        | 21uy -> XPropertyClass.PartitionFunction 
        | 22uy -> XPropertyClass.DatabaseFile 
        | 27uy -> XPropertyClass.PlanGuide 
        | _ -> failwithf "Unknown XPROPERTY_CLASS : %i" classCode

module XProperty =
    let readAll connection =
        DbTr.readList
            "SELECT class, name, major_id, minor_id, value FROM sys.extended_properties"
            []
            (fun r -> 
                (XPROPERTY_CLASS.findClass (readByte "class" r), readInt32 "major_id" r, readInt32 "minor_id" r),
                readString "name" r, readString "value" r)
        |> DbTr.commit_ connection
        |> List.groupBy (fun (k,n,v) -> k)
        |> List.map 
            (fun (k, xs) -> 
                k,
                xs |> List.map (fun (k,n,v) -> n,v) |> Map.ofList)
        |> Map.ofList
        |> RCMap.ofMap

    let getXProperties key xProperties =
        match RCMap.tryPick key xProperties with
        | Some ps -> ps
        | None -> Map.empty