module DbFlow.SqlServer.ShowPlanXml.Parsers.Primitives

open DbFlow.XmlParser
open DbFlow.SqlServer.ShowPlanXml

let parseDateTime (dateTimeStr : string) : PResult<System.DateTime, _> =
    match System.DateTime.TryParse dateTimeStr with
    | true, dt -> POk dt
    | false, _ -> Failf "Invalid DateTime format: '%s'" dateTimeStr

// ========================================
// Enum Parsers - no mutual dependencies
// ========================================

let parseIndexKind (s : string) : PResult<IndexKindType, _> =
    match s with
    | "Heap" -> POk IndexKindType.Heap
    | "Clustered" -> POk IndexKindType.Clustered
    | "FTSChangeTracking" -> POk IndexKindType.FTSChangeTracking
    | "FTSMapping" -> POk IndexKindType.FTSMapping
    | "NonClustered" -> POk IndexKindType.NonClustered
    | "PrimaryXML" -> POk IndexKindType.PrimaryXML
    | "SecondaryXML" -> POk IndexKindType.SecondaryXML
    | "Spatial" -> POk IndexKindType.Spatial
    | "ViewClustered" -> POk IndexKindType.ViewClustered
    | "ViewNonClustered" -> POk IndexKindType.ViewNonClustered
    | "NonClusteredHash" -> POk IndexKindType.NonClusteredHash
    | "SelectiveXML" -> POk IndexKindType.SelectiveXML
    | "SecondarySelectiveXML" -> POk IndexKindType.SecondarySelectiveXML
    | other -> Failf "Unknown IndexKindType: '%s'" other

let parseCloneAccessScope (s : string) : PResult<CloneAccessScopeType, _> =
    match s with
    | "Primary" -> POk CloneAccessScopeType.Primary
    | "Secondary" -> POk CloneAccessScopeType.Secondary
    | "Both" -> POk CloneAccessScopeType.Both
    | "Either" -> POk CloneAccessScopeType.Either
    | "ExactMatch" -> POk CloneAccessScopeType.ExactMatch
    | "Local" -> POk CloneAccessScopeType.Local
    | other -> Failf "Unknown CloneAccessScopeType: '%s'" other

let parseStorage (s : string) : PResult<StorageType, _> =
    match s with
    | "RowStore" -> POk StorageType.RowStore
    | "ColumnStore" -> POk StorageType.ColumnStore
    | "MemoryOptimized" -> POk StorageType.MemoryOptimized
    | other -> Failf "Unknown StorageType: '%s'" other

let parseArithmeticOperation (s : string) : PResult<ArithmeticOperationType, _> =
    match s with
    | "ADD" -> POk ArithmeticOperationType.ADD
    | "BIT_ADD" -> POk ArithmeticOperationType.BIT_ADD
    | "BIT_AND" -> POk ArithmeticOperationType.BIT_AND
    | "BIT_COMBINE" -> POk ArithmeticOperationType.BIT_COMBINE
    | "BIT_NOT" -> POk ArithmeticOperationType.BIT_NOT
    | "BIT_OR" -> POk ArithmeticOperationType.BIT_OR
    | "BIT_XOR" -> POk ArithmeticOperationType.BIT_XOR
    | "DIV" -> POk ArithmeticOperationType.DIV
    | "HASH" -> POk ArithmeticOperationType.HASH
    | "MINUS" -> POk ArithmeticOperationType.MINUS
    | "MOD" -> POk ArithmeticOperationType.MOD
    | "MULT" -> POk ArithmeticOperationType.MULT
    | "SUB" -> POk ArithmeticOperationType.SUB
    | "CONCAT" -> POk ArithmeticOperationType.CONCAT
    | other -> Failf "Unknown ArithmeticOperationType: '%s'" other

let parseLogicalOperation (s : string) : PResult<LogicalOperationType, _> =
    match s with
    | "AND" -> POk LogicalOperationType.AND
    | "IMPLIES" -> POk LogicalOperationType.IMPLIES
    | "IS NOT NULL" -> POk LogicalOperationType.IS_NOT_NULL
    | "IS NULL" -> POk LogicalOperationType.IS_NULL
    | "IS" -> POk LogicalOperationType.IS
    | "IsFalseOrNull" -> POk LogicalOperationType.IsFalseOrNull
    | "NOT" -> POk LogicalOperationType.NOT
    | "OR" -> POk LogicalOperationType.OR
    | "XOR" -> POk LogicalOperationType.XOR
    | other -> Failf "Unknown LogicalOperationType: '%s'" other

