module DbFlow.Test.PlanParser


open System.Xml

open DbFlow
open DbFlow.Test.XmlParser
open DbFlow.Test.ShowPlanXml    

let ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan"

/// Helper function to apply a Result-returning function to all elements in a list, accumulating results or returning the first error encountered.
let forAll (f : 'a -> Result<'b, _>) (xs : 'a list) : Result<'b list, _> =
    let rec forAll' acc xs =
        match xs with
        | [] -> Ok (List.rev acc)
        | x :: xs ->
            match f x with
            | Ok y -> forAll' (y :: acc) xs
            | Error e -> Error e
    forAll' [] xs

let parseDateTime (dateTimeStr : string) : Result<System.DateTime, _> =
    match System.DateTime.TryParse dateTimeStr with
    | true, dt -> Ok dt
    | false, _ -> Result.Errorf "Invalid DateTime format: '%s'" dateTimeStr

let parseSetOptionsType (setOptions : Linq.XElement) : Result<SetOptionsType, _> =
    Result.builder {
        let! ansiNulls = xAttr "ANSI_NULLS" setOptions
        let! ansiPadding = xAttr "ANSI_PADDING" setOptions
        let! ansiWarnings = xAttr "ANSI_WARNINGS" setOptions
        let! arithAbort = xAttr "ARITHABORT" setOptions
        let! concatNullYieldsNull = xAttr "CONCAT_NULL_YIELDS_NULL" setOptions
        let! numericRoundabort = xAttr "NUMERIC_ROUNDABORT" setOptions
        let! quotedIdentifier = xAttr "QUOTED_IDENTIFIER" setOptions
        return {
            ANSI_NULLS = ansiNulls
            ANSI_PADDING = ansiPadding
            ANSI_WARNINGS = ansiWarnings
            ARITHABORT = arithAbort
            CONCAT_NULL_YIELDS_NULL = concatNullYieldsNull
            NUMERIC_ROUNDABORT = numericRoundabort
            QUOTED_IDENTIFIER = quotedIdentifier
        }
    }

let parseBaseStmtInfoType (stmtSimple : Linq.XElement) : Result<BaseStmtInfoType, _> =
    Result.builder {
        
        let! statementSetOptions = xElementOptional parseSetOptionsType ("StatementSetOptions", ns) stmtSimple

        let! statementCompId = xAttr "StatementCompId" stmtSimple
        let! statementEstRows = xAttr "StatementEstRows" stmtSimple
        let! statementId = xAttr "StatementId" stmtSimple
        let! statementOptmLevel = xAttr "StatementOptmLevel" stmtSimple
        let! statementOptmEarlyAbortReason = xAttr "StatementOptmEarlyAbortReason" stmtSimple
        let! cardinalityEstimationModelVersion = xAttr "CardinalityEstimationModelVersion" stmtSimple
        let! statementSubTreeCost = xAttr "StatementSubTreeCost" stmtSimple
        let! statementText = xAttr "StatementText" stmtSimple
        let! statementType = xAttr "StatementType" stmtSimple
        let! queryHash = xAttr "QueryHash" stmtSimple
        let! queryPlanHash = xAttr "QueryPlanHash" stmtSimple
        let! retrievedFromCache = xAttr "RetrievedFromCache" stmtSimple
        let! statementSqlHandle = xAttr "StatementSqlHandle" stmtSimple
        let! securityPolicyApplied = xAttr "SecurityPolicyApplied" stmtSimple
        let! batchModeOnRowStoreUsed = xAttr "BatchModeOnRowStoreUsed" stmtSimple
        return {
            StatementSetOptions = statementSetOptions
            StatementCompId = statementCompId
            StatementEstRows = statementEstRows
            StatementId = statementId
            StatementOptmLevel = statementOptmLevel
            StatementOptmEarlyAbortReason = statementOptmEarlyAbortReason
            CardinalityEstimationModelVersion = cardinalityEstimationModelVersion
            StatementSubTreeCost = statementSubTreeCost
            StatementText = statementText
            StatementType = statementType
            QueryHash = queryHash
            QueryPlanHash = queryPlanHash 
            RetrievedFromCache = retrievedFromCache 
            StatementSqlHandle = statementSqlHandle
            SecurityPolicyApplied = securityPolicyApplied
            BatchModeOnRowStoreUsed = batchModeOnRowStoreUsed
        }
    }

let parseAffectingConvertWarning (warnings : Linq.XElement) : Result<AffectingConvertWarningType, _> =
    Result.builder {
        let! convertIssue = xAttrRequire "ConvertIssue" warnings
        let! expression = xAttrRequire "Expression" warnings
        return { 
            ConvertIssue = convertIssue
            Expression = expression
        }
    }

let parseWarningsType (warnings : Linq.XElement) : Result<WarningsType, _> =
    Result.builder {
        let! planAffectingConvert = xElementsAll warnings |> forAll parseAffectingConvertWarning

        let! noJoinPredicate = xAttr "NoJoinPredicate" warnings
        let! spatialGuess = xAttr "SpatialGuess" warnings
        let! unmatchedIndexes = xAttr "UnmatchedIndexes" warnings
        let! fullUpdateForOnlineIndexBuild = xAttr "FullUpdateForOnlineIndexBuild" warnings
        return { 
            PlanAffectingConvert = planAffectingConvert
            NoJoinPredicate = noJoinPredicate
            SpatialGuess = spatialGuess
            UnmatchedIndexes = unmatchedIndexes
            FullUpdateForOnlineIndexBuild = fullUpdateForOnlineIndexBuild
        }
    }

let parseMemoryGrantType (memoryGrantInfo : Linq.XElement) : Result<MemoryGrantType, _> =
    Result.builder {
        let! serialRequiredMemory = xAttrRequire "SerialRequiredMemory" memoryGrantInfo
        let! serialDesiredMemory = xAttrRequire "SerialDesiredMemory" memoryGrantInfo 
        let! requiredMemory = xAttr "RequiredMemory" memoryGrantInfo 
        let! desiredMemory = xAttr "DesiredMemory" memoryGrantInfo 
        let! requestedMemory = xAttr "RequestedMemory" memoryGrantInfo 
        let! grantWaitTime = xAttr "GrantWaitTime" memoryGrantInfo 
        let! grantedMemory = xAttr "GrantedMemory" memoryGrantInfo 
        let! maxUsedMemory = xAttr "MaxUsedMemory" memoryGrantInfo 
        let! maxQueryMemory = xAttr "MaxQueryMemory" memoryGrantInfo 
        let! lastRequestedMemory = xAttr "LastRequestedMemory" memoryGrantInfo 
        let! isMemoryGrantFeedbackAdjusted = xAttr "IsMemoryGrantFeedbackAdjusted" memoryGrantInfo
        return {
            SerialRequiredMemory = serialRequiredMemory 
            SerialDesiredMemory = serialDesiredMemory
            RequiredMemory = requiredMemory
            DesiredMemory = desiredMemory 
            RequestedMemory =requestedMemory
            GrantWaitTime =grantWaitTime
            GrantedMemory = grantedMemory
            MaxUsedMemory = maxUsedMemory
            MaxQueryMemory = maxQueryMemory
            LastRequestedMemory = lastRequestedMemory
            IsMemoryGrantFeedbackAdjusted = isMemoryGrantFeedbackAdjusted
        }
    }

let parseOptimizerHardwareDependentProperties (optimizerHardwareDependentProperties : Linq.XElement) : Result<OptimizerHardwareDependentPropertiesType, _> =
    Result.builder {
        let! estimatedAvailableMemoryGrant = xAttrRequire "EstimatedAvailableMemoryGrant" optimizerHardwareDependentProperties
        let! estimatedPagesCached = xAttrRequire "EstimatedPagesCached" optimizerHardwareDependentProperties
        let! estimatedAvailableDegreeOfParallelism = xAttr "EstimatedAvailableDegreeOfParallelism" optimizerHardwareDependentProperties
        let! maxCompileMemory = xAttr "MaxCompileMemory" optimizerHardwareDependentProperties
        return {
            EstimatedAvailableMemoryGrant = estimatedAvailableMemoryGrant
            EstimatedPagesCached = estimatedPagesCached
            EstimatedAvailableDegreeOfParallelism = estimatedAvailableDegreeOfParallelism
            MaxCompileMemory = maxCompileMemory
        }
    }

let parseStatsInfo (statsInfo : Linq.XElement) : Result<StatsInfoType, _> =
    Result.builder {
        let! database = xAttr "Database" statsInfo
        let! schema = xAttr "Schema" statsInfo
        let! table = xAttr "Table" statsInfo
        let! statistics = xAttrRequire "Statistics" statsInfo
        let! modificationCount = xAttrRequire "ModificationCount" statsInfo
        let! samplingPercent = xAttrRequire "SamplingPercent" statsInfo
        let! lastUpdate = xAttrTr parseDateTime "LastUpdate" statsInfo
        return {
            Database = database
            Schema = schema
            Table = table
            Statistics = statistics
            ModificationCount = modificationCount
            SamplingPercent = samplingPercent
            LastUpdate = lastUpdate
        }
    }

let parseOptimizerStatsUsage (optimizerStatsUsage : Linq.XElement) : Result<OptimizerStatsUsageType, _> =
    Result.builder {
        let! statisticsInfo = xElementsAll optimizerStatsUsage |> forAll parseStatsInfo
        return { StatisticsInfo = statisticsInfo }
    }

let parseMemoryFractionsType (memoryFractions : Linq.XElement) : Result<MemoryFractionsType, _> =
    Result.builder {
        let! input = xAttrRequire "Input" memoryFractions
        let! output = xAttrRequire "Output" memoryFractions
        return { Input = input; Output = output }
    }

let parseScalarType (scalarOperator : Linq.XElement) : Result<ScalarType, _> =
    Result.builder {
        // Placeholder for scalar operator parsing
        // TODO: Implement full scalar operator type parsing based on XSD
        return { Value = None }
    }

let parseInternalInfoType (internalInfo : Linq.XElement) : Result<InternalInfoType, _> =
    Result.builder {
        // Capture the raw XML content as string
        // InternalInfoType can contain arbitrary XML content
        let content = internalInfo.ToString() |> Some
        return { Content = content }
    }

let parseColumnReferenceType (columnReference : Linq.XElement) : Result<ColumnReferenceType, _> =
    Result.builder {
        // Sequence elements
        let! scalarOperator = xElementOptional parseScalarType ("ScalarOperator", ns) columnReference
        let! internalInfo = xElementOptional parseInternalInfoType ("InternalInfo", ns) columnReference

        // Attributes
        let! server = xAttr "Server" columnReference
        let! database = xAttr "Database" columnReference
        let! table = xAttr "Table" columnReference
        let! schema = xAttr "Schema" columnReference
        let! alias = xAttr "Alias" columnReference
        let! column = xAttrRequire "Column" columnReference
        let! computedColumn = xAttr "ComputedColumn" columnReference
        let! parameterDataType = xAttr "ParameterDataType" columnReference
        let! parameterCompiledValue = xAttr "ParameterCompiledValue" columnReference
        let! parameterRuntimeValue = xAttr "ParameterRuntimeValue" columnReference

        return {
            ScalarOperator = scalarOperator
            InternalInfo = internalInfo
            Server = server
            Database = database
            Schema = schema
            Table = table
            Alias = alias
            Column = column
            ComputedColumn = computedColumn
            ParameterDataType = parameterDataType
            ParameterCompiledValue = parameterCompiledValue
            ParameterRuntimeValue = parameterRuntimeValue   
        }
    }

let parseColumnReferenceListType (columnReferenceList : Linq.XElement) : Result<ColumnReferenceType list, _> =
    xElements ("Columns", ns) columnReferenceList |> forAll parseColumnReferenceType

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
    | other -> Result.Errorf "Unknown LogicalOp type: '%s'" other

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
    | other -> Result.Errorf "Unknown PhysicalOp type: '%s'" other

let parseExecutionMode (s : string) : Result<ExecutionModeType, _> =
    match s with
    | "Row" -> Ok ExecutionModeType.Row
    | "Batch" -> Ok ExecutionModeType.Batch
    | other -> Result.Errorf "Unknown ExecutionModeType type: '%s'" other

let parseRelOpType (relOp : Linq.XElement) : Result<RelOpType, _> =
    Result.builder {
        let! outputList = xElementRequire ("OutputList", ns) relOp |> Result.bind parseColumnReferenceListType
        let! warnings = xElementOptional parseWarningsType ("Warnings", ns) relOp
        let! memoryFractions = xElementOptional parseMemoryFractionsType ("MemoryFractions", ns) relOp

        let! avgRowSize = xAttrRequire "AvgRowSize" relOp
        let! estimateCPU = xAttrRequire "EstimateCPU" relOp
        let! estimateIO = xAttrRequire "EstimateIO" relOp
        let! estimateRebinds = xAttrRequire "EstimateRebinds" relOp
        let! estimateRewinds = xAttrRequire "EstimateRewinds" relOp
        let! estimateRows = xAttrRequire "EstimateRows" relOp
        let! logicalOp = xAttrRequire "LogicalOp" relOp |> Result.bind parseLogicalOp
        let! parallel_ = xAttrRequire "Parallel" relOp
        let! physicalOp = xAttrRequire "PhysicalOp" relOp |> Result.bind parsePhysicalOp
        let! estimatedTotalSubtreeCost = xAttrRequire "EstimatedTotalSubtreeCost" relOp

        let! estimatedExecutionMode = xAttrTr parseExecutionMode "EstimatedExecutionMode" relOp
        let! groupExecuted = xAttr "GroupExecuted" relOp
        let! estimateRowsWithoutRowGoal = xAttr "EstimateRowsWithoutRowGoal" relOp
        let! estimatedRowsRead = xAttr "EstimatedRowsRead" relOp
        let! nodeId = xAttr "NodeId" relOp
        let! remoteDataAccess = xAttr "RemoteDataAccess" relOp
        let! partitioned = xAttr "Partitioned" relOp
        let! isAdaptive = xAttr "IsAdaptive" relOp
        let! adaptiveThresholdRows = xAttr "AdaptiveThresholdRows" relOp
        let! tableCardinality = xAttr "TableCardinality" relOp
        let! statsCollectionId = xAttr "StatsCollectionId" relOp
        let! estimatedJoinType = xAttrTr parsePhysicalOp "EstimatedJoinType" relOp
        let! hyperScaleOptimizedQueryProcessing = xAttr "HyperScaleOptimizedQueryProcessing" relOp
        let! hyperScaleOptimizedQueryProcessingUnusedReason = xAttr "HyperScaleOptimizedQueryProcessingUnusedReason" relOp
        let! pdwAccumulativeCost = xAttr "PDWAccumulativeCost" relOp

        return {
            // Sequence elements
            OutputList = outputList
            Warnings = warnings
            MemoryFractions = memoryFractions
            RunTimeInformation = None // NYI
            RunTimePartitionSummary = None // NYI
            InternalInfo = None // NYI

            // Required attributes
            AvgRowSize = avgRowSize
            EstimateCPU = estimateCPU
            EstimateIO = estimateIO
            EstimateRebinds = estimateRebinds
            EstimateRewinds = estimateRewinds
            EstimateRows = estimateRows
            LogicalOp = logicalOp
            Parallel = parallel_
            PhysicalOp = physicalOp
            EstimatedTotalSubtreeCost = estimatedTotalSubtreeCost

            // Optional attributes
            EstimatedExecutionMode = estimatedExecutionMode
            GroupExecuted = groupExecuted
            EstimateRowsWithoutRowGoal = estimateRowsWithoutRowGoal
            EstimatedRowsRead = estimatedRowsRead
            NodeId = nodeId
            RemoteDataAccess = remoteDataAccess
            Partitioned = partitioned
            IsAdaptive = isAdaptive
            AdaptiveThresholdRows = adaptiveThresholdRows
            TableCardinality = tableCardinality
            StatsCollectionId = statsCollectionId
            EstimatedJoinType = estimatedJoinType
            HyperScaleOptimizedQueryProcessing = hyperScaleOptimizedQueryProcessing
            HyperScaleOptimizedQueryProcessingUnusedReason = hyperScaleOptimizedQueryProcessingUnusedReason
            PDWAccumulativeCost = pdwAccumulativeCost
        }
    }

let parseQueryPlanType (queryPlan : Linq.XElement) : Result<QueryPlanType, _> =
    Result.builder {
        // Attributes
        let! degreeOfParallelism = xAttr "DegreeOfParallelism" queryPlan
        let! nonParallelPlanReason = xAttr "NonParallelPlanReason" queryPlan
        let! memoryGrant = xAttr "MemoryGrant" queryPlan
        let! cachedPlanSize = xAttr "CachedPlanSize" queryPlan
        let! compileTime = xAttr "CompileTime" queryPlan
        let! compileCPU = xAttr "CompileCPU" queryPlan
        let! compileMemory = xAttr "CompileMemory" queryPlan
        // elements
        let! warnings = xElementOptional parseWarningsType ("Warnings", ns) queryPlan 
        let! memoryGrantInfo = xElementOptional parseMemoryGrantType ("MemoryGrantInfo", ns) queryPlan
        let! optimizerHardwareDependentProperties = xElementOptional parseOptimizerHardwareDependentProperties ("OptimizerHardwareDependentProperties", ns) queryPlan
        let! optimizerStatsUsage = xElementOptional parseOptimizerStatsUsage ("OptimizerStatsUsage", ns) queryPlan
        let! relOpType' = xElementRequire ("RelOp", ns) queryPlan
        let! relOpType = parseRelOpType relOpType'
        let! columnReferenceType = xElementOptional parseColumnReferenceListType ("ParameterList", ns) queryPlan
        
        return {
            DegreeOfParallelism = degreeOfParallelism
            NonParallelPlanReason = nonParallelPlanReason
            MemoryGrant = memoryGrant
            CachedPlanSize = cachedPlanSize
            CompileTime = compileTime
            CompileCPU = compileCPU
            CompileMemory = compileMemory
            Warnings = warnings
            MemoryGrantInfo = memoryGrantInfo 
            OptimizerHardwareDependentProperties = optimizerHardwareDependentProperties
            OptimizerStatsUsage = optimizerStatsUsage
            RelOp = relOpType
            ParameterList = match columnReferenceType with Some cs -> cs | None -> []
        }
    }

let parseReceiveOperation (receiveOperation : Linq.XElement) : Result<ReceiveOperationType, _> =
    Result.builder {
        let! operationType = xAttrRequire "OperationType" receiveOperation
        let! queryPlan = xElementRequire ("QueryPlan", ns) receiveOperation |> Result.bind parseQueryPlanType
        return {
            OperationType = operationType
            QueryPlan = queryPlan
        }
    }

let parseStmtSimple (stmtSimple : Linq.XElement) : Result<StmtSimpleType, _> =
    Result.builder {
        let! baseInfo = parseBaseStmtInfoType stmtSimple
        let! queryPlanType = xElementOptional parseQueryPlanType ("QueryPlan", ns) stmtSimple 
             
        return { BaseInfo = baseInfo; QueryPlan = queryPlanType }
    }

let parseStmtCursor (stmtCursor : Linq.XElement) : Result<StmtCursorType, _> =
    Result.builder {
        let! baseInfo = parseBaseStmtInfoType stmtCursor
        //return { 
        //    BaseInfo = baseInfo
        //}
        return failwith "NYI"
    }

let parseStmtReceive (stmtReceive : Linq.XElement) : Result<StmtReceiveType, _> =
    Result.builder {
        let! baseInfo = parseBaseStmtInfoType stmtReceive
        let! operations = xElements ("ReceiveOperationType", ns) stmtReceive |> forAll parseReceiveOperation
        return { 
            BaseInfo = baseInfo
            Operations = operations
        }
    }    

let parseStmtUseDb (stmtUseDb : Linq.XElement) : Result<StmtUseDbType, _> =
        Result.builder {
        let! baseInfo = parseBaseStmtInfoType stmtUseDb
        let! database = xAttrRequire ("Database", ns) stmtUseDb
        return { 
            BaseInfo = baseInfo
            Database = database
        }
    }

let parseExternalDistributedComputation (externalDistributedComputation : Linq.XElement) : Result<ExternalDistributedComputationType, _> =
    Result.builder {
        let! edcShowplanXml = xAttrRequire ("EdcShowplanXml", ns) externalDistributedComputation
        return { 
            EdcShowplanXml = edcShowplanXml 
        }
    }

let rec parseFunctionType (functionType : Linq.XElement) : Result<FunctionType, _> =
    Result.builder {
        let! statements = xElements ("Statements", ns) functionType |> forAll parseStmtBlockType
        let! procName = xAttrRequire "ProcName" functionType
        let! isNativelyCompiled = xAttr "IsNativelyCompiled" functionType
        return {
            Statements = statements
            ProcName = procName
            IsNativelyCompiled = isNativelyCompiled 
        }
    }

and parseStmtCond (stmtCond: Linq.XElement) : Result<StmtCondType, _> =
    Result.builder {
        let! baseInfo = parseBaseStmtInfoType stmtCond
        let! condition' = xElementRequire ("Condition", ns) stmtCond
        let! queryPlan = xElementOptional parseQueryPlanType ("QueryPlan", ns) condition' 
        let! udfFunctions = xElements ("UDF", ns) condition' |> forAll parseFunctionType
        let! thenStmt = xElementRequire ("Then", ns) stmtCond |> Result.bind parseStmtBlockType
        let! elseStmt = xElementOptional parseStmtBlockType ("Else", ns) stmtCond 
        return { 
            BaseInfo = baseInfo
            Condition = {| QueryPlan = queryPlan; UDFs = udfFunctions |}
            Then = thenStmt
            Else = elseStmt
        }
    }

and parseStmtBlockType (stmtType : Linq.XElement) : Result<StmtBlockType, _> =
    match stmtType.Name.LocalName with
    | "StmtSimple" -> parseStmtSimple stmtType |> Result.map StmtSimple 
    | "StmtCond" -> parseStmtCond stmtType |> Result.map StmtCond 
    | "StmtCursor" -> parseStmtCursor stmtType |> Result.map StmtCursor 
    | "StmtReceive" ->parseStmtReceive stmtType |> Result.map StmtReceive 
    | "StmtUseDb" -> parseStmtUseDb stmtType |> Result.map StmtUseDb 
    | "ExternalDistributedComputation" -> parseExternalDistributedComputation stmtType |> Result.map ExternalDistributedComputation 
    | n -> Result.Errorf "Expected valid StmtType but got '%s'" n 


let parseBatch (batch : Linq.XElement) : Result<Batch, _> =
    Result.builder {
        let! statements = xElementRequire ("Statements", ns) batch
        let! stmtBlockTypes = xElementsAll statements |> forAll parseStmtBlockType
        return { Statements = stmtBlockTypes }
    }

let parseShowPlanXML (root : Linq.XElement) : Result<ShowPlanXML, _> =
    Result.builder {
        let! version = xAttrRequire "Version" root  
        let! build = xAttrRequire "Build" root
        let! clusteredMode = xAttr "ClusteredMode" root
                    
        let! batchSequence = xElementRequire ("BatchSequence", ns) root
        let! batches =
            xElements ("Batch", ns) batchSequence
            |> forAll parseBatch 
        return {
            Version = version
            Build = build
            ClusteredMode = clusteredMode
            BatchSequence = { Batches = batches }
        }
    }
