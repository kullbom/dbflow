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
    | "AntiDiff" -> POk LogicalOpType.AntiDiff
    | "Assert" -> POk LogicalOpType.Assert
    | "AsyncConcat" -> POk LogicalOpType.AsyncConcat
    | "BatchHashTableBuild" -> POk LogicalOpType.BatchHashTableBuild
    | "BitmapCreate" -> POk LogicalOpType.BitmapCreate
    | "ClusteredIndexScan" -> POk LogicalOpType.ClusteredIndexScan
    | "ClusteredIndexSeek" -> POk LogicalOpType.ClusteredIndexSeek
    | "ClusteredUpdate" -> POk LogicalOpType.ClusteredUpdate
    | "Collapse" -> POk LogicalOpType.Collapse
    | "ComputeScalar" -> POk LogicalOpType.ComputeScalar
    | "Concatenation" -> POk LogicalOpType.Concatenation
    | "ConstantScan" -> POk LogicalOpType.ConstantScan
    | "ConstantTableGet" -> POk LogicalOpType.ConstantTableGet
    | "CrossJoin" -> POk LogicalOpType.CrossJoin
    | "Delete" -> POk LogicalOpType.Delete
    | "DeletedScan" -> POk LogicalOpType.DeletedScan
    | "DistinctSort" -> POk LogicalOpType.DistinctSort
    | "Distinct" -> POk LogicalOpType.Distinct
    | "DistributeStreams" -> POk LogicalOpType.DistributeStreams
    | "EagerSpool" -> POk LogicalOpType.EagerSpool
    | "ExternalExtractionScan" -> POk LogicalOpType.ExternalExtractionScan
    | "ExternalSelect" -> POk LogicalOpType.ExternalSelect
    | "Filter" -> POk LogicalOpType.Filter
    | "FlowDistinct" -> POk LogicalOpType.FlowDistinct
    | "ForeignKeyReferencesCheck" -> POk LogicalOpType.ForeignKeyReferencesCheck
    | "FullOuterJoin" -> POk LogicalOpType.FullOuterJoin
    | "GatherStreams" -> POk LogicalOpType.GatherStreams
    | "GbAgg" -> POk LogicalOpType.GbAgg
    | "GbApply" -> POk LogicalOpType.GbApply
    | "Get" -> POk LogicalOpType.Get
    | "Generic" -> POk LogicalOpType.Generic
    | "InnerApply" -> POk LogicalOpType.InnerApply
    | "IndexScan" -> POk LogicalOpType.IndexScan
    | "IndexSeek" -> POk LogicalOpType.IndexSeek
    | "InnerJoin" -> POk LogicalOpType.InnerJoin
    | "Insert" -> POk LogicalOpType.Insert
    | "InsertedScan" -> POk LogicalOpType.InsertedScan
    | "Intersect" -> POk LogicalOpType.Intersect
    | "IntersectAll" -> POk LogicalOpType.IntersectAll
    | "LazySpool" -> POk LogicalOpType.LazySpool
    | "LeftAntiSemiApply" -> POk LogicalOpType.LeftAntiSemiApply
    | "LeftSemiApply" -> POk LogicalOpType.LeftSemiApply
    | "LeftOuterApply" -> POk LogicalOpType.LeftOuterApply
    | "LeftAntiSemiJoin" -> POk LogicalOpType.LeftAntiSemiJoin
    | "LeftDiff" -> POk LogicalOpType.LeftDiff
    | "LeftDiffAll" -> POk LogicalOpType.LeftDiffAll
    | "LeftOuterJoin" -> POk LogicalOpType.LeftOuterJoin
    | "LeftSemiJoin" -> POk LogicalOpType.LeftSemiJoin
    | "LocalCube" -> POk LogicalOpType.LocalCube
    | "LogRowScan" -> POk LogicalOpType.LogRowScan
    | "Merge" -> POk LogicalOpType.Merge
    | "MergeInterval" -> POk LogicalOpType.MergeInterval
    | "MergeStats" -> POk LogicalOpType.MergeStats
    | "Move" -> POk LogicalOpType.Move
    | "ParameterTableScan" -> POk LogicalOpType.ParameterTableScan
    | "PartialAggregate" -> POk LogicalOpType.PartialAggregate
    | "Print" -> POk LogicalOpType.Print
    | "Project" -> POk LogicalOpType.Project
    | "Put" -> POk LogicalOpType.Put
    | "Rank" -> POk LogicalOpType.Rank
    | "RemoteDelete" -> POk LogicalOpType.RemoteDelete
    | "RemoteIndexScan" -> POk LogicalOpType.RemoteIndexScan
    | "RemoteIndexSeek" -> POk LogicalOpType.RemoteIndexSeek
    | "RemoteInsert" -> POk LogicalOpType.RemoteInsert
    | "RemoteQuery" -> POk LogicalOpType.RemoteQuery
    | "RemoteScan" -> POk LogicalOpType.RemoteScan
    | "RemoteUpdate" -> POk LogicalOpType.RemoteUpdate
    | "RepartitionStreams" -> POk LogicalOpType.RepartitionStreams
    | "RIDLookup" -> POk LogicalOpType.RIDLookup
    | "RightAntiSemiJoin" -> POk LogicalOpType.RightAntiSemiJoin
    | "RightDiff" -> POk LogicalOpType.RightDiff
    | "RightDiffAll" -> POk LogicalOpType.RightDiffAll
    | "RightOuterJoin" -> POk LogicalOpType.RightOuterJoin
    | "RightSemiJoin" -> POk LogicalOpType.RightSemiJoin
    | "Segment" -> POk LogicalOpType.Segment
    | "Sequence" -> POk LogicalOpType.Sequence
    | "Sort" -> POk LogicalOpType.Sort
    | "Split" -> POk LogicalOpType.Split
    | "Switch" -> POk LogicalOpType.Switch
    | "TableValuedFunction" -> POk LogicalOpType.TableValuedFunction
    | "TableScan" -> POk LogicalOpType.TableScan
    | "Top" -> POk LogicalOpType.Top
    | "TopNSort" -> POk LogicalOpType.TopNSort
    | "UDX" -> POk LogicalOpType.UDX
    | "Union" -> POk LogicalOpType.Union
    | "UnionAll" -> POk LogicalOpType.UnionAll
    | "Update" -> POk LogicalOpType.Update
    | "LocalStats" -> POk LogicalOpType.LocalStats
    | "WindowSpool" -> POk LogicalOpType.WindowSpool
    | "WindowAggregate" -> POk LogicalOpType.WindowAggregate
    | "KeyLookup" -> POk LogicalOpType.KeyLookup
    | "ExtensibleColumnStoreScan" -> POk LogicalOpType.ExtensibleColumnStoreScan
    | other -> Failf "Unknown LogicalOp type: '%s'" other