let parseSubqueryOperation (s : string) : PResult<SubqueryOperationType, _> =
    match s with
    | "EQ ALL" -> POk SubqueryOperationType.EQ_ALL
    | "EQ ANY" -> POk SubqueryOperationType.EQ_ANY
    | "EXISTS" -> POk SubqueryOperationType.EXISTS
    | "GE ALL" -> POk SubqueryOperationType.GE_ALL
    | "GE ANY" -> POk SubqueryOperationType.GE_ANY
    | "GT ALL" -> POk SubqueryOperationType.GT_ALL
    | "GT ANY" -> POk SubqueryOperationType.GT_ANY
    | "IN" -> POk SubqueryOperationType.IN
    | "LE ALL" -> POk SubqueryOperationType.LE_ALL
    | "LE ANY" -> POk SubqueryOperationType.LE_ANY
    | "LT ALL" -> POk SubqueryOperationType.LT_ALL
    | "LT ANY" -> POk SubqueryOperationType.LT_ANY
    | "NE ALL" -> POk SubqueryOperationType.NE_ALL
    | "NE ANY" -> POk SubqueryOperationType.NE_ANY
    | other -> Failf "Unknown SubqueryOperationType: '%s'" other

let parseCompareOp (s : string) : PResult<CompareOpType, _> =
    match s with
    | "BINARY IS" -> POk CompareOpType.BINARY_IS
    | "BOTH NULL" -> POk CompareOpType.BOTH_NULL
    | "EQ" -> POk CompareOpType.EQ
    | "GE" -> POk CompareOpType.GE
    | "GT" -> POk CompareOpType.GT
    | "IS" -> POk CompareOpType.IS
    | "IS NOT" -> POk CompareOpType.IS_NOT
    | "IS NOT NULL" -> POk CompareOpType.IS_NOT_NULL
    | "IS NULL" -> POk CompareOpType.IS_NULL
    | "LE" -> POk CompareOpType.LE
    | "LT" -> POk CompareOpType.LT
    | "NE" -> POk CompareOpType.NE
    | "ONE NULL" -> POk CompareOpType.ONE_NULL
    | other -> Failf "Unknown CompareOpType: '%s'" other

