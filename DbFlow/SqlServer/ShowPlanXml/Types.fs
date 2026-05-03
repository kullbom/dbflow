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
    // Additional information, like spill I/O stats may go here when available
    InternalInfo : InternalInfoType

    SpillLevel : uint64 option
    SpilledThreadCount : uint64 option
}

// Sort spill details
type SortSpillDetailsType = {
    // Additional information, like spill I/O stats may go here when available
    InternalInfo : InternalInfoType

    GrantedMemoryKb : uint64 option
    UsedMemoryKb : uint64 option
    WritesToTempDb : uint64 option
    ReadsFromTempDb : uint64 option
}

// Hash spill details
type HashSpillDetailsType = {
    // Additional information, like spill I/O stats may go here when available
    InternalInfo : InternalInfoType

    GrantedMemoryKb : uint64 option
    UsedMemoryKb : uint64 option
    WritesToTempDb : uint64 option
    ReadsFromTempDb : uint64 option
}

// Exchange spill details
type ExchangeSpillDetailsType = {
    // Additional information, like spill I/O stats may go here when available
    InternalInfo : InternalInfoType
    
    WritesToTempDb : uint64 option
}

// Query wait information
type WaitWarningType = {
    // Additional information, like spill I/O stats may go here when available
    InternalInfo : InternalInfoType
    
    WaitType : string // enum: "Memory Grant"
}

