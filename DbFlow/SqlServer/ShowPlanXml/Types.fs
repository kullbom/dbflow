namespace DbFlow.SqlServer.Experimental.ShowPlanXml

// ========================================
// Basic Types
// ========================================

/// The set options that affects query cost
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
    OnlineInbuildIndex: int option
    OnlineIndexBuildMappingIndex: int option
    GraphWorkTableType: int option
    GraphWorkTableIdentifier: int option
}

type InternalInfoType = {
    // Arbitrary XML content - can contain any child elements or attributes
    Content: string option
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

// Spill Warning for last query plan stats
type SpillOccurredType = {
    Detail: bool option
}

// Spill warning information
type SpillToTempDbType = {
    SpillLevel : uint64 option
    SpilledThreadCount : uint64 option
}

// Sort spill details
type SortSpillDetailsType = {
    GrantedMemoryKb : uint64 option
    UsedMemoryKb : uint64 option
    WritesToTempDb : uint64 option
    ReadsFromTempDb : uint64 option
}

// Hash spill details
type HashSpillDetailsType = {
    GrantedMemoryKb : uint64 option
    UsedMemoryKb : uint64 option
    WritesToTempDb : uint64 option
    ReadsFromTempDb : uint64 option
}

// Exchange spill details
type ExchangeSpillDetailsType = {
    WritesToTempDb : uint64 option
}

// Query wait information
type WaitWarningType = {
    WaitType : string // enum: "Memory Grant"
    WaitTime : uint64 option
}

/// Provide warning information for memory grant.
/// GrantWarningKind: Warning kind
/// RequestedMemory: Initial grant request in KB
/// GrantedMemory: Granted memory in KB
/// MaxUsedMemory: Maximum used memory grant in KB
type MemoryGrantWarningInfo = {
    GrantWarningKind : string
    RequestedMemory : uint64
    GrantedMemory : uint64
    MaxUsedMemory : uint64
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
    PartitionRange: PartitionRangeType list
    PartitionCount: uint64
}

type RunTimePartitionSummaryType = {
    PartitionsAccessed: PartitionsAccessedType
}

type WaitStatType = {
    WaitType: string
    WaitTimeMs: uint64
    WaitCount: uint64
}

type QueryExecTimeType = {
    CpuTime: uint64
    ElapsedTime: uint64
    UdfCpuTime: uint64 option
    UdfElapsedTime: uint64 option
}

type OptimizationReplayType = {
    Script: string
}

type ThreadReservationType = {
    NodeId: int option
    ReservedThreads: int
}

type ThreadStatType = {
    Branches: int
    UsedThreads: int option
    ThreadReservations: ThreadReservationType list
}

type MissingColumnType = {
    Name: string
    ColumnId: int
}

type ColumnGroupType = {
    Usage: string // "EQUALITY" | "INEQUALITY" | "INCLUDE"
    Columns: MissingColumnType list
}

type MissingIndexType = {
    Database: string
    Schema: string
    Table: string
    ColumnGroups: ColumnGroupType list
}

type MissingIndexGroupType = {
    Impact: float
    MissingIndexes: MissingIndexType list
}

type MissingIndexesType = {
    MissingIndexGroups: MissingIndexGroupType list
}

type GuessedSelectivityType = {
    Spatial: ObjectType
}

type ParameterizationType = {
    Objects: ObjectType list
}

type UnmatchedIndexesType = {
    Parameterization: ParameterizationType
}

type TraceFlag = {
    Value: uint64
    Scope: string // "Global" | "Session"
}

type TraceFlagListType = {
    IsCompileTime: bool
    TraceFlags: TraceFlag list
}

type ResourceEstimateType = {
    NodeCount: uint64 option
    Dop: float option
    MemoryInBytes: float option
    DiskWrittenInBytes: float option
    Scalable: bool option
}

// ========================================
// Column and Scalar Expression Foundation Types
// ========================================
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

and ScalarOperatorKind =
    | Aggregate of AggregateType
    | Arithmetic of ArithmeticType
    | Assign of AssignType
    | Compare of CompareType
    | Const of ConstType
    | Convert of ConvertType
    | Identifier of IdentType
    | IF of ConditionalType
    | Intrinsic of IntrinsicType
    | Logical of LogicalType
    | MultipleAssign of MultAssignType
    | ScalarExpressionList of ScalarExpressionListType
    | Sequence of ScalarSequenceType
    | Subquery of SubqueryType
    | UDTMethod of UDTMethodType
    | UserDefinedAggregate of UDAggregateType
    | UserDefinedFunction of UDFType

and ScalarType = {
    Kind: ScalarOperatorKind
    ScalarString: string option
    InternalInfo: InternalInfoType option
}

and ScalarExpressionType = {
    ScalarOperator: ScalarType
}

and ScalarExpressionListType = {
    ScalarOperators: ScalarType list // 1+ items
}

// ========================================
// Defined Value Types
// ========================================
and DefinedValueTypeValue =
    | ColumnReferences of ColumnReferenceType list // 1..n
    | ScalarOperator of ScalarType

and DefinedValueType = {
    // ValueVector or ColumnReference
    Defined: ColumnReferenceType list  
    /// "unbounded for union case"
    Value: DefinedValueTypeValue option 
}

and DefinedValuesListType = {
    DefinedValues: DefinedValueType list
}

// ========================================
// Seek Predicate Types
// ========================================
and ScanRangeType = {
    RangeColumns: ColumnReferenceType list
    RangeExpressions: ScalarExpressionListType
    ScanType: CompareOpType
}

and SeekPredicateType = {
    Prefix: ScanRangeType option
    StartRange: ScanRangeType option
    EndRange: ScanRangeType option
    IsNotNull: ColumnReferenceType option
}

and SeekPredicateNewType = {
    SeekKeys: SeekPredicateType list // 1-2
}

and SeekPredicatePartType = {
    SeekPredicatesNew: SeekPredicateNewType list
}

and SeekPredicatesType =
    | SeekPredicate of SeekPredicateType list
    | SeekPredicateNew of SeekPredicateNewType list
    | SeekPredicatePart of SeekPredicatePartType list

and RelOpSeekPredicate = 
    | SeekPredicate of SeekPredicateType
    | SeekPredicateNew of SeekPredicateNewType

// ========================================
// Scalar Operator Detail Types
// ========================================
and ConstType = {
    ConstValue: string
}

and IdentType = {
    ColumnReference: ColumnReferenceType option
    Table: string option
}

and CompareType = {
    CompareOp: CompareOpType
    ScalarOperators: ScalarType list // 1-2 items
}

and ConvertType = {
    DataType: string
    Length: int option
    Precision: int option
    Scale: int option
    Style: int
    Implicit: bool
    StyleExpression: ScalarExpressionType option
    ScalarOperator: ScalarType
}

and ArithmeticType = {
    Operation: ArithmeticOperationType
    ScalarOperators: ScalarType list // 1-2 items
}

and LogicalType = {
    Operation: LogicalOperationType
    ScalarOperators: ScalarType list // 1+ items
}

and AggregateType = {
    AggType: string
    Distinct: bool
    ScalarOperators: ScalarType list
}

and UDAggregateType = {
    Distinct: bool
    UDAggObject: ObjectType option
    ScalarOperators: ScalarType list
}

and MultAssignType = {
    Assigns: AssignType list
}

and ConditionalType = {
    Condition: ScalarExpressionType
    Then: ScalarExpressionType
    Else: ScalarExpressionType
}

and IntrinsicType = {
    FunctionName: string
    ScalarOperators: ScalarType list
}

and ScalarSequenceType = {
    FunctionName: string
}

and CLRFunctionType = {
    // Attributes
    Assembly: string option
    Class: string
    Method: string option
}

and UDFType = {
    FunctionName: string
    IsClrFunction: bool option
    CLRFunction: CLRFunctionType option
    ScalarOperators: ScalarType list
}

and UDTMethodType = {
    CLRFunction: CLRFunctionType option
    ScalarOperators: ScalarType list
}

and SubqueryType = {
    ScalarOperator: ScalarType option
    RelOp: RelOpType
    // Attributes
    Operation: SubqueryOperationType
}

and AssignType = {
    Target: AssignTargetType
    ScalarOperator: ScalarType
    SourceColumns: ColumnReferenceType list
    TargetColumns: ColumnReferenceType list
}

and AssignTargetType =
    | ColumnRef of ColumnReferenceType
    | ScalarOp of ScalarType

// ========================================
// Parameter Sensitive Predicates
// ========================================
and ParameterSensitivePredicateType = {
    StatisticsInfo: StatsInfoType list // 1 .. unbounded
    Predicate: ScalarExpressionType
    // Attributes
    LowBoundery : float
    HighBoundary : float
}

and DispatcherType = {
    ParameterSensitivePredicate : ParameterSensitivePredicateType list // 1..3 
}

// ========================================
// Warning Types
// ========================================
and Warning =
    | SpillOccurred of SpillOccurredType
    | ColumnsWithNoStatistics of ColumnReferenceType list
    | ColumnsWithStaleStatistics of ColumnReferenceType list
    | SpillToTempDb of SpillToTempDbType
    | Wait of WaitWarningType
    | PlanAffectingConvert of AffectingConvertWarningType 
    | SortSpillDetails of SortSpillDetailsType
    | HashSpillDetails of HashSpillDetailsType
    | ExchangeSpillDetails of ExchangeSpillDetailsType
    | MemoryGrantWarning of MemoryGrantWarningInfo

and WarningsType = {
    Warnings: Warning list
    NoJoinPredicate: bool option
    SpatialGuess: bool option
    UnmatchedIndexes: bool option
    FullUpdateForOnlineIndexBuild: bool option
}

// ========================================
// Relational Operator Base Types
// ========================================
and RelOpBaseType = {
    DefinedValues : DefinedValuesListType option
    InternalInfo : InternalInfoType option
}

and RelOpType = {
    // Sequence elements
    OutputList: ColumnReferenceType list
    Warnings: WarningsType option
    MemoryFractions: MemoryFractionsType option
    RunTimeInformation: RunTimeInformationType option
    RunTimePartitionSummary: RunTimePartitionSummaryType option
    InternalInfo: InternalInfoType option
    OperatorDetails: RelOpDetails option

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
// Relational Operator Detail Types - Simple Base
// ========================================
and SimpleIteratorOneChildType = {
    Base: RelOpBaseType
    RelOp: RelOpType
}

and FilterType = {
    Base: RelOpBaseType
    RelOp: RelOpType
    Predicate: ScalarExpressionType
    StartupExpression: bool
}

and ConstantScanType = {
    Base: RelOpBaseType
    Values: ScalarExpressionListType list
}

and RowsetType = {
    Base: RelOpBaseType
    Objects: ObjectType list
}

and IndexedViewInfoType = {
    Objects: ObjectType list
}

and OrderByType = {
    OrderByColumns: OrderByColumnType list
}

and OrderByColumnType = {
    ColumnReference: ColumnReferenceType
    Ascending: bool
}

// ========================================
// Relational Operator Detail Types - Scan Operations
// ========================================
and TableScanType = {
    RowsetBase: RowsetType
    Predicate: ScalarExpressionType option
    PartitionId: ColumnReferenceType option
    IndexedViewInfo: IndexedViewInfoType option
    Ordered: bool
    ForcedIndex: bool option
    ForceScan: bool option
    NoExpandHint: bool option
    Storage: StorageType option
}

and IndexScanType = {
    RowsetBase: RowsetType
    SeekPredicates: SeekPredicatesType option
    Predicates: ScalarExpressionType list
    PartitionId: ColumnReferenceType option
    IndexedViewInfo: IndexedViewInfoType option
    Lookup: bool option
    Ordered: bool
    ScanDirection: OrderType option
    ForcedIndex: bool option
    ForceSeek: bool option
    ForceSeekColumnCount: int option
    ForceScan: bool option
    NoExpandHint: bool option
    Storage: StorageType option
    DynamicSeek: bool option
    SBSFileUrl: string option
}

and XcsScanType = {
    RowsetBase: RowsetType
    Predicate: ScalarExpressionType option
    PartitionId: ColumnReferenceType option
    IndexedViewInfo: IndexedViewInfoType option
    RelOp: RelOpType
    Ordered: bool
    ForcedIndex: bool option
    ForceScan: bool option
    NoExpandHint: bool option
    Storage: StorageType option
}

and TableValuedFunctionType = {
    Base: RelOpBaseType
    Object: ObjectType option
    Predicate: ScalarExpressionType option
    RelOp: RelOpType option
    ParameterList: ScalarExpressionListType option
}

// ========================================
// Relational Operator Detail Types - Join and Hash Operations
// ========================================
and StarJoinInfoType = {
    Root: bool option
    OperationType: string // "Fetch" | "Index Intersection" | "Index Filter" | "Index Lookup"
}

and HashType = {
    Base: RelOpBaseType
    HashKeysBuild: ColumnReferenceType list option
    HashKeysProbe: ColumnReferenceType list option
    BuildResidual: ScalarExpressionType option
    ProbeResidual: ScalarExpressionType option
    StarJoinInfo: StarJoinInfoType option
    RelOps: RelOpType list // 1-2
    BitmapCreator: bool option
}

and JoinType = {
    Base: RelOpBaseType
    Predicates: ScalarExpressionType list
    Probes: ColumnReferenceType list
    RelOps: RelOpType list
}

and NestedLoopsType = {
    Base: RelOpBaseType
    Predicate: ScalarExpressionType option
    PassThru: ScalarExpressionType option
    OuterReferences: ColumnReferenceType list option
    PartitionId: ColumnReferenceType option
    ProbeColumn: ColumnReferenceType option
    StarJoinInfo: StarJoinInfoType option
    RelOps: RelOpType list // 2
    Optimized: bool
    WithOrderedPrefetch: bool option
    WithUnorderedPrefetch: bool option
}

and MergeType = {
    Base: RelOpBaseType
    InnerSideJoinColumns: ColumnReferenceType list option
    OuterSideJoinColumns: ColumnReferenceType list option
    Residual: ScalarExpressionType option
    PassThru: ScalarExpressionType option
    StarJoinInfo: StarJoinInfoType option
    RelOps: RelOpType list // 2
    ManyToMany: bool option
}

and AdaptiveJoinType = {
    Base: RelOpBaseType
    HashKeysBuild: ColumnReferenceType list option
    HashKeysProbe: ColumnReferenceType list option
    BuildResidual: ScalarExpressionType option
    ProbeResidual: ScalarExpressionType option
    StarJoinInfo: StarJoinInfoType option
    Predicate: ScalarExpressionType option
    PassThru: ScalarExpressionType option
    OuterReferences: ColumnReferenceType list option
    PartitionId: ColumnReferenceType option
    RelOps: RelOpType list // 3
    BitmapCreator: bool option
    Optimized: bool
    WithOrderedPrefetch: bool option
    WithUnorderedPrefetch: bool option
}

// ========================================
// Relational Operator Detail Types - Aggregation Operations
// ========================================
and RollupLevelType = {
    Level: int
}

and RollupInfoType = {
    RollupLevels: RollupLevelType list // 2+
    HighestLevel: int
}

and StreamAggregateType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceType list option
    RollupInfo: RollupInfoType option
    RelOp: RelOpType
}

and GroupingSetReferenceType = {
    Value: string
}

and GroupingSetListType = {
    GroupingSets: GroupingSetReferenceType list
}

and GbAggType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceType list option
    AggFunctions: DefinedValuesListType option
    RelOps: RelOpType list
    IsScalar: bool option
    AggType: string option
    HintType: string option
}