let parseLogicalOp (s : string) : PResult<LogicalOpType, _> =
    match s with
    | "Aggregate" -> POk LogicalOpType.Aggregate
    | "Anti Diff" -> POk LogicalOpType.AntiDiff
    | "Assert" -> POk LogicalOpType.Assert
    | "Async Concat" -> POk LogicalOpType.AsyncConcat
    | "Batch Hash Table Build" -> POk LogicalOpType.BatchHashTableBuild
    | "Bitmap Create" -> POk LogicalOpType.BitmapCreate
    | "Clustered Index Scan" -> POk LogicalOpType.ClusteredIndexScan
    | "Clustered Index Seek" -> POk LogicalOpType.ClusteredIndexSeek
    | "Clustered Update" -> POk LogicalOpType.ClusteredUpdate
    | "Collapse" -> POk LogicalOpType.Collapse
    | "Compute Scalar" -> POk LogicalOpType.ComputeScalar
    | "Concatenation" -> POk LogicalOpType.Concatenation
    | "Constant Scan" -> POk LogicalOpType.ConstantScan
    | "Constant Table Get" -> POk LogicalOpType.ConstantTableGet
    | "Cross Join" -> POk LogicalOpType.CrossJoin
    | "Delete" -> POk LogicalOpType.Delete
    | "Deleted Scan" -> POk LogicalOpType.DeletedScan
    | "Distinct Sort" -> POk LogicalOpType.DistinctSort
    | "Distinct" -> POk LogicalOpType.Distinct
    | "Distribute Streams" -> POk LogicalOpType.DistributeStreams
    | "Eager Spool" -> POk LogicalOpType.EagerSpool
    | "External Extraction Scan" -> POk LogicalOpType.ExternalExtractionScan
    | "External Select" -> POk LogicalOpType.ExternalSelect
    | "Filter" -> POk LogicalOpType.Filter
    | "Flow Distinct" -> POk LogicalOpType.FlowDistinct
    | "Foreign Key References Check" -> POk LogicalOpType.ForeignKeyReferencesCheck
    | "Full Outer Join" -> POk LogicalOpType.FullOuterJoin
    | "Gather Streams" -> POk LogicalOpType.GatherStreams
    | "GbAgg" -> POk LogicalOpType.GbAgg
    | "GbApply" -> POk LogicalOpType.GbApply
    | "Get" -> POk LogicalOpType.Get
    | "Generic" -> POk LogicalOpType.Generic
    | "Inner Apply" -> POk LogicalOpType.InnerApply
    | "Index Scan" -> POk LogicalOpType.IndexScan
    | "Index Seek" -> POk LogicalOpType.IndexSeek
    | "Inner Join" -> POk LogicalOpType.InnerJoin
    | "Insert" -> POk LogicalOpType.Insert
    | "Inserted Scan" -> POk LogicalOpType.InsertedScan
    | "Intersect" -> POk LogicalOpType.Intersect
    | "Intersect All" -> POk LogicalOpType.IntersectAll
    | "Lazy Spool" -> POk LogicalOpType.LazySpool
    | "Left Anti Semi Apply" -> POk LogicalOpType.LeftAntiSemiApply
    | "Left Semi Apply" -> POk LogicalOpType.LeftSemiApply
    | "Left Outer Apply" -> POk LogicalOpType.LeftOuterApply
    | "Left Anti Semi Join" -> POk LogicalOpType.LeftAntiSemiJoin
    | "Left Diff" -> POk LogicalOpType.LeftDiff
    | "Left Diff All" -> POk LogicalOpType.LeftDiffAll
    | "Left Outer Join" -> POk LogicalOpType.LeftOuterJoin
    | "Left Semi Join" -> POk LogicalOpType.LeftSemiJoin
    | "LocalCube" -> POk LogicalOpType.LocalCube
    | "Log Row Scan" -> POk LogicalOpType.LogRowScan
    | "Merge" -> POk LogicalOpType.Merge
    | "Merge Interval" -> POk LogicalOpType.MergeInterval
    | "Merge Stats" -> POk LogicalOpType.MergeStats
    | "Move" -> POk LogicalOpType.Move
    | "Parameter Table Scan" -> POk LogicalOpType.ParameterTableScan
    | "Partial Aggregate" -> POk LogicalOpType.PartialAggregate
    | "Print" -> POk LogicalOpType.Print
    | "Project" -> POk LogicalOpType.Project
    | "Put" -> POk LogicalOpType.Put
    | "Rank" -> POk LogicalOpType.Rank
    | "Remote Delete" -> POk LogicalOpType.RemoteDelete
    | "Remote Index Scan" -> POk LogicalOpType.RemoteIndexScan
    | "Remote Index Seek" -> POk LogicalOpType.RemoteIndexSeek
    | "Remote Insert" -> POk LogicalOpType.RemoteInsert
    | "Remote Query" -> POk LogicalOpType.RemoteQuery
    | "Remote Scan" -> POk LogicalOpType.RemoteScan
    | "Remote Update" -> POk LogicalOpType.RemoteUpdate
    | "Repartition Streams" -> POk LogicalOpType.RepartitionStreams
    | "RID Lookup" -> POk LogicalOpType.RIDLookup
    | "Right Anti Semi Join" -> POk LogicalOpType.RightAntiSemiJoin
    | "Right Diff" -> POk LogicalOpType.RightDiff
    | "Right Diff All" -> POk LogicalOpType.RightDiffAll
    | "Right Outer Join" -> POk LogicalOpType.RightOuterJoin
    | "Right Semi Join" -> POk LogicalOpType.RightSemiJoin
    | "Segment" -> POk LogicalOpType.Segment
    | "Sequence" -> POk LogicalOpType.Sequence
    | "Sort" -> POk LogicalOpType.Sort
    | "Split" -> POk LogicalOpType.Split
    | "Switch" -> POk LogicalOpType.Switch
    | "Table-valued function" -> POk LogicalOpType.TableValuedFunction
    | "Table Scan" -> POk LogicalOpType.TableScan
    | "Top" -> POk LogicalOpType.Top
    | "TopN Sort" -> POk LogicalOpType.TopNSort
    | "UDX" -> POk LogicalOpType.UDX
    | "Union" -> POk LogicalOpType.Union
    | "Union All" -> POk LogicalOpType.UnionAll
    | "Update" -> POk LogicalOpType.Update
    | "Local Stats" -> POk LogicalOpType.LocalStats
    | "Window Spool" -> POk LogicalOpType.WindowSpool
    | "Window Aggregate" -> POk LogicalOpType.WindowAggregate
    | "Key Lookup" -> POk LogicalOpType.KeyLookup
    | "Extensible Column Store Scan" -> POk LogicalOpType.ExtensibleColumnStoreScan
    | other -> Failf "Unknown LogicalOp type: '%s'" other
    
