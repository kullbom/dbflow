module DbFlow.SqlParser


type SearchPattern =
    | SimplePattern of (char -> bool)
    | CompoundPattern of SearchPattern list

module CodeSearch =
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
        
        let find searchPattern =
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

    let find searchPattern =
        let find' = Internal.find searchPattern
        (fun s -> find' s Map.empty 0 (s.Length - 1) 1)
    
    let findFrom searchPattern =
        let find' = Internal.find searchPattern 
        (fun s startAt -> find' s Map.empty startAt (s.Length - 1) 1)

    let findFromSkipping searchPattern =
        let find' = Internal.find searchPattern 
        (fun skip s startAt -> find' s skip startAt (s.Length - 1) 1)
    

module Batches =
    open CodeSearch

    let rec private findNewline (s : string) i stopAt =
        if i = stopAt 
        then -1
        else
            match s.[i] with
            | '\n' -> i + 1
            | '\r' -> 
                if i < stopAt - 1 && s.[i+1] = '\n' 
                then i + 2
                else i + 1
            | _ -> findNewline s (i + 1) stopAt

    // 1. Rewrite this as a one-pass solution looking for the beginning of the three blocks ...
    let collectSqlCommentsAndStrings (script : string) =
        let blocks = System.Collections.Generic.Dictionary<_,_>()
        let endings = System.Collections.Generic.HashSet<_>()
        let inline addBlock s e =
            blocks.Add (s, e)
            endings.Add e |> ignore
        let sLen = script.Length

        let inline safeIsChar i c = i <= sLen &&  c = script.[i]

        let rec collect i =
            if i = sLen 
            then ()
            else 
                match script.[i] with
                | '-' when safeIsChar (i + 1) '-' ->
                    match findNewline script (i + 2) sLen with
                    | -1 -> addBlock i sLen
                    | e -> addBlock i e; collect e
                | '/' when safeIsChar (i + 1) '*' ->
                    match script.IndexOf("*/", i + 2) with
                    | -1 -> addBlock i sLen
                    | e -> addBlock i (e + 2); collect (e + 2)
                | '\'' ->
                    match script.IndexOf("'", i + 1) with
                    | -1 -> addBlock i script.Length
                    | e -> addBlock i (e + 1); collect (e + 1)
                | _ -> collect (i + 1)
        collect 0
        blocks, endings

        // Split script in batches on "GO"-statements...

    let inline private go_at (s : string) i =
        (s.[i] = 'G' || s.[i] = 'g') && (s.[i + 1] = 'O' || s.[i + 1] = 'o')
    
    let private leadingGO (s : string) =
        if go_at s 0 
        then 
            if s.[2] = ';' && Set.contains s.[3] WS
            then 4
            else if Set.contains s.[2] WS
            then 3
            else -1
        else -1

    let private endingGO (s : string) = 
        let i = s.Length - 1
        if go_at s (i-2) && s.[i] = ';' 
        then i-2
        else if go_at s (i-1)  
        then i-1
        else -1 


    let splitInSqlBatches (script' : string) =
        let script =
            let i0 = match leadingGO script' with -1 -> 0 | x -> x 
            let i1 = match endingGO script' with -1 -> script'.Length | x -> x
            script'.Substring (i0, i1 - i0)
        
        let sLen = script.Length

        let inline safeIsChar i c = i < sLen &&  c = script.[i]
        let inline charIsWS c = c = '\r' || c = '\n' || c = '\t' || c = ' '

        let rec collect acc justSkipped from i =
            if i = sLen 
            then acc, from
            else 
                match script.[i] with
                | '-' when safeIsChar (i + 1) '-' ->
                    match findNewline script (i + 2) sLen with
                    | -1 -> acc, from
                    | e -> collect acc true from e
                | '/' when safeIsChar (i + 1) '*' ->
                    match script.IndexOf("*/", i + 2) with
                    | -1 -> acc, from
                    | e -> collect acc true from (e + 2)
                | '\'' ->
                    match script.IndexOf("'", i + 1) with
                    | -1 -> acc, from
                    | e -> collect acc true from (e + 1)
                | 'G' | 'g' when safeIsChar (i + 1) 'O' || safeIsChar (i + 1) 'o' ->
                    // Check before GO. Accept WS or (skip)block end
                    if justSkipped || charIsWS script.[i - 1]
                    then
                        let i' = 
                            if charIsWS script.[i + 2] then i + 2
                            else if script.[i + 2] = ';' then i + 3
                            else -1
                        if i' <> -1
                        then 
                            match script.Substring(from, i - from).Trim() with
                            | "" -> collect acc false i' i'
                            | batch -> collect (batch :: acc) false i' i'
                        else collect acc false from i'
                    else collect acc false from (i + 1)
                | _ -> collect acc false from (i + 1)
        let (batches', last) = collect [] false 0 0
        let batches =
            match script.Substring(last).Trim() with
            | "" -> batches'
            | batch -> batch :: batches'
        batches |> List.rev



module SqlDefinitions =

    open CodeSearch
(* Example:
CREATE TRIGGER trg_NrdbCaseToProcess
ON portability.NrdbMessage  AFTER INSERT 
AS BEGIN
*)
    let updateTriggerDefinition name parentName definition =
        let (skip, _) = Batches.collectSqlCommentsAndStrings definition
        
        let (_ , i0) = findFromSkipping [isStr "CREATE"; isWS] skip definition 0
        let (_ , i1) = findFromSkipping [isWS; isStr "TRIGGER"; isWS] skip definition (i0 - 1)
        let (n0, i2) = findFromSkipping [isNotWS] skip definition i1
        let (n1, i3) = findFromSkipping [isWS] skip definition i2
        let (_ , i4) = findFromSkipping [isWS; isStr "ON"; isWS] skip definition (i3 - 1)
        let (p0, i5) = findFromSkipping [isNotWS] skip definition i4
        let (p1, _ ) = findFromSkipping [isWS] skip definition i5

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
CREATE PROCEDURE [dbo].[GetAllRoles]
	@roleId int
AS
BEGIN*)
    let updateProcedureDefinition procName definingToken definition =
        let (skip, _) = Batches.collectSqlCommentsAndStrings definition
        
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
        
    
    
        