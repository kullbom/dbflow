namespace DbFlow.SqlServer.Experimental.ShowPlanXml

type QueryPlanIssue =
    | QPWarning of Warning
    | QPMissingIndexes of MissingIndexesType
    | QPScanWithoutSeekPredicate of RelOpType

module Abstractions =

    let foldQueryPlans f seed (p : Plan) =
        p.BatchSequence.Batches
        |> List.fold 
            (fun acc b ->
                b.Statements
                |> List.fold 
                    (fun acc' s -> 
                        match s with
                        | StmtSimple stmtSimple -> 
                            stmtSimple.QueryPlan
                            |> Option.fold (fun a0 qp -> f a0 qp) acc'
                        | StmtCond stmtCond -> 
                            stmtCond.Condition.QueryPlan
                            |> Option.fold (fun a0 qp -> f a0 qp) acc'
                        | StmtCursor stmtCursor -> 
                            stmtCursor.Operations
                            |> List.fold (fun a0 op -> f a0 op.QueryPlan) acc'
                        | StmtReceive stmtReceive -> 
                            stmtReceive.ReceivePlans
                            |> List.fold (fun a0 rp -> f a0 rp.QueryPlan) acc'
                        | StmtUseDb _stmtUseDb -> acc'
                        | ExternalDistributedComputation _distributedComputation -> acc')
                    acc)
            seed
    
    /// Folds a function over a single RelOpType, visiting all child RelOps recursively
    let rec foldRelOp' (f : 'a -> bool -> RelOpType -> 'a) hasTopParent (seed : 'a) (relOp : RelOpType) : 'a =
        // First apply f to this RelOp
        let acc = f seed hasTopParent relOp
    
        // Then recursively fold over child RelOps based on operator type
        match relOp.OperatorDetails with
        | None -> acc
        | Some detail ->
            match detail with
            // Operators with single required RelOp child
            | RelOpDetails.MergeInterval op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Sort op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Bitmap op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Collapse op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Segment op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Split op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Top op -> foldRelOp' f true acc op.RelOp
            | RelOpDetails.StreamAggregate op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Parallelism op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Filter op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.ComputeScalar op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Assert op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.Update op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.ForeignKeyReferencesCheck op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.RemoteFetch op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.RemoteModify op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.XcsScan op -> foldRelOp' f hasTopParent acc op.RelOp
            | RelOpDetails.TopSort op -> foldRelOp' f true acc op.SortBase.RelOp
    
            // Operators with optional RelOp children
            | RelOpDetails.Extension op -> 
                match op.RelOp with
                | Some relOp -> foldRelOp' f hasTopParent acc relOp
                | None -> acc
            | RelOpDetails.WindowSpool op ->
                match op.RelOp with
                | Some relOp -> foldRelOp' f hasTopParent acc relOp
                | None -> acc
            | RelOpDetails.WindowAggregate op ->
                match op.RelOp with
                | Some relOp -> foldRelOp' f hasTopParent acc relOp
                | None -> acc
            | RelOpDetails.TableValuedFunction op ->
                match op.RelOp with
                | Some relOp -> foldRelOp' f hasTopParent acc relOp
                | None -> acc
            | RelOpDetails.Put op ->
                match op.RelOp with
                | Some relOp -> foldRelOp' f hasTopParent acc relOp
                | None -> acc
    
            // Operators with list of RelOp children
            | RelOpDetails.Concat op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Union op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.UnionAll op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Sequence op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Merge op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.NestedLoops op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Hash op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Delete op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Insert op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Apply op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Join op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.GbAgg op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.GbApply op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.LocalCube op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Generic op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Project op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Move op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.ExternalSelect op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Get op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.AdaptiveJoin op -> List.fold (foldRelOp' f hasTopParent) acc op.RelOps
            | RelOpDetails.Switch op -> List.fold (foldRelOp' f hasTopParent) acc op.ConcatBase.RelOps
    
            // Leaf operators (no child RelOps)
            | RelOpDetails.ConstantScan _ -> acc
            | RelOpDetails.TableScan _ -> acc
            | RelOpDetails.IndexScan _ -> acc
            | RelOpDetails.RemoteScan _ -> acc
            | RelOpDetails.RowCountSpool _ -> acc
            | RelOpDetails.Spool _ -> acc
            | RelOpDetails.BatchHashTableBuild _ -> acc
            | RelOpDetails.CreateIndex _ -> acc
            | RelOpDetails.OnlineIndex _ -> acc
            | RelOpDetails.RemoteQuery _ -> acc
            | RelOpDetails.RemoteRange _ -> acc
            | RelOpDetails.SequenceProject _ -> acc
            | RelOpDetails.ExtExtractScan _ -> acc
            | RelOpDetails.DeletedScan _ -> acc
            | RelOpDetails.InsertedScan _ -> acc
            | RelOpDetails.ConstTableGet _ -> acc
            | RelOpDetails.SimpleUpdate _ -> acc
            | RelOpDetails.ScalarInsert _ -> acc
            | RelOpDetails.ParameterTableScan _ -> acc
            | RelOpDetails.PrintDataflow _ -> acc
            | RelOpDetails.LogRowScan _ -> acc
    
    
    let foldRelOp (f : 'a -> bool -> RelOpType -> 'a) (seed : 'a) (relOp : RelOpType) : 'a =
        foldRelOp' f false seed relOp

    /// Checks if a RelOp truly represents a full table scan
    /// A full table scan means all rows must be read because:
    /// 1. TableScan or RemoteScan always read entire table
    /// 2. Index/Clustered Index Scan without seek predicates that:
    ///    - Have no parent Top operator (must read all to get results)
    let isFullTableScan (hasTopParent : bool) (relOp : RelOpType) : bool =
        match relOp.PhysicalOp with
        | PhysicalOpType.TableScan -> true
        | PhysicalOpType.RemoteScan -> true
        | PhysicalOpType.IndexScan 
        | PhysicalOpType.ClusteredIndexScan -> 
            match relOp.OperatorDetails with
            | Some (RelOpDetails.IndexScan indexScan) -> 
                // If there are seek predicates, it's NOT a full scan
                match indexScan.SeekPredicates with
                | Some _ -> false
                | None -> 
                    // No seek predicates. It's a full scan UNLESS it can early-exit due to ordering+Top
                    not (hasTopParent && indexScan.Ordered)
            | Some (RelOpDetails.TableScan tableScan) -> 
                // Table scans without seek predicates are full scans unless they can early-exit
                not (hasTopParent && tableScan.Ordered)
            | _ -> false

        // TODO: Handle Deletes, Updates etc. that can also be full scans if they have no predicates and no early exit
        | _ -> false


    let issueString(qp : QueryPlanType) (qpi : QueryPlanIssue) =
        match qpi with
        | QPWarning w -> 
            match w with
            | SpillOccurred spillOccurred -> $"Spill Occurred" 
            | ColumnsWithNoStatistics columns  -> "ColumnsWithNoStatistics"
            | ColumnsWithStaleStatistics columns  -> "ColumnsWithStaleStatistics"
            | SpillToTempDb spillToTempDb -> "SpillToTempDb"
            | Wait waitWarning -> "Wait"
            | PlanAffectingConvert affectingConvertWarning -> "PlanAffectingConvert"
            | SortSpillDetails sortSpillDetails -> "SortSpill"
            | HashSpillDetails hashSpillDetails -> "HashSpill"
            | ExchangeSpillDetails exchangeSpillDetails -> "ExchangeSpill"
            | MemoryGrantWarning memoryGrantWarning -> "MemoryGrant"
        | QPMissingIndexes mi -> "MissingIndexes"
        | QPScanWithoutSeekPredicate _ -> "ScanWithoutSeekPredicate"
    
    let foldQueryPlanIssues f seed (p : Plan) =
        p
        |> foldQueryPlans 
            (fun acc qp -> 
                let a0 = 
                    qp.Warnings 
                    |> Option.fold 
                        (fun acc' ws -> 
                            ws.Warnings
                            |> List.fold (fun acc'' w -> f acc'' qp (QPWarning w)) acc')
                        acc
                let a1 =
                    qp.MissingIndexes
                    |> Option.fold (fun acc' mis -> f acc' qp (QPMissingIndexes mis)) a0
                qp.RelOp
                |> foldRelOp
                    (fun acc' hasTopParent ro ->
                        if isFullTableScan hasTopParent ro
                        then f acc' qp (QPScanWithoutSeekPredicate ro)
                        else acc')
                    a1)
            seed