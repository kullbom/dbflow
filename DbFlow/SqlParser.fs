module DbFlow.SqlParser

module CodeSearch =
    type SearchPattern =
        | SimplePattern of (char -> bool)
        | CompoundPattern of SearchPattern list
    
    let isCh (c : char) =
        SimplePattern (fun c0 -> System.Char.ToUpperInvariant(c) = System.Char.ToUpperInvariant(c0)) 

    let isStr (s : string) =
        CompoundPattern (s |> Seq.toList |> List.map isCh)

    let WS = ['\r'; '\n'; '\t'; ' '] |> Set.ofList
    
    let isIn cs = SimplePattern (fun c -> Set.contains c cs)
    
    let isNotIn cs = SimplePattern (fun c -> not <| Set.contains c cs)
    
    let isWS = isIn WS
    
    let isNotWS = isNotIn WS
    
    module private Internal =
        let rec flattenPattern flattened ps =
            match ps with
            | [] -> flattened
            | p :: ps' ->
                match p with
                | SimplePattern p -> flattenPattern (p :: flattened) ps'
                | CompoundPattern subPs -> 
                    let sub = flattenPattern [] subPs
                    flattenPattern (sub @ flattened) ps'
        
        let findFromSkipping searchPattern =
            let origPattern = flattenPattern [] searchPattern |> List.rev
            let rec findPattern (s : string) (skip : System.Collections.Generic.IDictionary<int,int>) startAt endAt dir predicates candidate i =
                let mutable e = 0 
                if skip.TryGetValue(i, &e) 
                then findPattern s skip startAt endAt dir predicates candidate e
                else 
                    match predicates with
                    | [] -> candidate, i
                    | predicate :: predicates' ->
                        if i = -1 || i < startAt || i > endAt
                        then -1, -1
                        else
                            let ch = s.[i]
                            match predicate ch with
                            | true ->
                                findPattern s skip startAt endAt dir 
                                    predicates' (if candidate = -1 then i else candidate) (i + dir)
                            | false -> 
                                findPattern s skip startAt endAt dir 
                                    origPattern -1 (if candidate = -1 then i + dir else candidate + dir) 
            fun (s : string) skip startAt endAt dir -> findPattern s skip startAt endAt dir origPattern -1 startAt

    let findFromSkipping searchPattern =
        let find' = Internal.findFromSkipping searchPattern 
        (fun skip s startAt -> find' s skip startAt (s.Length - 1) 1)

    

module Batches =
    let rec private findNextNewline (s : string) i stopAt =
        if i = stopAt 
        then -1
        else
            match s.[i] with
            | '\n' -> i + 1
            | '\r' -> 
                if i < stopAt - 1 && s.[i+1] = '\n' 
                then i + 2
                else i + 1
            | _ -> findNextNewline s (i + 1) stopAt
    
    // 1. Rewrite this as a one-pass solution looking for the beginning of the three blocks ... (see splitInSqlBatches)
    let collectSqlCommentsAndStrings (script : string) =
        let blocks = System.Collections.Generic.Dictionary<_,_>()
        let inline addBlock s e = blocks.Add (s, e)
        let sLen = script.Length

        let inline safeIsChar i c = i <= sLen &&  c = script.[i]

        let rec collect i =
            if i = sLen 
            then ()
            else 
                match script.[i] with
                // Line comments
                | '-' when safeIsChar (i + 1) '-' ->
                    match findNextNewline script (i + 2) sLen with
                    | -1 -> addBlock i sLen
                    | e -> addBlock i e; collect e
                // Block comments
                | '/' when safeIsChar (i + 1) '*' ->
                    match script.IndexOf("*/", i + 2) with
                    | -1 -> addBlock i sLen
                    | e -> addBlock i (e + 2); collect (e + 2)
                // Strings
                | '\'' ->
                    match script.IndexOf("'", i + 1) with
                    | -1 -> addBlock i script.Length
                    | e -> addBlock i (e + 1); collect (e + 1)
                | _ -> collect (i + 1)
        collect 0
        blocks

    // Split a script in batches on "GO"-statements ...

    let splitInSqlBatches (script : string) =
        let sLen = script.Length

        let inline safeIsChar i c = i < sLen &&  c = script.[i]
        let inline charIsWS c = c = '\r' || c = '\n' || c = '\t' || c = ' '

        let rec collect acc justSkipped from i =
            if i = sLen 
            then acc, from
            else 
                match script.[i] with
                // Find line comments
                | '-' when safeIsChar (i + 1) '-' ->
                    match findNextNewline script (i + 2) sLen with
                    | -1 -> acc, from
                    | e -> collect acc true from e
                // Find block comments
                | '/' when safeIsChar (i + 1) '*' ->
                    match script.IndexOf("*/", i + 2) with
                    | -1 -> acc, from
                    | e -> collect acc true from (e + 2)
                | '\'' ->
                // Find literal string
                    match script.IndexOf("'", i + 1) with
                    | -1 -> acc, from
                    | e -> collect acc true from (e + 1)
                // Find "GO"s ...
                | 'G' | 'g' when safeIsChar (i + 1) 'O' || safeIsChar (i + 1) 'o' ->
                    // Check before GO. Accept WS, begining of script or (skip)block end
                    if justSkipped || i = 0 || charIsWS script.[i - 1]
                    then
                        let i' = 
                            if i + 2 = sLen
                            then i + 2
                            else 
                                if charIsWS script.[i + 2] then i + 2
                                else if script.[i + 2] = ';' then i + 3
                                else -1
                        if i' <> -1
                        then 
                            match script.Substring(from, i - from) |> String.trim with
                            | "" -> collect acc false i' i'
                            | batch -> collect (batch :: acc) false i' i'
                        else collect acc false from i'
                    else collect acc false from (i + 1)
                | _ -> collect acc false from (i + 1)
        let (batches', last) = collect [] false 0 0
        let batches =
            match script.Substring(last) |> String.trim with
            | "" -> batches'
            | batch -> batch :: batches'
        batches |> List.rev



module SqlDefinitions =
    // The stored definition of triggers and procedures (and views) get out of sync if the objects are
    // renamed (with sp_rename). These parsers try to change the name to be up to date.

    open CodeSearch
(* Example:
CREATE TRIGGER tr_Brftgyhr
ON dbo.Message  AFTER INSERT 
AS BEGIN
*)
    let updatedTriggerDefinition name parentName definition =
        let skipThese = Batches.collectSqlCommentsAndStrings definition
        
        let (_ , i0) = findFromSkipping [isStr "CREATE"; isWS] skipThese definition 0
        let (_ , i1) = findFromSkipping [isWS; isStr "TRIGGER"; isWS] skipThese definition (i0 - 1)
        let (n0, i2) = findFromSkipping [isNotWS] skipThese definition i1
        let (n1, i3) = findFromSkipping [isWS] skipThese definition i2
        let (_ , i4) = findFromSkipping [isWS; isStr "ON"; isWS] skipThese definition (i3 - 1)
        let (p0, i5) = findFromSkipping [isNotWS] skipThese definition i4
        let (p1, _ ) = findFromSkipping [isWS] skipThese definition i5

        if p1 = -1
        then failwithf "Could not parse trigger definition for %s" name

        let newDefinition =
            definition.Substring(0, n0)
            + name
            + definition.Substring(n1, p0 - n1)
            + parentName
            + definition.Substring(p1)
        newDefinition

(* Example
CREATE PROCEDURE [dbo].[GetAllFoobars]
	@xyzId int
AS
BEGIN*)
    let updatedProcedureDefinition procName definingToken definition =
        let skip = Batches.collectSqlCommentsAndStrings definition
        
        let (_ , i0) = findFromSkipping [isStr "CREATE"; isWS] skip definition 0
        let (_ , i1) = findFromSkipping [isWS; isStr definingToken; isWS] skip definition (i0 - 1)
        let (n0, i2) = findFromSkipping [isNotWS] skip definition i1
        let (n1, _ ) = findFromSkipping [isIn (Set.add '(' WS)] skip definition i2
        
        if n1 = -1
        then failwithf "Could not parse procedure definition for %s" procName

        let newDefinition =
            definition.Substring(0, n0)
            + procName
            + definition.Substring(n1)
        newDefinition
        
    
    
        