let parsePhysicalOp (s : string) : PResult<PhysicalOpType, _> =
    match s with
    | "Adaptive Join" -> POk PhysicalOpType.AdaptiveJoin
    | "Apply" -> POk PhysicalOpType.Apply
    | "Assert" -> POk PhysicalOpType.Assert
    | "Batch Hash Table Build" -> POk PhysicalOpType.BatchHashTableBuild
    | "Bitmap" -> POk PhysicalOpType.Bitmap
    | "Broadcast" -> POk PhysicalOpType.Broadcast
    | "Clustered Index Delete" -> POk PhysicalOpType.ClusteredIndexDelete
    | "Clustered Index Insert" -> POk PhysicalOpType.ClusteredIndexInsert
    | "Clustered Index Scan" -> POk PhysicalOpType.ClusteredIndexScan
    | "Clustered Index Seek" -> POk PhysicalOpType.ClusteredIndexSeek
    | "Clustered Index Update" -> POk PhysicalOpType.ClusteredIndexUpdate
    | "Clustered Index Merge" -> POk PhysicalOpType.ClusteredIndexMerge
    | "Clustered Update" -> POk PhysicalOpType.ClusteredUpdate
    | "Collapse" -> POk PhysicalOpType.Collapse
    | "Columnstore Index Delete" -> POk PhysicalOpType.ColumnstoreIndexDelete
    | "Columnstore Index Insert" -> POk PhysicalOpType.ColumnstoreIndexInsert
    | "Columnstore Index Merge" -> POk PhysicalOpType.ColumnstoreIndexMerge
    | "Columnstore Index Scan" -> POk PhysicalOpType.ColumnstoreIndexScan
    | "Columnstore Index Update" -> POk PhysicalOpType.ColumnstoreIndexUpdate
    | "Compute Scalar" -> POk PhysicalOpType.ComputeScalar
    | "Compute To Control Node" -> POk PhysicalOpType.ComputeToControlNode
    | "Concatenation" -> POk PhysicalOpType.Concatenation
    | "Constant Scan" -> POk PhysicalOpType.ConstantScan
    | "Constant Table Get" -> POk PhysicalOpType.ConstantTableGet
    | "Control To Compute Nodes" -> POk PhysicalOpType.ControlToComputeNodes
    | "Delete" -> POk PhysicalOpType.Delete
    | "Deleted Scan" -> POk PhysicalOpType.DeletedScan
    | "External Broadcast" -> POk PhysicalOpType.ExternalBroadcast
    | "External Extraction Scan" -> POk PhysicalOpType.ExternalExtractionScan
    | "External Local Streaming" -> POk PhysicalOpType.ExternalLocalStreaming
    | "External Round Robin" -> POk PhysicalOpType.ExternalRoundRobin
    | "External Select" -> POk PhysicalOpType.ExternalSelect
    | "External Shuffle" -> POk PhysicalOpType.ExternalShuffle
    | "Filter" -> POk PhysicalOpType.Filter
    | "Foreign Key References Check" -> POk PhysicalOpType.ForeignKeyReferencesCheck
    | "GbAgg" -> POk PhysicalOpType.GbAgg
    | "GbApply" -> POk PhysicalOpType.GbApply
    | "Get" -> POk PhysicalOpType.Get
    | "Generic" -> POk PhysicalOpType.Generic
    | "Hash Match" -> POk PhysicalOpType.HashMatch
    | "Index Delete" -> POk PhysicalOpType.IndexDelete
    | "Index Insert" -> POk PhysicalOpType.IndexInsert
    | "Index Scan" -> POk PhysicalOpType.IndexScan
    | "Insert" -> POk PhysicalOpType.Insert
    | "Join" -> POk PhysicalOpType.Join
    | "Index Seek" -> POk PhysicalOpType.IndexSeek
    | "Index Spool" -> POk PhysicalOpType.IndexSpool
    | "Index Update" -> POk PhysicalOpType.IndexUpdate
    | "Inserted Scan" -> POk PhysicalOpType.InsertedScan
    | "Local Cube" -> POk PhysicalOpType.LocalCube
    | "Log Row Scan" -> POk PhysicalOpType.LogRowScan
    | "Merge Interval" -> POk PhysicalOpType.MergeInterval
    | "Merge Join" -> POk PhysicalOpType.MergeJoin
    | "Nested Loops" -> POk PhysicalOpType.NestedLoops
    | "Online Index Insert" -> POk PhysicalOpType.OnlineIndexInsert
    | "Parallelism" -> POk PhysicalOpType.Parallelism
    | "Parameter Table Scan" -> POk PhysicalOpType.ParameterTableScan
    | "Print" -> POk PhysicalOpType.Print
    | "Project" -> POk PhysicalOpType.Project
    | "Put" -> POk PhysicalOpType.Put
    | "Rank" -> POk PhysicalOpType.Rank
    | "Remote Delete" -> POk PhysicalOpType.RemoteDelete
    | "Remote Index Scan" -> POk PhysicalOpType.RemoteIndexScan
    | "Remote Index Seek" -> POk PhysicalOpType.RemoteIndexSeek
    | "Remote Insert" -> POk PhysicalOpType.RemoteInsert
    | "Remote Query" -> POk PhysicalOpType.RemoteQuery
    | "Remote Scan" -> POk PhysicalOpType.RemoteScan
    | "Remote Update" -> POk PhysicalOpType.RemoteUpdate
    | "RID Lookup" -> POk PhysicalOpType.RIDLookup
    | "Row Count Spool" -> POk PhysicalOpType.RowCountSpool
    | "Segment" -> POk PhysicalOpType.Segment
    | "Sequence" -> POk PhysicalOpType.Sequence
    | "Sequence Project" -> POk PhysicalOpType.SequenceProject
    | "Shuffle" -> POk PhysicalOpType.Shuffle
    | "Single Source Round Robin Move" -> POk PhysicalOpType.SingleSourceRoundRobinMove
    | "Sort" -> POk PhysicalOpType.Sort
    | "Split" -> POk PhysicalOpType.Split
    | "Stream Aggregate" -> POk PhysicalOpType.StreamAggregate
    | "Switch" -> POk PhysicalOpType.Switch
    | "Table Delete" -> POk PhysicalOpType.TableDelete
    | "Table Insert" -> POk PhysicalOpType.TableInsert
    | "Table Merge" -> POk PhysicalOpType.TableMerge
    | "Table Scan" -> POk PhysicalOpType.TableScan
    | "Table Spool" -> POk PhysicalOpType.TableSpool
    | "Table Update" -> POk PhysicalOpType.TableUpdate
    | "Table-valued function" -> POk PhysicalOpType.TableValuedFunction
    | "Top" -> POk PhysicalOpType.Top
    | "Trim" -> POk PhysicalOpType.Trim
    | "UDX" -> POk PhysicalOpType.UDX
    | "Union" -> POk PhysicalOpType.Union
    | "Union All" -> POk PhysicalOpType.UnionAll
    | "Window Aggregate" -> POk PhysicalOpType.WindowAggregate
    | "Window Spool" -> POk PhysicalOpType.WindowSpool
    | "Key Lookup" -> POk PhysicalOpType.KeyLookup
    | "Extensible Column Store Scan" -> POk PhysicalOpType.ExtensibleColumnStoreScan
    | other -> Failf "Unknown PhysicalOp type: '%s'" other

let parseExecutionMode (s : string) : PResult<ExecutionModeType, _> =
    match s with
    | "Row" -> POk ExecutionModeType.Row
    | "Batch" -> POk ExecutionModeType.Batch
    | other -> Failf "Unknown ExecutionModeType type: '%s'" other