and GbApplyType = {
    Base: RelOpBaseType
    Predicates: ScalarExpressionType list
    AggFunctions: DefinedValuesListType option
    RelOps: RelOpType list
    JoinType: string option
    AggType: string option
}

and LocalCubeType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceType list option
    GroupingSets: GroupingSetListType option
    RelOps: RelOpType list
}

// ========================================
// Relational Operator Detail Types - Sorting and Deduplication
// ========================================
and SortType = {
    Base: RelOpBaseType
    OrderBy: OrderByType
    PartitionId: ColumnReferenceType option
    RelOp: RelOpType
    Distinct: bool
}

and TopType = {
    Base: RelOpBaseType
    TieColumns: ColumnReferenceType list option
    OffsetExpression: ScalarExpressionType option
    TopExpression: ScalarExpressionType option
    RelOp: RelOpType
    RowCount: bool option
    Rows: int option
    IsPercent: bool option
    WithTies: bool option
    TopLocation: string option
}

and TopSortType = {
    SortBase: SortType
    Rows: int
    WithTies: bool option
}

// ========================================
// Relational Operator Detail Types - Set Operations
// ========================================
and ConcatType = {
    Base: RelOpBaseType
    RelOps: RelOpType list // 2+
}

and SwitchType = {
    ConcatBase: ConcatType
    Predicate: ScalarExpressionType option
}

