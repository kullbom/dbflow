namespace DbFlow.Tests.Units

open Xunit
open Xunit.Abstractions

open DbFlow
open DbFlow.SqlParser
open DbFlow.SqlParser.CodeSearch

type Search (outputHelper:ITestOutputHelper) = 
    let logger s = outputHelper.WriteLine s
    
    [<Fact>]
    member x.``Search 01``() =
        let pattern = [isWS; isCh 'a'; isWS] 
        let find_a s = 
            let (startPos, endPos) = find pattern s
            if startPos <> -1
            then Assert.Equal(startPos + 3, endPos)
            startPos
                
        Assert.Equal (8, find_a "fasdhj b a hj")
        Assert.Equal (8, find_a "fasdhj b a ")
        Assert.Equal (-1, find_a "fasdhj b a")
        Assert.Equal (0, find_a " a  fasdhj b a")

        Assert.Equal (8, find_a "FASDHJ B A HJ")
        Assert.Equal (8, find_a "FASDHJ B A ")
        Assert.Equal (-1, find_a "FASDHJ B A")
        Assert.Equal (0, find_a " A  FASDHJ B A")

    [<Fact>]
    member x.``Search 02``() =
        let pattern = [isWS; isStr "go"; isWS] 
        let find_go s = 
            let (startPos, endPos) = find pattern s
            if startPos <> -1
            then Assert.Equal(startPos + 4, endPos)
            startPos
                
        Assert.Equal (8, find_go "fasdhj 
GO b a hj")
        Assert.Equal (8, find_go "fasdhj b GO a ")
        Assert.Equal (9, find_go "fasdhj b\t\tGO\t a")
        Assert.Equal (-1, find_go " a  fasdhj b a")
        Assert.Equal (0, find_go " go  fasdhj b a")

        Assert.Equal (8, find_go "FASDHJ 
GO B A HJ")
        Assert.Equal (8, find_go "FASDHJ B GO A ")
        Assert.Equal (9, find_go "FASDHJ B\t\tGO\t A")
        Assert.Equal (-1, find_go " A  FASDHJ B A")
        Assert.Equal (0, find_go " GO  FASDHJ B A")

        Assert.Equal (8, find_go "fasdhj 
gO b a hj")
        Assert.Equal (8, find_go "fasdhj b go a ")
        Assert.Equal (9, find_go "fasdhj b\t\tGo\t a")
        Assert.Equal (-1, find_go " a  fasdhj b a go")
        Assert.Equal (-1, find_go "GO a  fasdhj b a go")
        Assert.Equal (0, find_go " gO  fasdhj b a")

    [<Fact>]
    member x.``Find and skip`` () =
        let input = "xjxoxhxaxnx"
        let xs = 
            let rec collect_x m i =
                match findFrom [isCh 'x'] input i with
                | (-1,_) -> m
                | (s, e) -> collect_x (Map.add s e m) e
            collect_x Map.empty 0
        let cs =
            xs |> Map.fold (fun acc s e -> input.Substring(s, e - s) :: acc) [] |> List.rev
        let rest =
            xs
            |> Map.fold (fun (i, acc) s e -> e, input.Substring(i, s - i) :: acc) (0, []) 
            |> fun (i, acc) -> input.Substring(i) :: acc
            |> List.rev
        let res = findFromSkipping [isStr "johan"] xs input 0
        ()
            
type BatchParsing (outputHelper:ITestOutputHelper) = 
    let logger s = outputHelper.WriteLine s
    
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
        let (m, _) = Batches.collectSqlCommentsAndStrings inputScript
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
        let (m, _) = Batches.collectSqlCommentsAndStrings inputScript
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
        let (m, _) = Batches.collectSqlCommentsAndStrings inputScript
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
        let (m, _) = Batches.collectSqlCommentsAndStrings inputScript
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
    let logger s = outputHelper.WriteLine s
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

            logger $"File contains {input.Length} characters"

            let bs = 
                Batches.splitInSqlBatches
                |> Logger.logTime logger "splitInSqlBatches" input
                
            let nBatches = bs |> List.length
            
            logger $"Found {nBatches} batches"

            Assert.Equal (3995, nBatches)
        else 
            () // Should "Skip"


        


