namespace DbFlow.Tests

open Xunit
open Microsoft.Data.SqlClient

open DbFlow
open DbFlow.SqlServer

module Helpers = 
    let separateTableScript (fullTableScript : string) =
        fullTableScript.Split("\r\n")
        |> Array.fold 
            (fun (sb : System.Text.StringBuilder, inlineKeys, standAloneKeys, documentation) (line : string) ->
                match line with
                | "" ->
                    // Filter away empty lines as well ... 
                    sb, inlineKeys, standAloneKeys, documentation
                | _ when line.StartsWith "CREATE" && not (line.StartsWith "CREATE TABLE") ->
                    sb, inlineKeys, line :: standAloneKeys, documentation
                | _ when line.StartsWith "   ," ->
                    sb, line :: inlineKeys, standAloneKeys, documentation
                | _ when line.StartsWith "EXECUTE [sys].[sp_addextendedproperty]" ->
                    sb, inlineKeys, standAloneKeys, line :: documentation
                | _ ->
                    sb.AppendLine line, inlineKeys, standAloneKeys, documentation)
            (System.Text.StringBuilder (), [], [], [])
        |> fun (sb, inlineKeys, standAloneKeys, documentation) -> 
            sb.ToString (), 
            inlineKeys |> List.sort |> List.toArray,
            standAloneKeys |> List.sort |> List.toArray,
            documentation |> List.sort |> List.toArray
                    
    let aEqual a0 a1 = 
        (a0 |> Array.length) = (a1 |> Array.length) 
        && (a0, a1) ||> Array.forall2 (fun (s0 : string) (s1 : string) -> s0.ToUpperInvariant() = s1.ToUpperInvariant()) 

    let sortedSubdirs d =
        System.IO.Directory.GetDirectories d 
        |> Array.map (fun d -> d.Substring(d.LastIndexOf("\\") + 1))
        |> Array.sortBy (fun s -> s.ToUpperInvariant())
        
    let sortedFiles d =
        System.IO.Directory.GetFiles d 
        |> Array.map (fun d -> d.Substring(d.LastIndexOf("\\") + 1))
        |> Array.sortBy (fun s -> s.ToUpperInvariant())
        
    let compareScriptFolder logger dirName dir0 dir1 =
        let files0 = sortedFiles dir0
        let files1 = sortedFiles dir1

        if not (aEqual files0 files1)
        then Assert.Fail $"Different files in '{dirName}'"

        for file in files0 do
            let fileContent0 = System.IO.File.ReadAllText (System.IO.Path.Combine(dir0, file))
            let fileContent1 = System.IO.File.ReadAllText (System.IO.Path.Combine(dir1, file))
            match dirName with
            // The order of constraints and keys does not seems to be deterministic
            | "check_constraints"
            | "foreign_keys"
            | "defaults" ->
                let defs0 = fileContent0 |> SqlParser.Batches.splitInSqlBatches |> List.sort |> List.toArray
                let defs1 = fileContent1 |> SqlParser.Batches.splitInSqlBatches |> List.sort |> List.toArray

                if not (aEqual defs0 defs1)
                then Assert.Fail $"The content of {dirName}\\{file} is not the same"

            | "tables" ->
                // In table definitions the key order does not seems to be deterministic
                let (content0, inlineKeys0, standAloneKeys0, doc0) = separateTableScript fileContent0    
                let (content1, inlineKeys1, standAloneKeys1, doc1) = separateTableScript fileContent1

                Assert.True ((content0 = content1), $"The content of {dirName}\\{file} is not the same")

                if not (aEqual inlineKeys0 inlineKeys1)
                then Assert.Fail $"The content ('inline keys') of {dirName}\\{file} is not the same"

                if not (aEqual standAloneKeys0 standAloneKeys1)
                then Assert.Fail $"The content ('stand alone keys') of {dirName}\\{file} is not the same"

                if not (aEqual doc0 doc1)
                then Assert.Fail $"The content ('documentation') of {dirName}\\{file} is not the same"
                ()
            // These folders are expected to contain identical files
            | "" // Root folder
            | "table_types"
            | "xmlschemacollections"
            | "synonyms" 
            | "functions" 
            | "procedures" 
            | "schemas" 
            | "triggers" 
            | "sequences" 
            | "user_defined_types" 
            | "views" -> 
                Assert.True ((fileContent0 = fileContent1), $"The content of {dirName}\\{file} is not the same")
                ()
            | _ -> failwithf "Unknown subdirectory %s" dirName
    
    let compareScriptFolders logger dir0 dir1 =
        
        compareScriptFolder logger "" dir0 dir1
        
        let subdirs0 = sortedSubdirs dir0 
        let subdirs1 = sortedSubdirs dir1 
        
        if not (aEqual subdirs0 subdirs1)
        then Assert.Fail $"{subdirs0} != {subdirs1}"

        for dir in subdirs0 do
            let subdir0 = System.IO.Path.Combine(dir0, dir)
            let subdir1 = System.IO.Path.Combine(dir1, dir)
            
            compareScriptFolder logger dir subdir0 subdir1
            
    
    let withLocalDb logger f =
        use localDb = new SqlServer.LocalTempDb(logger)
        let localDbConnectionString = localDb.ConnectionString
        let timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff")
        logger.info $"{timestamp} New local db: {localDbConnectionString}" 
        
        f localDbConnectionString

    let withLocalDbFromScripts logger scriptFolder f =
        withLocalDb logger
            (fun connectionStr ->
                Execute.performDbUpgrade logger connectionStr scriptFolder

                f connectionStr)

    