and BitmapType = {
    Base: RelOpBaseType
    HashKeys: ColumnReferenceType list
    RelOp: RelOpType
}

and CollapseType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceType list
    RelOp: RelOpType
}

and SequenceType = {
    Base: RelOpBaseType
    RelOps: RelOpType list // 2+
    IsGraphDBTransitiveClosure: bool option
    GraphSequenceIdentifier: int option
}

// ========================================
// Relational Operator Detail Types - Misc Operations
// ========================================
and ComputeScalarType = {
    Base: RelOpBaseType
    RelOp: RelOpType
    ComputeSequence: bool option
}

and SegmentType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceType list
    SegmentColumn: ColumnReferenceType
    RelOp: RelOpType
}

and SplitType = {
    Base: RelOpBaseType
    ActionColumn: ColumnReferenceType option
    RelOp: RelOpType
}

and UDXType = {
    Base: RelOpBaseType
    UsedUDXColumns: ColumnReferenceType list option
    RelOp: RelOpType option
    UDXName: string
}

and WindowType = {
    Base: RelOpBaseType
    RelOp: RelOpType option
}

and WindowAggregateType = {
    Base: RelOpBaseType
    RelOp: RelOpType option
}

// ========================================
// Relational Operator Detail Types - Parallelism
// ========================================
and ActivationInfoType = {
    Object: ObjectType option

    Type: string
    FragmentElimination: string option
}