/// Provide warning information for memory grant.
/// GrantWarningKind: Warning kind
/// RequestedMemory: Initial grant request in KB
/// GrantedMemory: Granted memory in KB
/// MaxUsedMemory: Maximum used memory grant in KB
type MemoryGrantWarningInfo = {
    GrantWarningKind : string // enum: "Excessive Grant", "Used More Than Granted", "Grant Increase" 
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


// ========================================
// Mutually Recursive Types
// ========================================
type Warning =
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


// ========================================
// Relational Operator Detail Types
// ========================================

and RelOpRelOpBaseType = {
    DefinedValues : DefinedValuesListType option
    InternalInfo : InternalInfoType option
}
(*
<xsd:complexType name="RelOpType">
		<xsd:sequence>
			
			<xsd:choice>
				<xsd:element name="AdaptiveJoin" type="shp:AdaptiveJoinType" />
				<!--				PDW						-->
				<xsd:element name="Apply" type="shp:JoinType" />
				<!--				/PDW					-->
				<xsd:element name="Assert" type="shp:FilterType" />
				<xsd:element name="BatchHashTableBuild" type="shp:BatchHashTableBuildType" />
				<xsd:element name="Bitmap" type="shp:BitmapType" />
				<xsd:element name="Collapse" type="shp:CollapseType" />
				<xsd:element name="ComputeScalar" type="shp:ComputeScalarType" />
				<xsd:element name="Concat" type="shp:ConcatType" />
				<xsd:element name="ConstantScan" type="shp:ConstantScanType" />
				<!--				PDW							-->
				<xsd:element name="ConstTableGet" type="shp:GetType" />
				<!--				/PDW							-->
				<xsd:element name="CreateIndex" type="shp:CreateIndexType" />
				<!--				PDW					-->
				<xsd:element name="Delete" type="shp:DMLOpType" />
				<!--				/PDW					-->
				<xsd:element name="DeletedScan" type="shp:RowsetType" />
				<xsd:element name="Extension" type="shp:UDXType" />
				<xsd:element name="ExternalSelect" type="shp:ExternalSelectType" />
				<xsd:element name="ExtExtractScan" type="shp:RemoteType" />
				<xsd:element name="Filter" type="shp:FilterType" />
				<xsd:element name="ForeignKeyReferencesCheck" type="shp:ForeignKeyReferencesCheckType" />
				<!--				PDW					-->
				<xsd:element name="GbAgg" type="shp:GbAggType" />
				<xsd:element name="GbApply" type="shp:GbApplyType" />
				<!--				/PDW					-->
				<xsd:element name="Generic" type="shp:GenericType" />
				<!--				PDW					-->
				<xsd:element name="Get" type="shp:GetType" />
				<!--				/PDW					-->
				<xsd:element name="Hash" type="shp:HashType" />
				<xsd:element name="IndexScan" type="shp:IndexScanType" />
				<xsd:element name="InsertedScan" type="shp:RowsetType" />
				<!--				PDW					-->
				<xsd:element name="Insert" type="shp:DMLOpType" />
				<xsd:element name="Join" type="shp:JoinType" />
				<xsd:element name="LocalCube" type="shp:LocalCubeType"/>
				<!--				/PDW					-->
				<xsd:element name="LogRowScan" type="shp:RelOpBaseType" />
				<xsd:element name="Merge" type="shp:MergeType" />
				<xsd:element name="MergeInterval" type="shp:SimpleIteratorOneChildType" />
				<!--				PDW					-->
				<xsd:element name="Move" type="shp:MoveType" />
				<!--				/PDW					-->
				<xsd:element name="NestedLoops" type="shp:NestedLoopsType" />
				<xsd:element name="OnlineIndex" type="shp:CreateIndexType" />
				<xsd:element name="Parallelism" type="shp:ParallelismType" />
				<xsd:element name="ParameterTableScan" type="shp:RelOpBaseType" />
				<xsd:element name="PrintDataflow" type="shp:RelOpBaseType" />
				<!--				PDW					-->
				<xsd:element name="Project" type="shp:ProjectType" />
				<!--				/PDW					-->
				<xsd:element name="Put" type="shp:PutType" />
				<xsd:element name="RemoteFetch" type="shp:RemoteFetchType" />
				<xsd:element name="RemoteModify" type="shp:RemoteModifyType" />
				<xsd:element name="RemoteQuery" type="shp:RemoteQueryType" />
				<xsd:element name="RemoteRange" type="shp:RemoteRangeType" />
				<xsd:element name="RemoteScan" type="shp:RemoteType" />
				<xsd:element name="RowCountSpool" type="shp:SpoolType" />
				<xsd:element name="ScalarInsert" type="shp:ScalarInsertType" />
				<xsd:element name="Segment" type="shp:SegmentType" />
				<xsd:element name="Sequence" type="shp:SequenceType" />
				<xsd:element name="SequenceProject" type="shp:ComputeScalarType" />
				<xsd:element name="SimpleUpdate" type="shp:SimpleUpdateType" />
				<xsd:element name="Sort" type="shp:SortType" />
				<xsd:element name="Split" type="shp:SplitType" />
				<xsd:element name="Spool" type="shp:SpoolType" />
				<xsd:element name="StreamAggregate" type="shp:StreamAggregateType" />
				<xsd:element name="Switch" type="shp:SwitchType" />
				<xsd:element name="TableScan" type="shp:TableScanType" />
				<xsd:element name="TableValuedFunction" type="shp:TableValuedFunctionType" />
				<xsd:element name="Top" type="shp:TopType" />
				<xsd:element name="TopSort" type="shp:TopSortType" />
				<xsd:element name="Update" type="shp:UpdateType" />
				<!--				PDW					-->
				<xsd:element name="Union" type="shp:ConcatType" />
				<xsd:element name="UnionAll" type="shp:ConcatType" />
				<!--				/PDW					-->
				<xsd:element name="WindowSpool" type="shp:WindowType" />
				<xsd:element name="WindowAggregate" type="shp:WindowAggregateType" />
				<xsd:element name="XcsScan" type="shp:XcsScanType" />
			</xsd:choice>
		</xsd:sequence>
		
		<!-- For PDW cost Operators -->
		<xsd:attribute name="PDWAccumulativeCost" type="xsd:double" use="optional" />
	</xsd:complexType>
	
*)

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

and AssignTargetType =
    | ColumnRef of ColumnReferenceType
    | ScalarOp of ScalarType

and AssignType = {
    Target: AssignTargetType
    Value: ScalarType
    SourceColumns: ColumnReferenceType list
    TargetColumns: ColumnReferenceType list
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

and RunTimeInformationType = {
    RunTimeCountersPerThread: RunTimeCountersPerThread list
}


// ========================================
// Parameter sensitive plan optimization schema definition
// ========================================

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

/// This is the dispatcher expression in XML format for the parameter sensitive plan.
and DispatcherType = {
    ParameterSensitivePredicate : ParameterSensitivePredicateType list // 1..3 
}

// ========================================
// Relational Operators
// ========================================

and RelOpType = {
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

and QueryPlanType = {
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

and BaseStmtInfoType = {
    StatementSetOptions: SetOptionsType option
    StatementCompId: int option
    StatementEstRows: float option
    StatementId: int option
    StatementOptmLevel: string option
    StatementOptmEarlyAbortReason: string option // "TimeOut" / "MemoryLimitExceeded" / "GoodEnoughPlanFound"
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

and ExternalDistributedComputationType = {
    EdcShowplanXml: string
}

and StmtSimpleType = {
    BaseInfo: BaseStmtInfoType
    QueryPlan: QueryPlanType option
}

and CursorOperationType = {
    OperationType: string // "FetchQuery" | "PopulateQuery" | "RefreshQuery"
    QueryPlan: QueryPlanType
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

and ReceivePlanType = {
    OperationType: string // "ReceivePlanSelect" | "ReceivePlanUpdate"
    QueryPlan: QueryPlanType
}

and StmtReceiveType = {
    BaseInfo: BaseStmtInfoType
    ReceivePlans: ReceivePlanType list
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
    Else: StmtBlockType list
}

// The statement block that contains many statements
and StmtBlockType =
    | StmtSimple of StmtSimpleType
    | StmtCond of StmtCondType
    | StmtCursor of StmtCursorType
    | StmtReceive of StmtReceiveType
    | StmtUseDb of StmtUseDbType
    | ExternalDistributedComputation of ExternalDistributedComputationType

and Batch = {
    Statements: StmtBlockType list
}

type BatchSequence = {
    Batches: Batch list
}


