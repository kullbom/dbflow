module DbFlow.Plans.EnumsParsers

open System

open DbFlow
open DbFlow.ShowPlanXml
open DbFlow.XmlParser

// ========================================
// Enum Parsers - no mutual dependencies
// ========================================

let parseIndexKind (s : string) : Result<IndexKindType, _> =
    match s with
    | "Heap" -> Ok IndexKindType.Heap
    | "Clustered" -> Ok IndexKindType.Clustered
    | "FTSChangeTracking" -> Ok IndexKindType.FTSChangeTracking
    | "FTSMapping" -> Ok IndexKindType.FTSMapping
    | "NonClustered" -> Ok IndexKindType.NonClustered
    | "PrimaryXML" -> Ok IndexKindType.PrimaryXML
    | "SecondaryXML" -> Ok IndexKindType.SecondaryXML
    | "Spatial" -> Ok IndexKindType.Spatial
    | "ViewClustered" -> Ok IndexKindType.ViewClustered
    | "ViewNonClustered" -> Ok IndexKindType.ViewNonClustered
    | "NonClusteredHash" -> Ok IndexKindType.NonClusteredHash
    | "SelectiveXML" -> Ok IndexKindType.SelectiveXML
    | "SecondarySelectiveXML" -> Ok IndexKindType.SecondarySelectiveXML
    | other -> Failf "Unknown IndexKindType: '%s'" other

let parseCloneAccessScope (s : string) : Result<CloneAccessScopeType, _> =
    match s with
    | "Primary" -> Ok CloneAccessScopeType.Primary
    | "Secondary" -> Ok CloneAccessScopeType.Secondary
    | "Both" -> Ok CloneAccessScopeType.Both
    | "Either" -> Ok CloneAccessScopeType.Either
    | "ExactMatch" -> Ok CloneAccessScopeType.ExactMatch
    | "Local" -> Ok CloneAccessScopeType.Local
    | other -> Failf "Unknown CloneAccessScopeType: '%s'" other

let parseStorage (s : string) : Result<StorageType, _> =
    match s with
    | "RowStore" -> Ok StorageType.RowStore
    | "ColumnStore" -> Ok StorageType.ColumnStore
    | "MemoryOptimized" -> Ok StorageType.MemoryOptimized
    | other -> Failf "Unknown StorageType: '%s'" other

let parseArithmeticOperation (s : string) : Result<ArithmeticOperationType, _> =
    match s with
    | "ADD" -> Ok ArithmeticOperationType.ADD
    | "BIT_ADD" -> Ok ArithmeticOperationType.BIT_ADD
    | "BIT_AND" -> Ok ArithmeticOperationType.BIT_AND
    | "BIT_COMBINE" -> Ok ArithmeticOperationType.BIT_COMBINE
    | "BIT_NOT" -> Ok ArithmeticOperationType.BIT_NOT
    | "BIT_OR" -> Ok ArithmeticOperationType.BIT_OR
    | "BIT_XOR" -> Ok ArithmeticOperationType.BIT_XOR
    | "DIV" -> Ok ArithmeticOperationType.DIV
    | "HASH" -> Ok ArithmeticOperationType.HASH
    | "MINUS" -> Ok ArithmeticOperationType.MINUS
    | "MOD" -> Ok ArithmeticOperationType.MOD
    | "MULT" -> Ok ArithmeticOperationType.MULT
    | "SUB" -> Ok ArithmeticOperationType.SUB
    | "CONCAT" -> Ok ArithmeticOperationType.CONCAT
    | other -> Failf "Unknown ArithmeticOperationType: '%s'" other

let parseLogicalOperation (s : string) : Result<LogicalOperationType, _> =
    match s with
    | "AND" -> Ok LogicalOperationType.AND
    | "IMPLIES" -> Ok LogicalOperationType.IMPLIES
    | "IS NOT NULL" -> Ok LogicalOperationType.IS_NOT_NULL
    | "IS NULL" -> Ok LogicalOperationType.IS_NULL
    | "IS" -> Ok LogicalOperationType.IS
    | "IsFalseOrNull" -> Ok LogicalOperationType.IsFalseOrNull
    | "NOT" -> Ok LogicalOperationType.NOT
    | "OR" -> Ok LogicalOperationType.OR
    | "XOR" -> Ok LogicalOperationType.XOR
    | other -> Failf "Unknown LogicalOperationType: '%s'" other