and BrickRoutingType = {
    Object: ObjectType option
    FragmentIdColumn: ColumnReferenceType option
}

and ParallelismType = {
    Base: RelOpBaseType
    PartitionColumns: ColumnReferenceType list option
    OrderBy: OrderByType option
    HashKeys: ColumnReferenceType list option
    ProbeColumn: ColumnReferenceType option
    Predicate: ScalarExpressionType option
    Activation: ActivationInfoType option
    BrickRouting: BrickRoutingType option
    RelOp: RelOpType
    PartitioningType: PartitionType option
    Remoting: bool option
    LocalParallelism: bool option
    InRow: bool option
}

// ========================================
// Relational Operator Detail Types - Update Operations
// ========================================
and AssignmentMapType = {
    Assigns: AssignType list
}

and SetPredicateElementType = {
    ScalarOperator: ScalarType

    SetPredicateType: string option // "Update" | "Insert"
}

and OutputColumnsType = {
    DefinedValues: DefinedValuesListType option
    Objects: ObjectType list
}

and SimpleUpdateType = {
    RowsetBase: RowsetType
    SeekPredicate: RelOpSeekPredicate option
    SetPredicate: ScalarExpressionType option
    DMLRequestSort: bool option
}

and UpdateType = {
    RowsetBase: RowsetType
    SetPredicates: SetPredicateElementType list
    ProbeColumn: ColumnReferenceType option
    ActionColumn: ColumnReferenceType option
    OriginalActionColumn: ColumnReferenceType option
    AssignmentMap: AssignmentMapType option
    SourceTable: ParameterizationType option
    TargetTable: ParameterizationType option
    RelOp: RelOpType
    WithOrderedPrefetch: bool option
    WithUnorderedPrefetch: bool option
    DMLRequestSort: bool option
}

