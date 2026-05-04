namespace DbFlow.SqlServer.ShowPlanXml

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
    PartitionCount: uint64
    PartitionRange: PartitionRangeType list
}

type RunTimePartitionSummaryType = {
    PartitionsAccessed: PartitionsAccessedType
}

type WaitStatType = {
    WaitType: string
    WaitTimeMs: uint64
    WaitCount: uint64
}

type WaitStatListType = {
    WaitStats: WaitStatType list
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
// Mutually Recursive Types
// ========================================
type Warning =
    | SpillOccurred of SpillOccurredType
    | ColumnsWithNoStatistics of ColumnReferenceListType
    | ColumnsWithStaleStatistics of ColumnReferenceListType
    | SpillToTempDb of SpillToTempDbType
    | Wait of WaitWarningType
    | PlanAffectingConvert of AffectingConvertWarningType 
    | SortSpillDetails of SortSpillDetailsType
    | HashSpillDetails of HashSpillDetailsType
    | ExchangeSpillDetails of ExchangeSpillDetailsType
    | MemoryGrantWarning of MemoryGrantWarningInfo


// List of all possible iterator or query specific warnings (e.g. hash spilling, no join predicate)
and WarningsType = {
    Warnings: Warning list
    NoJoinPredicate: bool option
    SpatialGuess: bool option
    UnmatchedIndexes: bool option
    FullUpdateForOnlineIndexBuild: bool option
}

and ColumnReferenceType = {
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

// TODO : ta bort
and SingleColumnReferenceType = {
    ColumnReference: ColumnReferenceType
}
// TODO : ta bort
and ColumnReferenceListType = {
    ColumnReferences: ColumnReferenceType list
}

and DefinedValueType = {
    Value: ScalarType
    SourceColumns: ColumnReferenceListType option
    TargetColumns: ColumnReferenceListType option
}

and DefinedValuesListType = {
    DefinedValues: DefinedValueType list
}

// ========================================
// Seek Predicate Types
// ========================================

and ScanRangeType = {
    RangeColumns: ColumnReferenceListType
    RangeExpressions: ScalarExpressionListType
    ScanType: CompareOpType
}

and SeekPredicateType = {
    Prefix: ScanRangeType option
    StartRange: ScanRangeType option
    EndRange: ScanRangeType option
    IsNotNull: SingleColumnReferenceType option
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

/// This contains information related to the parameter sensitive predicate:
/// Boundaries used to determine different ranges;
/// Statistics information used to compute the boundaries;
/// Predicate details.
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
// Relational Operator Detail Types
// ========================================

and RelOpBaseType = {
    DefinedValues : DefinedValuesListType option
    InternalInfo : InternalInfoType option
}

and RelOpType = {
    // Sequence elements
    OutputList: ColumnReferenceListType
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
    //| LogRowScan
    | Merge of MergeType
    | MergeInterval of SimpleIteratorOneChildType
    | Move of MoveType
    | NestedLoops of NestedLoopsType
    | OnlineIndex of CreateIndexType
    | Parallelism of ParallelismType
    //| ParameterTableScan
    //| PrintDataflow
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

// Base types for specific operators
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

and TableScanType = {
    RowsetBase: RowsetType
    Predicate: ScalarExpressionType option
    PartitionId: SingleColumnReferenceType option
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
    PartitionId: SingleColumnReferenceType option
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
    PartitionId: SingleColumnReferenceType option
    IndexedViewInfo: IndexedViewInfoType option
    RelOp: RelOpType
    Ordered: bool
    ForcedIndex: bool option
    ForceScan: bool option
    NoExpandHint: bool option
    Storage: StorageType option
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

and TableValuedFunctionType = {
    Base: RelOpBaseType
    Object: ObjectType option
    Predicate: ScalarExpressionType option
    RelOp: RelOpType option
    ParameterList: ScalarExpressionListType option
}

and HashType = {
    Base: RelOpBaseType
    HashKeysBuild: ColumnReferenceListType option
    HashKeysProbe: ColumnReferenceListType option
    BuildResidual: ScalarExpressionType option
    ProbeResidual: ScalarExpressionType option
    StarJoinInfo: StarJoinInfoType option
    RelOps: RelOpType list // 1-2
    BitmapCreator: bool option
}

and ComputeScalarType = {
    Base: RelOpBaseType
    RelOp: RelOpType
    ComputeSequence: bool option
}

and ParallelismType = {
    Base: RelOpBaseType
    PartitionColumns: ColumnReferenceListType option
    OrderBy: OrderByType option
    HashKeys: ColumnReferenceListType option
    ProbeColumn: SingleColumnReferenceType option
    Predicate: ScalarExpressionType option
    Activation: ActivationInfoType option
    BrickRouting: BrickRoutingType option
    RelOp: RelOpType
    PartitioningType: PartitionType option
    Remoting: bool option
    LocalParallelism: bool option
    InRow: bool option
}

and ActivationInfoType = {
    Type: string
    Object: ObjectType option
    FragmentElimination: string option
}

and BrickRoutingType = {
    Object: ObjectType option
    FragmentIdColumn: SingleColumnReferenceType option
}

and StreamAggregateType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceListType option
    RollupInfo: RollupInfoType option
    RelOp: RelOpType
}

and RollupInfoType = {
    HighestLevel: int
    RollupLevels: RollupLevelType list
}

and RollupLevelType = {
    Level: int
}

and SortType = {
    Base: RelOpBaseType
    OrderBy: OrderByType
    PartitionId: SingleColumnReferenceType option
    RelOp: RelOpType
    Distinct: bool
}

and BitmapType = {
    Base: RelOpBaseType
    HashKeys: ColumnReferenceListType
    RelOp: RelOpType
}

and CollapseType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceListType
    RelOp: RelOpType
}

and ConcatType = {
    Base: RelOpBaseType
    RelOps: RelOpType list // 2+
}

and SwitchType = {
    ConcatBase: ConcatType
    Predicate: ScalarExpressionType option
}

and MergeType = {
    Base: RelOpBaseType
    InnerSideJoinColumns: ColumnReferenceListType option
    OuterSideJoinColumns: ColumnReferenceListType option
    Residual: ScalarExpressionType option
    PassThru: ScalarExpressionType option
    StarJoinInfo: StarJoinInfoType option
    RelOps: RelOpType list // 2
    ManyToMany: bool option
}

and NestedLoopsType = {
    Base: RelOpBaseType
    Predicate: ScalarExpressionType option
    PassThru: ScalarExpressionType option
    OuterReferences: ColumnReferenceListType option
    PartitionId: SingleColumnReferenceType option
    ProbeColumn: SingleColumnReferenceType option
    StarJoinInfo: StarJoinInfoType option
    RelOps: RelOpType list // 2
    Optimized: bool
    WithOrderedPrefetch: bool option
    WithUnorderedPrefetch: bool option
}

and SegmentType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceListType
    SegmentColumn: SingleColumnReferenceType
    RelOp: RelOpType
}

and SequenceType = {
    Base: RelOpBaseType
    RelOps: RelOpType list // 2+
    IsGraphDBTransitiveClosure: bool option
    GraphSequenceIdentifier: int option
}

and SplitType = {
    Base: RelOpBaseType
    ActionColumn: SingleColumnReferenceType option
    RelOp: RelOpType
}

and TopType = {
    Base: RelOpBaseType
    TieColumns: ColumnReferenceListType option
    OffsetExpression: ScalarExpressionType option
    TopExpression: ScalarExpressionType option
    RelOp: RelOpType
    RowCount: bool option
    Rows: int option
    IsPercent: bool option
    WithTies: bool option
    TopLocation: string option
}

and UDXType = {
    Base: RelOpBaseType
    UsedUDXColumns: ColumnReferenceListType option
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

and PutType = {
    RemoteBase: RemoteQueryType
    RelOp: RelOpType option
    IsExternallyComputed: bool option
    ShuffleType: string option
    ShuffleColumn: string option
}

and SimpleUpdateType = {
    RowsetBase: RowsetType
    SeekPredicate: (SeekPredicateType * SeekPredicateNewType) option
    SetPredicate: ScalarExpressionType option
    DMLRequestSort: bool option
}

and UpdateType = {
    RowsetBase: RowsetType
    SetPredicates: SetPredicateElementType list
    ProbeColumn: SingleColumnReferenceType option
    ActionColumn: SingleColumnReferenceType option
    OriginalActionColumn: SingleColumnReferenceType option
    AssignmentMap: AssignmentMapType option
    SourceTable: ParameterizationType option
    TargetTable: ParameterizationType option
    RelOp: RelOpType
    WithOrderedPrefetch: bool option
    WithUnorderedPrefetch: bool option
    DMLRequestSort: bool option
}

and SetPredicateElementType = {
    SetPredicateType: string option // "Update" | "Insert"
    ScalarExpression: ScalarType
}

and CreateIndexType = {
    RowsetBase: RowsetType
    RelOp: RelOpType
}

and SpoolType = {
    Base: RelOpBaseType
    SeekPredicate: (SeekPredicateType * SeekPredicateNewType) option
    RelOp: RelOpType option
    Stack: bool option
    PrimaryNodeId: int option
}

and BatchHashTableBuildType = {
    Base: RelOpBaseType
    RelOp: RelOpType
    BitmapCreator: bool option
}

and ScalarInsertType = {
    RowsetBase: RowsetType
    SetPredicate: ScalarExpressionType option
    DMLRequestSort: bool option
}

and TopSortType = {
    SortBase: SortType
    Rows: int
    WithTies: bool option
}

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

and GenericType = {
    Base: RelOpBaseType
    RelOps: RelOpType list
}

and MoveType = {
    Base: RelOpBaseType
    DistributionKey: ColumnReferenceListType option
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

and ProjectType = {
    Base: RelOpBaseType
    RelOps: RelOpType list
    IsNoOp: bool option
}

and JoinType = {
    Base: RelOpBaseType
    Predicates: ScalarExpressionType list
    Probes: SingleColumnReferenceType list
    RelOps: RelOpType list
}

and GbApplyType = {
    Base: RelOpBaseType
    Predicates: ScalarExpressionType list
    AggFunctions: DefinedValuesListType option
    RelOps: RelOpType list
    JoinType: string option
    AggType: string option
}

and GbAggType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceListType option
    AggFunctions: DefinedValuesListType option
    RelOps: RelOpType list
    IsScalar: bool option
    AggType: string option
    HintType: string option
}

and GroupingSetReferenceType = {
    Value: string
}

and GroupingSetListType = {
    GroupingSets: GroupingSetReferenceType list
}

and LocalCubeType = {
    Base: RelOpBaseType
    GroupBy: ColumnReferenceListType option
    GroupingSets: GroupingSetListType option
    RelOps: RelOpType list
}

and DMLOpType = {
    Base: RelOpBaseType
    AssignmentMap: AssignmentMapType option
    SourceTable: ParameterizationType option
    TargetTable: ParameterizationType option
    RelOps: RelOpType list
}

and AssignmentMapType = {
    Assigns: AssignType list
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

and GetType = {
    Base: RelOpBaseType
    Bookmarks: ColumnReferenceListType option
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

and OutputColumnsType = {
    DefinedValues: DefinedValuesListType option
    Objects: ObjectType list
}

and ForeignKeyReferencesCheckType = {
    Base: RelOpBaseType
    RelOp: RelOpType
    ForeignKeyReferenceChecks: ForeignKeyReferenceCheckType list
    ForeignKeyReferencesCount: int option
    NoMatchingIndexCount: int option
    PartialMatchingIndexCount: int option
}

and ForeignKeyReferenceCheckType = {
    IndexScan: IndexScanType
}

and AdaptiveJoinType = {
    Base: RelOpBaseType
    HashKeysBuild: ColumnReferenceListType option
    HashKeysProbe: ColumnReferenceListType option
    BuildResidual: ScalarExpressionType option
    ProbeResidual: ScalarExpressionType option
    StarJoinInfo: StarJoinInfoType option
    Predicate: ScalarExpressionType option
    PassThru: ScalarExpressionType option
    OuterReferences: ColumnReferenceListType option
    PartitionId: SingleColumnReferenceType option
    RelOps: RelOpType list // 3
    BitmapCreator: bool option
    Optimized: bool
    WithOrderedPrefetch: bool option
    WithUnorderedPrefetch: bool option
}

and StarJoinInfoType = {
    Root: bool option
    OperationType: string // "Fetch" | "Index Intersection" | "Index Filter" | "Index Lookup"
}

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

and ScalarExpressionListType = {
    ScalarOperators: ScalarType list // 1+ items
}

// ========================================
// Main Scalar Type
// ========================================

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
    WaitStats: WaitStatListType option
    QueryTimeStats: QueryExecTimeType option
    RelOp: RelOpType
    ParameterList: ColumnReferenceListType option
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

// Complex statement type that is constructed by a condition, a then clause and an optional else clause.
and StmtCondType = {
    BaseInfo: BaseStmtInfoType
    Condition: {| QueryPlan : QueryPlanType option; UDFs : FunctionType list |}
    Then: StmtBlockType list
    Else: StmtBlockType list option
}

// The statement block that contains many statements
and StmtBlockType =
    | ExternalDistributedComputation of ExternalDistributedComputationType
    | StmtSimple of StmtSimpleType
    | StmtCond of StmtCondType
    | StmtCursor of StmtCursorType
    | StmtReceive of StmtReceiveType
    | StmtUseDb of StmtUseDbType

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
