module DbFlow.Test.ShowPlanXml

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
// Basic Types
// ========================================

type SetOptionsType = {
    ANSI_NULLS: bool option
    ANSI_PADDING: bool option
    ANSI_WARNINGS: bool option
    ARITHABORT: bool option
    CONCAT_NULL_YIELDS_NULL: bool option
    NUMERIC_ROUNDABORT: bool option
    QUOTED_IDENTIFIER: bool option
}

type ObjectType = {
    Server: string option
    Database: string option
    Schema: string option
    Table: string option
    Index: string option
    Filtered: bool option
    Alias: string option
    TableReferenceId: int option
    IndexKind: IndexKindType option
    CloneAccessScope: CloneAccessScopeType option
    Storage: StorageType option
}

[<System.ObsoleteAttribute("Not yet implemented")>]
type ScalarType = {
    // Placeholder - complex scalar operator type
    Value: string option
}

type InternalInfoType = {
    // Arbitrary XML content - can contain any child elements or attributes
    Content: string option
}

type ColumnReferenceType = {
    // Sequence elements
    ScalarOperator: ScalarType option
    InternalInfo: InternalInfoType option

    // Attributes
    Server: string option
    Database: string option
    Schema: string option
    Table: string option
    Alias: string option
    Column: string
    ComputedColumn: bool option
    ParameterDataType: string option
    ParameterCompiledValue: string option
    ParameterRuntimeValue: string option
}

type MemoryGrantType = {
    SerialRequiredMemory: uint64
    SerialDesiredMemory: uint64
    RequiredMemory: uint64 option
    DesiredMemory: uint64 option
    RequestedMemory: uint64 option
    GrantWaitTime: uint64 option
    GrantedMemory: uint64 option
    MaxUsedMemory: uint64 option
    MaxQueryMemory: uint64 option
    LastRequestedMemory: uint64 option
    IsMemoryGrantFeedbackAdjusted: string option
}

type OptimizerHardwareDependentPropertiesType = {
    EstimatedAvailableMemoryGrant: uint64
    EstimatedPagesCached: uint64
    EstimatedAvailableDegreeOfParallelism: uint64 option
    MaxCompileMemory: uint64 option
}

type StatsInfoType = {
    Database: string option
    Schema: string option
    Table: string option
    Statistics: string
    ModificationCount: uint64
    SamplingPercent: float
    LastUpdate: System.DateTime option
}

type OptimizerStatsUsageType = {
    StatisticsInfo: StatsInfoType list
}

type AffectingConvertWarningType = {
    ConvertIssue: string
    Expression: string
}

type WarningsType = {
    PlanAffectingConvert: AffectingConvertWarningType list
    NoJoinPredicate: bool option
    SpatialGuess: bool option
    UnmatchedIndexes: bool option
    FullUpdateForOnlineIndexBuild: bool option
}

type RunTimeCountersPerThread = {
    Thread: int
    ActualRows: uint64
    ActualEndOfScans: uint64
    ActualExecutions: uint64
    ActualExecutionMode: ExecutionModeType option
    ActualElapsedms: uint64 option
    ActualCPUms: uint64 option
    ActualScans: uint64 option
    ActualLogicalReads: uint64 option
    ActualPhysicalReads: uint64 option
}

type RunTimeInformationType = {
    RunTimeCountersPerThread: RunTimeCountersPerThread list
}

type MemoryFractionsType = {
    Input: float
    Output: float
}

type PartitionRangeType = {
    Start: uint64
    End: uint64
}

type PartitionsAccessedType = {
    PartitionCount: uint64
    PartitionRange: PartitionRangeType list
}

type RunTimePartitionSummaryType = {
    PartitionsAccessed: PartitionsAccessedType
}

// ========================================
// Relational Operators
// ========================================

type RelOpType = {
    // Sequence elements
    OutputList: ColumnReferenceType list
    Warnings: WarningsType option
    MemoryFractions: MemoryFractionsType option
    RunTimeInformation: RunTimeInformationType option
    RunTimePartitionSummary: RunTimePartitionSummaryType option
    InternalInfo: InternalInfoType option

    // Required attributes
    AvgRowSize: float
    EstimateCPU: float
    EstimateIO: float
    EstimateRebinds: float
    EstimateRewinds: float
    EstimateRows: float
    LogicalOp: LogicalOpType
    Parallel: bool
    PhysicalOp: PhysicalOpType
    EstimatedTotalSubtreeCost: float

    // Optional attributes
    EstimatedExecutionMode: ExecutionModeType option
    GroupExecuted: bool option
    EstimateRowsWithoutRowGoal: float option
    EstimatedRowsRead: float option
    NodeId: int option
    RemoteDataAccess: bool option
    Partitioned: bool option
    IsAdaptive: bool option
    AdaptiveThresholdRows: float option
    TableCardinality: float option
    StatsCollectionId: uint64 option
    EstimatedJoinType: PhysicalOpType option
    HyperScaleOptimizedQueryProcessing: string option
    HyperScaleOptimizedQueryProcessingUnusedReason: string option
    PDWAccumulativeCost: float option
}