and DMLOpType = {
    Base: RelOpBaseType
    AssignmentMap: AssignmentMapType option
    SourceTable: ParameterizationType option
    TargetTable: ParameterizationType option
    RelOps: RelOpType list
}

and ScalarInsertType = {
    RowsetBase: RowsetType
    SetPredicate: ScalarExpressionType option
    DMLRequestSort: bool option
}

and CreateIndexType = {
    RowsetBase: RowsetType
    RelOp: RelOpType
}

// ========================================
// Relational Operator Detail Types - Spool Operations
// ========================================
and SpoolType = {
    Base: RelOpBaseType
    SeekPredicate: RelOpSeekPredicate option
    RelOp: RelOpType option
    Stack: bool option
    PrimaryNodeId: int option
}

and BatchHashTableBuildType = {
    Base: RelOpBaseType
    RelOp: RelOpType
    BitmapCreator: bool option
}

// ========================================
// Relational Operator Detail Types - Remote Operations
// ========================================
and RemoteType = {
    Base: RelOpBaseType
    RemoteDestination: string option
    RemoteSource: string option
    RemoteObject: string option
}

and RemoteRangeType = {
    RemoteBase: RemoteType
    SeekPredicates: SeekPredicatesType option
}

and RemoteFetchType = {
    RemoteBase: RemoteType
    RelOp: RelOpType
}