let parsePhysicalOp (s : string) : PResult<PhysicalOpType, _> =
    match s with
    | "AdaptiveJoin" -> POk PhysicalOpType.AdaptiveJoin
    | "Apply" -> POk PhysicalOpType.Apply
    | "Assert" -> POk PhysicalOpType.Assert
    | "BatchHashTableBuild" -> POk PhysicalOpType.BatchHashTableBuild
    | "Bitmap" -> POk PhysicalOpType.Bitmap
    | "Broadcast" -> POk PhysicalOpType.Broadcast
    | "ClusteredIndexDelete" -> POk PhysicalOpType.ClusteredIndexDelete
    | "ClusteredIndexInsert" -> POk PhysicalOpType.ClusteredIndexInsert
    | "ClusteredIndexScan" -> POk PhysicalOpType.ClusteredIndexScan
    | "ClusteredIndexSeek" -> POk PhysicalOpType.ClusteredIndexSeek
    | "ClusteredIndexUpdate" -> POk PhysicalOpType.ClusteredIndexUpdate
    | "ClusteredIndexMerge" -> POk PhysicalOpType.ClusteredIndexMerge
    | "ClusteredUpdate" -> POk PhysicalOpType.ClusteredUpdate
    | "Collapse" -> POk PhysicalOpType.Collapse
    | "ColumnstoreIndexDelete" -> POk PhysicalOpType.ColumnstoreIndexDelete
    | "ColumnstoreIndexInsert" -> POk PhysicalOpType.ColumnstoreIndexInsert
    | "ColumnstoreIndexMerge" -> POk PhysicalOpType.ColumnstoreIndexMerge
    | "ColumnstoreIndexScan" -> POk PhysicalOpType.ColumnstoreIndexScan
    | "ColumnstoreIndexUpdate" -> POk PhysicalOpType.ColumnstoreIndexUpdate
    | "ComputeScalar" -> POk PhysicalOpType.ComputeScalar
    | "ComputeToControlNode" -> POk PhysicalOpType.ComputeToControlNode
    | "Concatenation" -> POk PhysicalOpType.Concatenation
    | "ConstantScan" -> POk PhysicalOpType.ConstantScan
    | "ConstantTableGet" -> POk PhysicalOpType.ConstantTableGet
    | "ControlToComputeNodes" -> POk PhysicalOpType.ControlToComputeNodes
    | "Delete" -> POk PhysicalOpType.Delete
    | "DeletedScan" -> POk PhysicalOpType.DeletedScan
    | "ExternalBroadcast" -> POk PhysicalOpType.ExternalBroadcast
    | "ExternalExtractionScan" -> POk PhysicalOpType.ExternalExtractionScan
    | "ExternalLocalStreaming" -> POk PhysicalOpType.ExternalLocalStreaming
    | "ExternalRoundRobin" -> POk PhysicalOpType.ExternalRoundRobin
    | "ExternalSelect" -> POk PhysicalOpType.ExternalSelect
    | "ExternalShuffle" -> POk PhysicalOpType.ExternalShuffle
    | "Filter" -> POk PhysicalOpType.Filter
    | "ForeignKeyReferencesCheck" -> POk PhysicalOpType.ForeignKeyReferencesCheck
    | "GbAgg" -> POk PhysicalOpType.GbAgg
    | "GbApply" -> POk PhysicalOpType.GbApply
    | "Get" -> POk PhysicalOpType.Get
    | "Generic" -> POk PhysicalOpType.Generic
    | "HashMatch" -> POk PhysicalOpType.HashMatch
    | "IndexDelete" -> POk PhysicalOpType.IndexDelete
    | "IndexInsert" -> POk PhysicalOpType.IndexInsert
    | "IndexScan" -> POk PhysicalOpType.IndexScan
    | "Insert" -> POk PhysicalOpType.Insert
    | "Join" -> POk PhysicalOpType.Join
    | "IndexSeek" -> POk PhysicalOpType.IndexSeek
    | "IndexSpool" -> POk PhysicalOpType.IndexSpool
    | "IndexUpdate" -> POk PhysicalOpType.IndexUpdate
    | "InsertedScan" -> POk PhysicalOpType.InsertedScan
    | "LocalCube" -> POk PhysicalOpType.LocalCube
    | "LogRowScan" -> POk PhysicalOpType.LogRowScan
    | "MergeInterval" -> POk PhysicalOpType.MergeInterval
    | "MergeJoin" -> POk PhysicalOpType.MergeJoin
    | "NestedLoops" -> POk PhysicalOpType.NestedLoops
    | "OnlineIndexInsert" -> POk PhysicalOpType.OnlineIndexInsert
    | "Parallelism" -> POk PhysicalOpType.Parallelism
    | "ParameterTableScan" -> POk PhysicalOpType.ParameterTableScan
    | "Print" -> POk PhysicalOpType.Print
    | "Project" -> POk PhysicalOpType.Project
    | "Put" -> POk PhysicalOpType.Put
    | "Rank" -> POk PhysicalOpType.Rank
    | "RemoteDelete" -> POk PhysicalOpType.RemoteDelete
    | "RemoteIndexScan" -> POk PhysicalOpType.RemoteIndexScan
    | "RemoteIndexSeek" -> POk PhysicalOpType.RemoteIndexSeek
    | "RemoteInsert" -> POk PhysicalOpType.RemoteInsert
    | "RemoteQuery" -> POk PhysicalOpType.RemoteQuery
    | "RemoteScan" -> POk PhysicalOpType.RemoteScan
    | "RemoteUpdate" -> POk PhysicalOpType.RemoteUpdate
    | "RIDLookup" -> POk PhysicalOpType.RIDLookup
    | "RowCountSpool" -> POk PhysicalOpType.RowCountSpool
    | "Segment" -> POk PhysicalOpType.Segment
    | "Sequence" -> POk PhysicalOpType.Sequence
    | "SequenceProject" -> POk PhysicalOpType.SequenceProject
    | "Shuffle" -> POk PhysicalOpType.Shuffle
    | "SingleSourceRoundRobinMove" -> POk PhysicalOpType.SingleSourceRoundRobinMove
    | "Sort" -> POk PhysicalOpType.Sort
    | "Split" -> POk PhysicalOpType.Split
    | "StreamAggregate" -> POk PhysicalOpType.StreamAggregate
    | "Switch" -> POk PhysicalOpType.Switch
    | "TableDelete" -> POk PhysicalOpType.TableDelete
    | "TableInsert" -> POk PhysicalOpType.TableInsert
    | "TableMerge" -> POk PhysicalOpType.TableMerge
    | "TableScan" -> POk PhysicalOpType.TableScan
    | "TableSpool" -> POk PhysicalOpType.TableSpool
    | "TableUpdate" -> POk PhysicalOpType.TableUpdate
    | "TableValuedFunction" -> POk PhysicalOpType.TableValuedFunction
    | "Top" -> POk PhysicalOpType.Top
    | "Trim" -> POk PhysicalOpType.Trim
    | "UDX" -> POk PhysicalOpType.UDX
    | "Union" -> POk PhysicalOpType.Union
    | "UnionAll" -> POk PhysicalOpType.UnionAll
    | "WindowAggregate" -> POk PhysicalOpType.WindowAggregate
    | "WindowSpool" -> POk PhysicalOpType.WindowSpool
    | "KeyLookup" -> POk PhysicalOpType.KeyLookup
    | "ExtensibleColumnStoreScan" -> POk PhysicalOpType.ExtensibleColumnStoreScan
    | other -> Failf "Unknown PhysicalOp type: '%s'" other

let parseExecutionMode (s : string) : PResult<ExecutionModeType, _> =
    match s with
    | "Row" -> POk ExecutionModeType.Row
    | "Batch" -> POk ExecutionModeType.Batch
    | other -> Failf "Unknown ExecutionModeType type: '%s'" other