let parseSubqueryOperation (s : string) : Result<SubqueryOperationType, _> =
    match s with
    | "EQ ALL" -> Ok SubqueryOperationType.EQ_ALL
    | "EQ ANY" -> Ok SubqueryOperationType.EQ_ANY
    | "EXISTS" -> Ok SubqueryOperationType.EXISTS
    | "GE ALL" -> Ok SubqueryOperationType.GE_ALL
    | "GE ANY" -> Ok SubqueryOperationType.GE_ANY
    | "GT ALL" -> Ok SubqueryOperationType.GT_ALL
    | "GT ANY" -> Ok SubqueryOperationType.GT_ANY
    | "IN" -> Ok SubqueryOperationType.IN
    | "LE ALL" -> Ok SubqueryOperationType.LE_ALL
    | "LE ANY" -> Ok SubqueryOperationType.LE_ANY
    | "LT ALL" -> Ok SubqueryOperationType.LT_ALL
    | "LT ANY" -> Ok SubqueryOperationType.LT_ANY
    | "NE ALL" -> Ok SubqueryOperationType.NE_ALL
    | "NE ANY" -> Ok SubqueryOperationType.NE_ANY
    | other -> Failf "Unknown SubqueryOperationType: '%s'" other

let parseCompareOp (s : string) : Result<CompareOpType, _> =
    match s with
    | "BINARY IS" -> Ok CompareOpType.BINARY_IS
    | "BOTH NULL" -> Ok CompareOpType.BOTH_NULL
    | "EQ" -> Ok CompareOpType.EQ
    | "GE" -> Ok CompareOpType.GE
    | "GT" -> Ok CompareOpType.GT
    | "IS" -> Ok CompareOpType.IS
    | "IS NOT" -> Ok CompareOpType.IS_NOT
    | "IS NOT NULL" -> Ok CompareOpType.IS_NOT_NULL
    | "IS NULL" -> Ok CompareOpType.IS_NULL
    | "LE" -> Ok CompareOpType.LE
    | "LT" -> Ok CompareOpType.LT
    | "NE" -> Ok CompareOpType.NE
    | "ONE NULL" -> Ok CompareOpType.ONE_NULL
    | other -> Failf "Unknown CompareOpType: '%s'" other