and RemoteModifyType = {
    RemoteBase: RemoteType
    SetPredicate: ScalarExpressionType option
    RelOp: RelOpType
}

and RemoteQueryType = {
    RemoteBase: RemoteType
    RemoteQuery: string option
}

and PutType = {
    RemoteBase: RemoteQueryType
    RelOp: RelOpType option
    IsExternallyComputed: bool option
    ShuffleType: string option
    ShuffleColumn: string option
}

// ========================================
// Relational Operator Detail Types - Other Operations
// ========================================
and GetType = {
    Base: RelOpBaseType
    Bookmarks: ColumnReferenceType list option
    OutputColumns: OutputColumnsType option
    GeneratedData: ScalarExpressionListType option
    RelOps: RelOpType list
    NumRows: int option
    IsExternal: bool option
    IsDistributed: bool option
    IsHashDistributed: bool option
    IsReplicated: bool option
    IsRoundRobin: bool option
}

and ProjectType = {
    Base: RelOpBaseType
    RelOps: RelOpType list
    IsNoOp: bool option
}

and GenericType = {
    Base: RelOpBaseType
    RelOps: RelOpType list
}

and MoveType = {
    Base: RelOpBaseType
    DistributionKey: ColumnReferenceType list option
    ResourceEstimate: ResourceEstimateType option
    RelOps: RelOpType list
    MoveType: string option
    DistributionType: string option
    IsDistributed: bool option
    IsExternal: bool option
    IsFull: bool option
}

and ExternalSelectType = {
    Base: RelOpBaseType
    RelOps: RelOpType list
    MaterializeOperation: string option
    DistributionType: string option
    IsDistributed: bool option
    IsExternal: bool option
    IsFull: bool option
}

and ForeignKeyReferenceCheckType = {
    IndexScan: IndexScanType
}

and ForeignKeyReferencesCheckType = {
    Base: RelOpBaseType
    RelOp: RelOpType
    ForeignKeyReferenceChecks: ForeignKeyReferenceCheckType list
    ForeignKeyReferencesCount: int option
    NoMatchingIndexCount: int option
    PartialMatchingIndexCount: int option
}

// ========================================
// Relational Operator Details Discriminated Union
// ========================================
and RelOpDetails =
    | AdaptiveJoin of AdaptiveJoinType
    | Apply of JoinType
    | Assert of FilterType
    | BatchHashTableBuild of BatchHashTableBuildType
    | Bitmap of BitmapType
    | Collapse of CollapseType
    | ComputeScalar of ComputeScalarType
    | Concat of ConcatType
    | ConstantScan of ConstantScanType
    | ConstTableGet of GetType
    | CreateIndex of CreateIndexType
    | Delete of DMLOpType
    | DeletedScan of RowsetType
    | Extension of UDXType
    | ExternalSelect of ExternalSelectType
    | ExtExtractScan of RemoteType
    | Filter of FilterType
    | ForeignKeyReferencesCheck of ForeignKeyReferencesCheckType
    | GbAgg of GbAggType
    | GbApply of GbApplyType
    | Generic of GenericType
    | Get of GetType
    | Hash of HashType
    | IndexScan of IndexScanType
    | InsertedScan of RowsetType
    | Insert of DMLOpType
    | Join of JoinType
    | LocalCube of LocalCubeType
    | LogRowScan of RelOpBaseType
    | Merge of MergeType
    | MergeInterval of SimpleIteratorOneChildType
    | Move of MoveType
    | NestedLoops of NestedLoopsType
    | OnlineIndex of CreateIndexType
    | Parallelism of ParallelismType
    | ParameterTableScan of RelOpBaseType
    | PrintDataflow of RelOpBaseType
    | Project of ProjectType
    | Put of PutType
    | RemoteFetch of RemoteFetchType
    | RemoteModify of RemoteModifyType
    | RemoteQuery of RemoteQueryType
    | RemoteRange of RemoteRangeType
    | RemoteScan of RemoteType
    | RowCountSpool of SpoolType
    | ScalarInsert of ScalarInsertType
    | Segment of SegmentType
    | Sequence of SequenceType
    | SequenceProject of ComputeScalarType
    | SimpleUpdate of SimpleUpdateType
    | Sort of SortType
    | Split of SplitType
    | Spool of SpoolType
    | StreamAggregate of StreamAggregateType
    | Switch of SwitchType
    | TableScan of TableScanType
    | TableValuedFunction of TableValuedFunctionType
    | Top of TopType
    | TopSort of TopSortType
    | Update of UpdateType
    | Union of ConcatType
    | UnionAll of ConcatType
    | WindowSpool of WindowType
    | WindowAggregate of WindowAggregateType
    | XcsScan of XcsScanType

