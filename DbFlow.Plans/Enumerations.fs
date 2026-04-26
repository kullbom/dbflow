namespace DbFlow.ShowPlanXml

// ========================================
// Enumerations (SimpleTypes)
// ========================================

type StorageType =
    | RowStore
    | ColumnStore
    | MemoryOptimized

type ExecutionModeType =
    | Row
    | Batch

type CursorType =
    | Dynamic
    | FastForward
    | Keyset
    | SnapShot

type OrderType =
    | BACKWARD
    | FORWARD

type PartitionType =
    | Broadcast
    | Demand
    | Hash
    | NoPartitioning
    | Range
    | RoundRobin
    | CloneLocation

type CompareOpType =
    | BINARY_IS
    | BOTH_NULL
    | EQ
    | GE
    | GT
    | IS
    | IS_NOT
    | IS_NOT_NULL
    | IS_NULL
    | LE
    | LT
    | NE
    | ONE_NULL

type IndexKindType =
    | Heap
    | Clustered
    | FTSChangeTracking
    | FTSMapping
    | NonClustered
    | PrimaryXML
    | SecondaryXML
    | Spatial
    | ViewClustered
    | ViewNonClustered
    | NonClusteredHash
    | SelectiveXML
    | SecondarySelectiveXML

type CloneAccessScopeType =
    | Primary
    | Secondary
    | Both
    | Either
    | ExactMatch
    | Local

type LogicalOpType =
    | Aggregate
    | AntiDiff
    | Assert
    | AsyncConcat
    | BatchHashTableBuild
    | BitmapCreate
    | ClusteredIndexScan
    | ClusteredIndexSeek
    | ClusteredUpdate
    | Collapse
    | ComputeScalar
    | Concatenation
    | ConstantScan
    | ConstantTableGet
    | CrossJoin
    | Delete
    | DeletedScan
    | DistinctSort
    | Distinct
    | DistributeStreams
    | EagerSpool
    | ExternalExtractionScan
    | ExternalSelect
    | Filter
    | FlowDistinct
    | ForeignKeyReferencesCheck
    | FullOuterJoin
    | GatherStreams
    | GbAgg
    | GbApply
    | Get
    | Generic
    | InnerApply
    | IndexScan
    | IndexSeek
    | InnerJoin
    | Insert
    | InsertedScan
    | Intersect
    | IntersectAll
    | LazySpool
    | LeftAntiSemiApply
    | LeftSemiApply
    | LeftOuterApply
    | LeftAntiSemiJoin
    | LeftDiff
    | LeftDiffAll
    | LeftOuterJoin
    | LeftSemiJoin
    | LocalCube
    | LogRowScan
    | Merge
    | MergeInterval
    | MergeStats
    | Move
    | ParameterTableScan
    | PartialAggregate
    | Print
    | Project
    | Put
    | Rank
    | RemoteDelete
    | RemoteIndexScan
    | RemoteIndexSeek
    | RemoteInsert
    | RemoteQuery
    | RemoteScan
    | RemoteUpdate
    | RepartitionStreams
    | RIDLookup
    | RightAntiSemiJoin
    | RightDiff
    | RightDiffAll
    | RightOuterJoin
    | RightSemiJoin
    | Segment
    | Sequence
    | Sort
    | Split
    | Switch
    | TableValuedFunction
    | TableScan
    | Top
    | TopNSort
    | UDX
    | Union
    | UnionAll
    | Update
    | LocalStats
    | WindowSpool
    | WindowAggregate
    | KeyLookup
    | ExtensibleColumnStoreScan

type PhysicalOpType =
    | AdaptiveJoin
    | Apply
    | Assert
    | BatchHashTableBuild
    | Bitmap
    | Broadcast
    | ClusteredIndexDelete
    | ClusteredIndexInsert
    | ClusteredIndexScan
    | ClusteredIndexSeek
    | ClusteredIndexUpdate
    | ClusteredIndexMerge
    | ClusteredUpdate
    | Collapse
    | ColumnstoreIndexDelete
    | ColumnstoreIndexInsert
    | ColumnstoreIndexMerge
    | ColumnstoreIndexScan
    | ColumnstoreIndexUpdate
    | ComputeScalar
    | ComputeToControlNode
    | Concatenation
    | ConstantScan
    | ConstantTableGet
    | ControlToComputeNodes
    | Delete
    | DeletedScan
    | ExternalBroadcast
    | ExternalExtractionScan
    | ExternalLocalStreaming
    | ExternalRoundRobin
    | ExternalSelect
    | ExternalShuffle
    | Filter
    | ForeignKeyReferencesCheck
    | GbAgg
    | GbApply
    | Get
    | Generic
    | HashMatch
    | IndexDelete
    | IndexInsert
    | IndexScan
    | Insert
    | Join
    | IndexSeek
    | IndexSpool
    | IndexUpdate
    | InsertedScan
    | LocalCube
    | LogRowScan
    | MergeInterval
    | MergeJoin
    | NestedLoops
    | OnlineIndexInsert
    | Parallelism
    | ParameterTableScan
    | Print
    | Project
    | Put
    | Rank
    | RemoteDelete
    | RemoteIndexScan
    | RemoteIndexSeek
    | RemoteInsert
    | RemoteQuery
    | RemoteScan
    | RemoteUpdate
    | RIDLookup
    | RowCountSpool
    | Segment
    | Sequence
    | SequenceProject
    | Shuffle
    | SingleSourceRoundRobinMove
    | Sort
    | Split
    | StreamAggregate
    | Switch
    | TableDelete
    | TableInsert
    | TableMerge
    | TableScan
    | TableSpool
    | TableUpdate
    | TableValuedFunction
    | Top
    | Trim
    | UDX
    | Union
    | UnionAll
    | WindowAggregate
    | WindowSpool
    | KeyLookup
    | ExtensibleColumnStoreScan

// ========================================
// Scalar Operator Enumerations
// ========================================

type ArithmeticOperationType =
    | ADD
    | BIT_ADD
    | BIT_AND
    | BIT_COMBINE
    | BIT_NOT
    | BIT_OR
    | BIT_XOR
    | DIV
    | HASH
    | MINUS
    | MOD
    | MULT
    | SUB
    | CONCAT

type LogicalOperationType =
    | AND
    | IMPLIES
    | IS_NOT_NULL
    | IS_NULL
    | IS
    | IsFalseOrNull
    | NOT
    | OR
    | XOR

type SubqueryOperationType =
    | EQ_ALL
    | EQ_ANY
    | EXISTS
    | GE_ALL
    | GE_ANY
    | GT_ALL
    | GT_ANY
    | IN
    | LE_ALL
    | LE_ANY
    | LT_ALL
    | LT_ANY
    | NE_ALL
    | NE_ANY