let parseLogicalOp (s : string) : Result<LogicalOpType, _> =
    match s with
    | "Aggregate" -> Ok LogicalOpType.Aggregate
    | "AntiDiff" -> Ok LogicalOpType.AntiDiff
    | "Assert" -> Ok LogicalOpType.Assert
    | "AsyncConcat" -> Ok LogicalOpType.AsyncConcat
    | "BatchHashTableBuild" -> Ok LogicalOpType.BatchHashTableBuild
    | "BitmapCreate" -> Ok LogicalOpType.BitmapCreate
    | "ClusteredIndexScan" -> Ok LogicalOpType.ClusteredIndexScan
    | "ClusteredIndexSeek" -> Ok LogicalOpType.ClusteredIndexSeek
    | "ClusteredUpdate" -> Ok LogicalOpType.ClusteredUpdate
    | "Collapse" -> Ok LogicalOpType.Collapse
    | "ComputeScalar" -> Ok LogicalOpType.ComputeScalar
    | "Concatenation" -> Ok LogicalOpType.Concatenation
    | "ConstantScan" -> Ok LogicalOpType.ConstantScan
    | "ConstantTableGet" -> Ok LogicalOpType.ConstantTableGet
    | "CrossJoin" -> Ok LogicalOpType.CrossJoin
    | "Delete" -> Ok LogicalOpType.Delete
    | "DeletedScan" -> Ok LogicalOpType.DeletedScan
    | "DistinctSort" -> Ok LogicalOpType.DistinctSort
    | "Distinct" -> Ok LogicalOpType.Distinct
    | "DistributeStreams" -> Ok LogicalOpType.DistributeStreams
    | "EagerSpool" -> Ok LogicalOpType.EagerSpool
    | "ExternalExtractionScan" -> Ok LogicalOpType.ExternalExtractionScan
    | "ExternalSelect" -> Ok LogicalOpType.ExternalSelect
    | "Filter" -> Ok LogicalOpType.Filter
    | "FlowDistinct" -> Ok LogicalOpType.FlowDistinct
    | "ForeignKeyReferencesCheck" -> Ok LogicalOpType.ForeignKeyReferencesCheck
    | "FullOuterJoin" -> Ok LogicalOpType.FullOuterJoin
    | "GatherStreams" -> Ok LogicalOpType.GatherStreams
    | "GbAgg" -> Ok LogicalOpType.GbAgg
    | "GbApply" -> Ok LogicalOpType.GbApply
    | "Get" -> Ok LogicalOpType.Get
    | "Generic" -> Ok LogicalOpType.Generic
    | "InnerApply" -> Ok LogicalOpType.InnerApply
    | "IndexScan" -> Ok LogicalOpType.IndexScan
    | "IndexSeek" -> Ok LogicalOpType.IndexSeek
    | "InnerJoin" -> Ok LogicalOpType.InnerJoin
    | "Insert" -> Ok LogicalOpType.Insert
    | "InsertedScan" -> Ok LogicalOpType.InsertedScan
    | "Intersect" -> Ok LogicalOpType.Intersect
    | "IntersectAll" -> Ok LogicalOpType.IntersectAll
    | "LazySpool" -> Ok LogicalOpType.LazySpool
    | "LeftAntiSemiApply" -> Ok LogicalOpType.LeftAntiSemiApply
    | "LeftSemiApply" -> Ok LogicalOpType.LeftSemiApply
    | "LeftOuterApply" -> Ok LogicalOpType.LeftOuterApply
    | "LeftAntiSemiJoin" -> Ok LogicalOpType.LeftAntiSemiJoin
    | "LeftDiff" -> Ok LogicalOpType.LeftDiff
    | "LeftDiffAll" -> Ok LogicalOpType.LeftDiffAll
    | "LeftOuterJoin" -> Ok LogicalOpType.LeftOuterJoin
    | "LeftSemiJoin" -> Ok LogicalOpType.LeftSemiJoin
    | "LocalCube" -> Ok LogicalOpType.LocalCube
    | "LogRowScan" -> Ok LogicalOpType.LogRowScan
    | "Merge" -> Ok LogicalOpType.Merge
    | "MergeInterval" -> Ok LogicalOpType.MergeInterval
    | "MergeStats" -> Ok LogicalOpType.MergeStats
    | "Move" -> Ok LogicalOpType.Move
    | "ParameterTableScan" -> Ok LogicalOpType.ParameterTableScan
    | "PartialAggregate" -> Ok LogicalOpType.PartialAggregate
    | "Print" -> Ok LogicalOpType.Print
    | "Project" -> Ok LogicalOpType.Project
    | "Put" -> Ok LogicalOpType.Put
    | "Rank" -> Ok LogicalOpType.Rank
    | "RemoteDelete" -> Ok LogicalOpType.RemoteDelete
    | "RemoteIndexScan" -> Ok LogicalOpType.RemoteIndexScan
    | "RemoteIndexSeek" -> Ok LogicalOpType.RemoteIndexSeek
    | "RemoteInsert" -> Ok LogicalOpType.RemoteInsert
    | "RemoteQuery" -> Ok LogicalOpType.RemoteQuery
    | "RemoteScan" -> Ok LogicalOpType.RemoteScan
    | "RemoteUpdate" -> Ok LogicalOpType.RemoteUpdate
    | "RepartitionStreams" -> Ok LogicalOpType.RepartitionStreams
    | "RIDLookup" -> Ok LogicalOpType.RIDLookup
    | "RightAntiSemiJoin" -> Ok LogicalOpType.RightAntiSemiJoin
    | "RightDiff" -> Ok LogicalOpType.RightDiff
    | "RightDiffAll" -> Ok LogicalOpType.RightDiffAll
    | "RightOuterJoin" -> Ok LogicalOpType.RightOuterJoin
    | "RightSemiJoin" -> Ok LogicalOpType.RightSemiJoin
    | "Segment" -> Ok LogicalOpType.Segment
    | "Sequence" -> Ok LogicalOpType.Sequence
    | "Sort" -> Ok LogicalOpType.Sort
    | "Split" -> Ok LogicalOpType.Split
    | "Switch" -> Ok LogicalOpType.Switch
    | "TableValuedFunction" -> Ok LogicalOpType.TableValuedFunction
    | "TableScan" -> Ok LogicalOpType.TableScan
    | "Top" -> Ok LogicalOpType.Top
    | "TopNSort" -> Ok LogicalOpType.TopNSort
    | "UDX" -> Ok LogicalOpType.UDX
    | "Union" -> Ok LogicalOpType.Union
    | "UnionAll" -> Ok LogicalOpType.UnionAll
    | "Update" -> Ok LogicalOpType.Update
    | "LocalStats" -> Ok LogicalOpType.LocalStats
    | "WindowSpool" -> Ok LogicalOpType.WindowSpool
    | "WindowAggregate" -> Ok LogicalOpType.WindowAggregate
    | "KeyLookup" -> Ok LogicalOpType.KeyLookup
    | "ExtensibleColumnStoreScan" -> Ok LogicalOpType.ExtensibleColumnStoreScan
    | other -> Failf "Unknown LogicalOp type: '%s'" other