// ========================================
// Runtime Information
// ========================================
and RunTimeCountersPerThread = {
    Thread: int
    BrickId: int option
    ActualRebinds: uint64 option
    ActualRewinds: uint64 option
    ActualRows: uint64
    RowRequalifications: uint64 option
    ActualRowsRead: uint64 option
    Batches: uint64 option
    ActualEndOfScans: uint64
    ActualExecutions: uint64
    ActualExecutionMode: ExecutionModeType option
    TaskAddr: uint64 option
    SchedulerId: uint64 option
    FirstActiveTime: uint64 option
    LastActiveTime: uint64 option
    OpenTime: uint64 option
    FirstRowTime: uint64 option
    LastRowTime: uint64 option
    CloseTime: uint64 option
    ActualElapsedms: uint64 option
    ActualCPUms: uint64 option
    ActualScans: uint64 option
    ActualLogicalReads: uint64 option
    ActualPhysicalReads: uint64 option
    ActualPageServerReads: uint64 option
    ActualReadAheads: uint64 option
    ActualPageServerReadAheads: uint64 option
    ActualLobLogicalReads: uint64 option
    ActualLobPhysicalReads: uint64 option
    ActualLobPageServerReads: uint64 option
    ActualLobReadAheads: uint64 option
    ActualLobPageServerReadAheads: uint64 option
    SegmentReads: int option
    SegmentSkips: int option
    ActualLocallyAggregatedRows: uint64 option
    InputMemoryGrant: uint64 option
    OutputMemoryGrant: uint64 option
    UsedMemoryGrant: uint64 option
    IsInterleavedExecuted: bool option
    ActualJoinType: PhysicalOpType option
    HpcRowCount: uint64 option
    HpcKernelElapsedUs: uint64 option
    HpcHostToDeviceBytes: uint64 option
    HpcDeviceToHostBytes: uint64 option
    ActualPageServerPushedPageIDs: uint64 option
    ActualPageServerRowsReturned: uint64 option
    ActualPageServerRowsRead: uint64 option
    ActualPageServerPushedReads: uint64 option
}

and RunTimeInformationType = {
    RunTimeCountersPerThread: RunTimeCountersPerThread list
}

// ========================================
// Query Plan
// ========================================

and QueryPlanType = {
    DegreeOfParallelism: int option
    EffectiveDegreeOfParallelism: int option
    NonParallelPlanReason: string option
    DOPFeedbackAdjusted: string option
    MemoryGrant: uint64 option
    CachedPlanSize: uint64 option
    CompileTime: uint64 option
    CompileCPU: uint64 option
    CompileMemory: uint64 option
    UsePlan: bool option
    ContainsInterleavedExecutionCandidates: bool option
    ContainsInlineScalarTsqlUdfs: bool option
    QueryVariantID: int option
    DispatcherPlanHandle: string option
    ExclusiveProfileTimeActive: bool option
    InternalInfo: InternalInfoType option
    OptimizationReplay: OptimizationReplayType option
    ThreadStat: ThreadStatType option
    MissingIndexes: MissingIndexesType option
    GuessedSelectivity: GuessedSelectivityType option
    UnmatchedIndexes: UnmatchedIndexesType option
    Warnings: WarningsType option
    MemoryGrantInfo: MemoryGrantType option
    OptimizerHardwareDependentProperties: OptimizerHardwareDependentPropertiesType option
    OptimizerStatsUsage: OptimizerStatsUsageType option
    TraceFlags: TraceFlagListType list
    WaitStats: WaitStatType list option
    QueryTimeStats: QueryExecTimeType option
    RelOp: RelOpType
    ParameterList: ColumnReferenceType list option
}

