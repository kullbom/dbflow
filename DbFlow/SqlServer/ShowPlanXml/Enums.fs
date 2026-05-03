namespace DbFlow.SqlServer.ShowPlanXml

// ========================================
// Enumerations (SimpleTypes)
// ========================================

[<RequireQualifiedAccess>]
type OrderType =
    | BACKWARD
    | FORWARD
    
[<RequireQualifiedAccess>]
type SetPredicateType =
    | Update
    | Insert

[<RequireQualifiedAccess>]
type StorageType =
    | RowStore
    | ColumnStore
    | MemoryOptimized

[<RequireQualifiedAccess>]
type ExecutionModeType =
    | Row
    | Batch

[<RequireQualifiedAccess>]
type CursorType =
    | Dynamic
    | FastForward
    | Keyset
    | SnapShot

[<RequireQualifiedAccess>]
type PartitionType =
    | Broadcast
    | Demand
    | Hash
    | NoPartitioning
    | Range
    | RoundRobin
    | CloneLocation

[<RequireQualifiedAccess>]
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

[<RequireQualifiedAccess>]
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

[<RequireQualifiedAccess>]
type CloneAccessScopeType =
    | Primary
    | Secondary
    | Both
    | Either
    | ExactMatch
    | Local

/// These are the logical operators to which "query"
/// portions of T-SQL statement are translated. Subsequent
/// to that translation, a physical operator is chosen for
/// evaluating each logical operator. The SQL Server query
/// optimizer uses a cost-based approach to decide which 
/// physical operator will implement a logical operator.
[<RequireQualifiedAccess>]
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

/// Each of the physical operator is an iterator. An iterator
/// can answer three method calls: Init(), GetNext(), and Close().
/// Upon receiving an Init() call, an iterator initializes itself,
/// setting up any data structures if necessary. Upon receiving a
/// GetNext() call, the iterator produces the "next" packet of 
/// data and gives it to the iterator that made the GetNext() call.
/// To produce the "next" packet of data, the iterator may have to
/// make zero or more GetNext() (or even Init()) calls to its 
/// children. Upon receiving a Close() call, an iterator performs
/// some clean-up operations and shuts itself down. Typically, an
/// iterator receives one Init() call, followed by many GetNext()
/// calls, and then a single Close() call.
/// 
/// The "query" portion of a T-SQL statement is typically a tree
/// made up of iterators.  
/// 
/// Usually, there is a one-to-many mapping among logical operators
/// and physical operators. That is, usually multiple physical operators
/// can implement a logical operator. In some cases in SQL Server,
/// however, a physical operator can implement multiple logical operators.
[<RequireQualifiedAccess>]
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

[<RequireQualifiedAccess>]
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

[<RequireQualifiedAccess>]
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

[<RequireQualifiedAccess>]
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

// ========================================
// Memory and Warning Enumerations
// ========================================

[<RequireQualifiedAccess>]
type MemoryGrantWarningType =
    | ExcessiveGrant
    | UsedMoreThanGranted
    | GrantIncrease

[<RequireQualifiedAccess>]
type MemoryGrantFeedbackInfoType =
    | YesAdjusting
    | YesStable
    | NoFirstExecution
    | NoAccurateGrant
    | NoFeedbackDisabled
    | YesPercentileAdjusting

[<RequireQualifiedAccess>]
type DOPFeedbackInfoType =
    | YesAdjusting
    | YesStable
    | NoFeedback

[<RequireQualifiedAccess>]
type TraceFlagScopeType =
    | Global
    | Session

// ========================================
// Cursor Operation Type
// ========================================

[<RequireQualifiedAccess>]
type CursorOperationType =
    | FetchQuery
    | PopulateQuery
    | RefreshQuery

[<RequireQualifiedAccess>]
type CursorConcurrencyType =
    | ReadOnly
    | Pessimistic
    | Optimistic

// ========================================
// Receive Plan Operation Type
// ========================================

[<RequireQualifiedAccess>]
type ReceivePlanOperationType =
    | ReceivePlanSelect
    | ReceivePlanUpdate

// ========================================
// Column Group Usage Type
// ========================================

[<RequireQualifiedAccess>]
type ColumnGroupUsageType =
    | EQUALITY
    | INEQUALITY
    | INCLUDE

// ========================================
// Star Join Operation Type
// ========================================

[<RequireQualifiedAccess>]
type StarJoinOperationType =
    | Fetch
    | IndexIntersection
    | IndexFilter
    | IndexLookup

// ========================================
// Activation Type (Parallelism)
// ========================================

[<RequireQualifiedAccess>]
type ActivationType =
    | CloneLocation
    | Resource
    | SingleBrick
    | Region

// ========================================
// Conversion Warning Issue Type
// ========================================

[<RequireQualifiedAccess>]
type ConversionWarningIssueType =
    | CardinalityEstimate
    | SeekPlan

// ========================================
// Wait Type
// ========================================

[<RequireQualifiedAccess>]
type WaitTypeKind =
    | MemoryGrant

// ========================================
// Statement Optimization Early Abort Reason
// ========================================

[<RequireQualifiedAccess>]
type StatementOptmEarlyAbortReason =
    | TimeOut
    | MemoryLimitExceeded
    | GoodEnoughPlanFound