let parsePhysicalOp (s : string) : Result<PhysicalOpType, _> =
    match s with
    | "AdaptiveJoin" -> Ok PhysicalOpType.AdaptiveJoin
    | "Apply" -> Ok PhysicalOpType.Apply
    | "Assert" -> Ok PhysicalOpType.Assert
    | "BatchHashTableBuild" -> Ok PhysicalOpType.BatchHashTableBuild
    | "Bitmap" -> Ok PhysicalOpType.Bitmap
    | "Broadcast" -> Ok PhysicalOpType.Broadcast
    | "ClusteredIndexDelete" -> Ok PhysicalOpType.ClusteredIndexDelete
    | "ClusteredIndexInsert" -> Ok PhysicalOpType.ClusteredIndexInsert
    | "ClusteredIndexScan" -> Ok PhysicalOpType.ClusteredIndexScan
    | "ClusteredIndexSeek" -> Ok PhysicalOpType.ClusteredIndexSeek
    | "ClusteredIndexUpdate" -> Ok PhysicalOpType.ClusteredIndexUpdate
    | "ClusteredIndexMerge" -> Ok PhysicalOpType.ClusteredIndexMerge
    | "ClusteredUpdate" -> Ok PhysicalOpType.ClusteredUpdate
    | "Collapse" -> Ok PhysicalOpType.Collapse
    | "ColumnstoreIndexDelete" -> Ok PhysicalOpType.ColumnstoreIndexDelete
    | "ColumnstoreIndexInsert" -> Ok PhysicalOpType.ColumnstoreIndexInsert
    | "ColumnstoreIndexMerge" -> Ok PhysicalOpType.ColumnstoreIndexMerge
    | "ColumnstoreIndexScan" -> Ok PhysicalOpType.ColumnstoreIndexScan
    | "ColumnstoreIndexUpdate" -> Ok PhysicalOpType.ColumnstoreIndexUpdate
    | "ComputeScalar" -> Ok PhysicalOpType.ComputeScalar
    | "ComputeToControlNode" -> Ok PhysicalOpType.ComputeToControlNode
    | "Concatenation" -> Ok PhysicalOpType.Concatenation
    | "ConstantScan" -> Ok PhysicalOpType.ConstantScan
    | "ConstantTableGet" -> Ok PhysicalOpType.ConstantTableGet
    | "ControlToComputeNodes" -> Ok PhysicalOpType.ControlToComputeNodes
    | "Delete" -> Ok PhysicalOpType.Delete
    | "DeletedScan" -> Ok PhysicalOpType.DeletedScan
    | "ExternalBroadcast" -> Ok PhysicalOpType.ExternalBroadcast
    | "ExternalExtractionScan" -> Ok PhysicalOpType.ExternalExtractionScan
    | "ExternalLocalStreaming" -> Ok PhysicalOpType.ExternalLocalStreaming
    | "ExternalRoundRobin" -> Ok PhysicalOpType.ExternalRoundRobin
    | "ExternalSelect" -> Ok PhysicalOpType.ExternalSelect
    | "ExternalShuffle" -> Ok PhysicalOpType.ExternalShuffle
    | "Filter" -> Ok PhysicalOpType.Filter
    | "ForeignKeyReferencesCheck" -> Ok PhysicalOpType.ForeignKeyReferencesCheck
    | "GbAgg" -> Ok PhysicalOpType.GbAgg
    | "GbApply" -> Ok PhysicalOpType.GbApply
    | "Get" -> Ok PhysicalOpType.Get
    | "Generic" -> Ok PhysicalOpType.Generic
    | "HashMatch" -> Ok PhysicalOpType.HashMatch
    | "IndexDelete" -> Ok PhysicalOpType.IndexDelete
    | "IndexInsert" -> Ok PhysicalOpType.IndexInsert
    | "IndexScan" -> Ok PhysicalOpType.IndexScan
    | "Insert" -> Ok PhysicalOpType.Insert
    | "Join" -> Ok PhysicalOpType.Join
    | "IndexSeek" -> Ok PhysicalOpType.IndexSeek
    | "IndexSpool" -> Ok PhysicalOpType.IndexSpool
    | "IndexUpdate" -> Ok PhysicalOpType.IndexUpdate
    | "InsertedScan" -> Ok PhysicalOpType.InsertedScan
    | "LocalCube" -> Ok PhysicalOpType.LocalCube
    | "LogRowScan" -> Ok PhysicalOpType.LogRowScan
    | "MergeInterval" -> Ok PhysicalOpType.MergeInterval
    | "MergeJoin" -> Ok PhysicalOpType.MergeJoin
    | "NestedLoops" -> Ok PhysicalOpType.NestedLoops
    | "OnlineIndexInsert" -> Ok PhysicalOpType.OnlineIndexInsert
    | "Parallelism" -> Ok PhysicalOpType.Parallelism
    | "ParameterTableScan" -> Ok PhysicalOpType.ParameterTableScan
    | "Print" -> Ok PhysicalOpType.Print
    | "Project" -> Ok PhysicalOpType.Project
    | "Put" -> Ok PhysicalOpType.Put
    | "Rank" -> Ok PhysicalOpType.Rank
    | "RemoteDelete" -> Ok PhysicalOpType.RemoteDelete
    | "RemoteIndexScan" -> Ok PhysicalOpType.RemoteIndexScan
    | "RemoteIndexSeek" -> Ok PhysicalOpType.RemoteIndexSeek
    | "RemoteInsert" -> Ok PhysicalOpType.RemoteInsert
    | "RemoteQuery" -> Ok PhysicalOpType.RemoteQuery
    | "RemoteScan" -> Ok PhysicalOpType.RemoteScan
    | "RemoteUpdate" -> Ok PhysicalOpType.RemoteUpdate
    | "RIDLookup" -> Ok PhysicalOpType.RIDLookup
    | "RowCountSpool" -> Ok PhysicalOpType.RowCountSpool
    | "Segment" -> Ok PhysicalOpType.Segment
    | "Sequence" -> Ok PhysicalOpType.Sequence
    | "SequenceProject" -> Ok PhysicalOpType.SequenceProject
    | "Shuffle" -> Ok PhysicalOpType.Shuffle
    | "SingleSourceRoundRobinMove" -> Ok PhysicalOpType.SingleSourceRoundRobinMove
    | "Sort" -> Ok PhysicalOpType.Sort
    | "Split" -> Ok PhysicalOpType.Split
    | "StreamAggregate" -> Ok PhysicalOpType.StreamAggregate
    | "Switch" -> Ok PhysicalOpType.Switch
    | "TableDelete" -> Ok PhysicalOpType.TableDelete
    | "TableInsert" -> Ok PhysicalOpType.TableInsert
    | "TableMerge" -> Ok PhysicalOpType.TableMerge
    | "TableScan" -> Ok PhysicalOpType.TableScan
    | "TableSpool" -> Ok PhysicalOpType.TableSpool
    | "TableUpdate" -> Ok PhysicalOpType.TableUpdate
    | "TableValuedFunction" -> Ok PhysicalOpType.TableValuedFunction
    | "Top" -> Ok PhysicalOpType.Top
    | "Trim" -> Ok PhysicalOpType.Trim
    | "UDX" -> Ok PhysicalOpType.UDX
    | "Union" -> Ok PhysicalOpType.Union
    | "UnionAll" -> Ok PhysicalOpType.UnionAll
    | "WindowAggregate" -> Ok PhysicalOpType.WindowAggregate
    | "WindowSpool" -> Ok PhysicalOpType.WindowSpool
    | "KeyLookup" -> Ok PhysicalOpType.KeyLookup
    | "ExtensibleColumnStoreScan" -> Ok PhysicalOpType.ExtensibleColumnStoreScan
    | other -> Failf "Unknown PhysicalOp type: '%s'" other

let parseExecutionMode (s : string) : Result<ExecutionModeType, _> =
    match s with
    | "Row" -> Ok ExecutionModeType.Row
    | "Batch" -> Ok ExecutionModeType.Batch
    | other -> Failf "Unknown ExecutionModeType type: '%s'" other