// ========================================
// Statement Types
// ========================================

and BaseStmtInfoType = {
    StatementSetOptions: SetOptionsType option
    StatementCompId: int option
    StatementEstRows: float option
    StatementId: int option
    QueryCompilationReplay: int option
    StatementOptmLevel: string option
    StatementOptmEarlyAbortReason: string option
    CardinalityEstimationModelVersion: string option
    StatementSubTreeCost: float option
    StatementText: string option
    StatementType: string option
    TemplatePlanGuideDB: string option
    TemplatePlanGuideName: string option
    PlanGuideDB: string option
    PlanGuideName: string option
    ParameterizedText: string option
    ParameterizedPlanHandle: string option
    QueryHash: string option
    QueryPlanHash: string option
    RetrievedFromCache: string option
    StatementSqlHandle: string option
    DatabaseContextSettingsId: uint64 option
    ParentObjectId: uint64 option
    BatchSqlHandle: string option
    StatementParameterizationType: int option
    SecurityPolicyApplied: bool option
    BatchModeOnRowStoreUsed: bool option
    QueryStoreStatementHintId: uint64 option
    QueryStoreStatementHintText: string option
    QueryStoreStatementHintSource: string option
    ContainsLedgerTables: bool option
}
/// This is only found in the serialized xml for Gen3 external distributed statements.
and ExternalDistributedComputationType = {
    EdcShowplanXml: string
}

and StmtSimpleType = {
    BaseInfo: BaseStmtInfoType
    Dispatcher: DispatcherType option
    QueryPlan: QueryPlanType option
    UDFs: FunctionType list
    StoredProc: FunctionType option
}

and CursorOperationType = {
    OperationType: string // "FetchQuery" | "PopulateQuery" | "RefreshQuery"
    Dispatcher: DispatcherType option
    QueryPlan: QueryPlanType
    UDFs: FunctionType list
}

and StmtCursorType = {
    BaseInfo: BaseStmtInfoType
    CursorName: string option
    CursorActualType: CursorType option
    CursorRequestedType: CursorType option
    CursorConcurrency: string option // "Read Only" | "Pessimistic" | "Optimistic"
    ForwardOnly: bool option
    Operations: CursorOperationType list
}

and ReceivePlanDetailType = {
    OperationType: ReceivePlanOperationType 
    QueryPlan: QueryPlanType
}

and StmtReceiveType = {
    BaseInfo: BaseStmtInfoType
    ReceivePlans: ReceivePlanDetailType list
}

and StmtUseDbType = {
    BaseInfo: BaseStmtInfoType
    Database: string
}

/// Shows the plan for the UDF or stored procedure
and FunctionType = {
    Statements : StmtBlockType list
    ProcName : string
    IsNativelyCompiled : bool option
}

and StmtCondition = {
    QueryPlan : QueryPlanType option
    UDFs : FunctionType list
}
/// Complex statement type that is constructed by a condition, a then clause and an optional else clause.
and StmtCondType = {
    BaseInfo: BaseStmtInfoType
    Condition: StmtCondition
    Then: StmtBlockType list
    Else: StmtBlockType list option
}

and StmtBlockType =
    | ExternalDistributedComputation of ExternalDistributedComputationType
    | StmtSimple of StmtSimpleType
    | StmtCond of StmtCondType
    | StmtCursor of StmtCursorType
    | StmtReceive of StmtReceiveType
    | StmtUseDb of StmtUseDbType

/// The statement block that contains many statements
and Batch = {
    Statements: StmtBlockType list
}

type BatchSequence = {
    Batches: Batch list
}

type ShowPlanXML = {
    Version: string
    Build: string
    ClusteredMode: bool option
    BatchSequence: BatchSequence
}