// ========================================
// Query Plan
// ========================================

type QueryPlanType = {
    DegreeOfParallelism: int option
    NonParallelPlanReason: string option
    MemoryGrant: uint64 option
    CachedPlanSize: uint64 option
    CompileTime: uint64 option
    CompileCPU: uint64 option
    CompileMemory: uint64 option
    Warnings: WarningsType option
    MemoryGrantInfo: MemoryGrantType option
    OptimizerHardwareDependentProperties: OptimizerHardwareDependentPropertiesType option
    OptimizerStatsUsage: OptimizerStatsUsageType option
    RelOp: RelOpType
    ParameterList: ColumnReferenceType list
}

// ========================================
// Statement Types
// ========================================

type BaseStmtInfoType = {
    StatementSetOptions: SetOptionsType option
    StatementCompId: int option
    StatementEstRows: float option
    StatementId: int option
    StatementOptmLevel: string option
    StatementOptmEarlyAbortReason: string option
    CardinalityEstimationModelVersion: string option
    StatementSubTreeCost: float option
    StatementText: string option
    StatementType: string option
    QueryHash: string option
    QueryPlanHash: string option
    RetrievedFromCache: string option
    StatementSqlHandle: string option
    SecurityPolicyApplied: bool option
    BatchModeOnRowStoreUsed: bool option
}

type ExternalDistributedComputationType = {
    EdcShowplanXml: string
}

type StmtSimpleType = {
    BaseInfo: BaseStmtInfoType
    QueryPlan: QueryPlanType option
}

type CursorOperationType = {
    OperationType: string // "FetchQuery" | "PopulateQuery" | "RefreshQuery"
    QueryPlan: QueryPlanType
}

type StmtCursorType = {
    BaseInfo: BaseStmtInfoType
    CursorName: string option
    CursorActualType: CursorType option
    CursorRequestedType: CursorType option
    CursorConcurrency: string option // "Read Only" | "Pessimistic" | "Optimistic"
    ForwardOnly: bool option
    Operations: CursorOperationType list
}

type ReceiveOperationType = {
    OperationType: string // "ReceivePlanSelect" | "ReceivePlanUpdate"
    QueryPlan: QueryPlanType
}

type StmtReceiveType = {
    BaseInfo: BaseStmtInfoType
    Operations: ReceiveOperationType list
}

type StmtUseDbType = {
    BaseInfo: BaseStmtInfoType
    Database: string
}

// <xsd:complexType name="FunctionType">
// 	<xsd:annotation>
// 		<xsd:documentation>Shows the plan for the UDF or stored procedure</xsd:documentation>
// 	</xsd:annotation>
// 	<xsd:sequence>
// 		<xsd:element name="Statements" type="shp:StmtBlockType" />
// 	</xsd:sequence>
// 	<xsd:attribute name="ProcName" type="xsd:string" />
// 	<xsd:attribute name="IsNativelyCompiled" type="xsd:boolean" use="optional" />
// </xsd:complexType>

/// Shows the plan for the UDF or stored procedure
type FunctionType = {
    Statements : StmtBlockType list

    ProcName : string
    IsNativelyCompiled : bool option
}

and StmtCondType = {
    BaseInfo: BaseStmtInfoType
    Condition: {| QueryPlan : QueryPlanType option; UDFs : FunctionType list |}
    Then: StmtBlockType
    Else: StmtBlockType option
}
and StmtBlockType =
    | StmtSimple of StmtSimpleType
    | StmtCond of StmtCondType
    | StmtCursor of StmtCursorType
    | StmtReceive of StmtReceiveType
    | StmtUseDb of StmtUseDbType
    | ExternalDistributedComputation of ExternalDistributedComputationType
and  Batch = {
    Statements: StmtBlockType list
}

type BatchSequence = {
    Batches: Batch list
}

// ========================================
// Root Element
// ========================================

type ShowPlanXML = {
    Version: string
    Build: string
    ClusteredMode: bool option
    BatchSequence: BatchSequence
}



