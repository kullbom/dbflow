namespace DbFlow.Tests.Units

open Xunit
open Xunit.Abstractions

open DbFlow
open DbFlow.SqlParser
open DbFlow.SqlParser.CodeSearch

type BatchParsing (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine

    [<Fact>]
    member x.``Sql batches 01`` () =
        let inputScript = "GO\r\nFoobar\r\nGO\r\n\r\nALTER F GO\r\nLast\r\nGO"
        let actual = 
            Batches.splitInSqlBatches
            |> Logger.logTime logger "splitInSqlBatches" inputScript
        let expected = ["Foobar"; "ALTER F"; "Last"]

        Assert.True ((expected, actual) ||> List.forall2 (=), "Not the expected result from 'splitInSqlBatches'")

    [<Fact>]
    member x.``Sql batches 02 - with comments`` () =
        let inputScript = "GO\r\nFoobar /*\r\nGO\r\n\r\nALTER F GO */\r\n-- Last\r\nGO"
        let actual = 
            Batches.splitInSqlBatches
            |> Logger.logTime logger "splitInSqlBatches" inputScript
        let expected = ["Foobar /*\r\nGO\r\n\r\nALTER F GO */\r\n-- Last"]

        Assert.True ((expected, actual) ||> List.forall2 (=), "Not the expected result from 'splitInSqlBatches'")

    [<Fact>]
    member x.``Sql batches 03 - simplified`` () =
        let inputScript = """/*
GO*/foo 
-- Help
CR A
GO // 'foo'
-- /* This 
-- b */
DROP
"""
        let m = Batches.collectSqlCommentsAndStrings inputScript
        let cs =
            m |> Seq.fold (fun acc kv -> inputScript.Substring(kv.Key, kv.Value - kv.Key) :: acc) [] |> List.rev
        let rest =
            m 
            |> Seq.fold (fun (i, acc) kv -> kv.Value, inputScript.Substring(i, kv.Key - i) :: acc) (0, []) 
            |> fun (i, acc) -> inputScript.Substring(i) :: acc
            |> List.rev
        let res0 = findFromSkipping [isWS; isStr "GO"; isWS] m inputScript 0
        
        let actual = 
            Batches.splitInSqlBatches
            |> Logger.logTime logger "splitInSqlBatches" inputScript
        let expected = [
            "/*
GO*/foo 
-- Help
CR A"
            "// 'foo'
-- /* This 
-- b */
DROP"
        ]

        Assert.True ((expected, actual) ||> List.forall2 (=), "Not the expected result from 'splitInSqlBatches'")
        ()
        
    [<Fact>]
    member x.``Sql batches 03`` () =
        let inputScript = """/*
We have ...
GO
*/foo 

-- Help
CREATE PROC A
	EXEC sp_rename @OldName, @NewName, N'OBJECT'
GO // 'foo'

-- Rename 

exec A 'dbo', 'Abc', 'PK__Abc__', NULL
GO -- /* This is GO
-- Drop */
DROP PROCEDURE A
"""
        let m = Batches.collectSqlCommentsAndStrings inputScript
        let cs =
            m |> Seq.fold (fun acc kv -> inputScript.Substring(kv.Key, kv.Value - kv.Key) :: acc) [] |> List.rev
        let rest =
            m 
            |> Seq.fold (fun (i, acc) kv -> kv.Value, inputScript.Substring(i, kv.Key - i) :: acc) (0, []) 
            |> fun (i, acc) -> inputScript.Substring(i) :: acc
            |> List.rev

        //let gos = SqlDefinitions.collectGOStatements inputScript

        let actual = 
            Batches.splitInSqlBatches
            |> Logger.logTime logger "splitInSqlBatches" inputScript
        let expected = [
            "/*
We have ...
GO
*/foo 

-- Help
CREATE PROC A
	EXEC sp_rename @OldName, @NewName, N'OBJECT'"
            "// 'foo'

-- Rename 

exec A 'dbo', 'Abc', 'PK__Abc__', NULL"
            "-- /* This is GO
-- Drop */
DROP PROCEDURE A"
        ]

        Assert.True ((expected, actual) ||> List.forall2 (=), "Not the expected result from 'splitInSqlBatches'")
        ()

    [<Fact>]
    member x.``Sql batches 04`` () =
        let inputScript = """/*
We have a number of primary keys that got 'random' ... lorem ispu GO ---
	...
GO
*/ 

-- Helper function - droped at end
CREATE PROCEDURE RenameAutoNamedConstraint @TableSchema as VARCHAR(100), @TableName as VARCHAR(500), @AutoNamedConstraint VARCHAR(500), @DNewName VARCHAR(500) AS
	DECLARE @OldName as NVARCHAR(1000) = @TableSchema+'.'
	DECLARE @NewName as NVARCHAR(1000) = ISNULL(@DNewName, 'PK_'+@TableName)
	
	EXEC sp_rename @OldName, @NewName, N'OBJECT'
GO

-- Rename all implicitly named primary keys

exec RenameAutoNamedConstraint 'dbo', 'Abc', 'PK__Abc__', NULL
GO -- /* This is i good place to GO
-- Drop the helper function */
DROP PROCEDURE RenameAutoNamedConstraint
"""
        let m = Batches.collectSqlCommentsAndStrings inputScript
        let cs =
            m |> Seq.fold (fun acc kv -> inputScript.Substring(kv.Key, kv.Value - kv.Key) :: acc) [] |> List.rev

        //let gos = SqlDefinitions.collectGOStatements inputScript

        let actual = 
            Batches.splitInSqlBatches
            |> Logger.logTime logger "splitInSqlBatches" inputScript
        let expected = [
            """/*
We have a number of primary keys that got 'random' ... lorem ispu GO ---
	...
GO
*/ 

-- Helper function - droped at end
CREATE PROCEDURE RenameAutoNamedConstraint @TableSchema as VARCHAR(100), @TableName as VARCHAR(500), @AutoNamedConstraint VARCHAR(500), @DNewName VARCHAR(500) AS
	DECLARE @OldName as NVARCHAR(1000) = @TableSchema+'.'
	DECLARE @NewName as NVARCHAR(1000) = ISNULL(@DNewName, 'PK_'+@TableName)
	
	EXEC sp_rename @OldName, @NewName, N'OBJECT'"""
            """-- Rename all implicitly named primary keys

exec RenameAutoNamedConstraint 'dbo', 'Abc', 'PK__Abc__', NULL"""
            """-- /* This is i good place to GO
-- Drop the helper function */
DROP PROCEDURE RenameAutoNamedConstraint"""
        ]

        Assert.True ((expected, actual) ||> List.forall2 (=), "Not the expected result from 'splitInSqlBatches'")
        ()

    [<Fact>]
    member x.``Sql batches 05`` () =
        let inputScript = """EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The yschters of the KLI stringlar' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Gruftiy', @level2type=N'COLUMN',@level2name=N'HoogaBooga1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'When Fgler a HEY, the striglars are not in the asfkdj by (1) or (2) BKr, and (3) AKr.  Thus, if two Wizards have BKr, they are "supposed" to go in different plyschers.  For example, notes and NILT workds go in different plyschers.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Gruftiy', @level2type=N'COLUMN',@level2name=N'HoogaBooga2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'If ''tyberlu'' is y, this is a flue list of ''trufely'', ''Sct'', und ''Bdy''; otherwise it must be really empty.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Gruftiy', @level2type=N'COLUMN',@level2name=N'HoogaBooga3'
GO
"""
        let m = Batches.collectSqlCommentsAndStrings inputScript
        let cs =
            m |> Seq.fold (fun acc kv -> inputScript.Substring(kv.Key, kv.Value - kv.Key) :: acc) [] |> List.rev

        let actual = 
            Batches.splitInSqlBatches
            |> Logger.logTime logger "splitInSqlBatches" inputScript
        let expected = [
            """EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The yschters of the KLI stringlar' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Gruftiy', @level2type=N'COLUMN',@level2name=N'HoogaBooga1'"""
            """EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'When Fgler a HEY, the striglars are not in the asfkdj by (1) or (2) BKr, and (3) AKr.  Thus, if two Wizards have BKr, they are "supposed" to go in different plyschers.  For example, notes and NILT workds go in different plyschers.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Gruftiy', @level2type=N'COLUMN',@level2name=N'HoogaBooga2'"""
            """EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'If ''tyberlu'' is y, this is a flue list of ''trufely'', ''Sct'', und ''Bdy''; otherwise it must be really empty.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Gruftiy', @level2type=N'COLUMN',@level2name=N'HoogaBooga3'"""
        ]

        Assert.True ((expected, actual) ||> List.forall2 (=), "Not the expected result from 'splitInSqlBatches'")
        ()

type DefinitionParsing (outputHelper:ITestOutputHelper) = 
    let logger = Logger.create outputHelper.WriteLine

    [<Fact>]
    member x.``Trigger``() =
        let input = "SET ANSI_NULLS ON 
GO
CREATE TRIGGER orig_trigger_name
ON ns.orig_parent_name  AFTER INSERT 
AS BEGIN
"
        let tName = "Foo"
        let pName = "Bar"
        let expected = 
            input.Replace("orig_trigger_name", tName).Replace("ns.orig_parent_name", pName)
        Assert.Equal(expected, SqlDefinitions.updateTriggerDefinition tName pName input)


    [<Fact>]
    member x.``Read large file`` () =
        // 2025-08-08 
        // File contains 1040715 characters
        // 16:45:53.639 Executed collectSqlCommentsAndStrings (took 18062 ms)
        // 16:48:01.193 Executed split on GO (took 127552 ms)
        // Found 3995 batches
        
        // 2025-08-10
        // File contains 1040715 characters
        // 16:18:20.336 Executed splitInSqlBatches (took 434 ms)
        // Found 3995 batches

    
        let file' = (__SOURCE_DIRECTORY__ + "\\..\\..\\dbflow-regression\\large-example.sql")
        let file = System.IO.Path.GetFullPath file'
        if System.IO.File.Exists file
        then
            let input = System.IO.File.ReadAllText file

            logger.info $"File contains {input.Length} characters"

            let bs = 
                Batches.splitInSqlBatches
                |> Logger.logTime logger "splitInSqlBatches" input
                
            let nBatches = bs |> List.length
            
            logger.info $"Found {nBatches} batches"

            Assert.Equal (3995, nBatches)
        else 
            () // Should "Skip"


        


