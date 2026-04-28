module DbFlow.SqlServer.ShowPlanXml.Parsers.Internal

open System.Xml

open DbFlow.XmlParser
open DbFlow.SqlServer.ShowPlanXml
open DbFlow.SqlServer.ShowPlanXml.Parsers.Primitives

let ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan"

// ========================================
// Simple parsers - no circular dependencies
// ========================================

let parseSetOptionsType (setOptions : Linq.XElement) : PResult<SetOptionsType, _> =
    PResult.builder {
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

// ObjectType only uses xAttr/xAttrTr + enum parsers - no circular deps
let parseObjectType (o : Linq.XElement) : PResult<ObjectType, _> =
    PResult.builder {
        let! server = xAttr "Server" o
        let! database = xAttr "Database" o
        let! schema = xAttr "Schema" o
        let! table = xAttr "Table" o
        let! index = xAttr "Index" o
        let! filtered = xAttr "Filtered" o
        let! alias = xAttr "Alias" o
        let! tableReferenceId = xAttr "TableReferenceId" o
        let! indexKind = xAttrP "IndexKind" parseIndexKind o
        let! cloneAccessScope = xAttrP "CloneAccessScope" parseCloneAccessScope o
        let! storage = xAttrP "Storage" parseStorage o
        return { 
            Server = server
            Database = database
            Schema = schema
            Table = table
            Index = index
            Filtered = filtered
            Alias = alias
            TableReferenceId = tableReferenceId
            IndexKind = indexKind
            CloneAccessScope = cloneAccessScope
            Storage = storage
        }
    }

let parseInternalInfoType (internalInfo : Linq.XElement) : PResult<InternalInfoType, _> =
    PResult.builder {
        let content = internalInfo.ToString() |> Some
        return { Content = content }
    }

let parseConstType (const_ : Linq.XElement) : PResult<ConstType, _> =
    PResult.builder {
        let! constValue = xAttrReq "ConstValue" const_
        return { ConstValue = constValue }
    }

let parseScalarSequenceType (sequence : Linq.XElement) : PResult<ScalarSequenceType, _> =
    PResult.builder {
        let! functionName = xAttrReq "FunctionName" sequence
        return { FunctionName = functionName }
    }

let parseCLRFunctionType (clrFunction : Linq.XElement) : PResult<CLRFunctionType, _> =
    PResult.builder {
        let! assembly = xAttr "Assembly" clrFunction
        let! class_ = xAttrReq "Class" clrFunction
        let! method = xAttr "Method" clrFunction
        return { 
            Assembly = assembly
            Class = class_
            Method = method 
        }
    }

let parseAffectingConvertWarning (element : Linq.XElement) : PResult<AffectingConvertWarningType, _> =
    PResult.builder {
        let! convertIssue = xAttrReq "ConvertIssue" element
        let! expression = xAttrReq "Expression" element
        return { 
            ConvertIssue = convertIssue
            Expression = expression
        }
    }

let parseWarning (warning : Linq.XElement) : PResult<Warning, _> =
    match warning.Name.LocalName with
    | "SpillOccurred" -> 
        Failf "NYI"
    | "ColumnsWithNoStatistics" -> 
        Failf "NYI"
    | "ColumnsWithStaleStatistics" -> 
        Failf "NYI"
    | "SpillToTempDb" -> 
        Failf "NYI"
    | "Wait" -> 
        Failf "NYI"
    | "PlanAffectingConvert" -> 
        Failf "NYI"
    | "SortSpillDetails" -> 
        Failf "NYI"
    | "HashSpillDetails" -> 
        Failf "NYI"
    | "ExchangeSpillDetails" -> 
        Failf "NYI"
    | "MemoryGrantWarning" -> 
        Failf "NYI"
    
    | name -> 
        Failf "Expected warning but got '%s'" name

let parseWarningsType (warnings : Linq.XElement) : PResult<WarningsType, _> =
    PResult.builder {
        let! (planAffectingConvert, rest) = xElementMany (ensureName parseAffectingConvertWarning warnings
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

let parseMemoryFractionsType (memoryFractions : Linq.XElement) : PResult<MemoryFractionsType, _> =
    PResult.builder {
        let! input = xAttrReq "Input" memoryFractions
        let! output = xAttrReq "Output" memoryFractions
        return { Input = input; Output = output }
    }

let parseMemoryGrantType (memoryGrantInfo : Linq.XElement) : PResult<MemoryGrantType, _> =
    PResult.builder {
        let! serialRequiredMemory = xAttrReq "SerialRequiredMemory" memoryGrantInfo
        let! serialDesiredMemory = xAttrReq "SerialDesiredMemory" memoryGrantInfo 
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
            RequestedMemory = requestedMemory
            GrantWaitTime = grantWaitTime
            GrantedMemory = grantedMemory
            MaxUsedMemory = maxUsedMemory
            MaxQueryMemory = maxQueryMemory
            LastRequestedMemory = lastRequestedMemory
            IsMemoryGrantFeedbackAdjusted = isMemoryGrantFeedbackAdjusted
        }
    }

let parseOptimizerHardwareDependentProperties (props : Linq.XElement) : PResult<OptimizerHardwareDependentPropertiesType, _> =
    PResult.builder {
        let! estimatedAvailableMemoryGrant = xAttrReq "EstimatedAvailableMemoryGrant" props
        let! estimatedPagesCached = xAttrReq "EstimatedPagesCached" props
        let! estimatedAvailableDegreeOfParallelism = xAttr "EstimatedAvailableDegreeOfParallelism" props
        let! maxCompileMemory = xAttr "MaxCompileMemory" props
        return {
            EstimatedAvailableMemoryGrant = estimatedAvailableMemoryGrant
            EstimatedPagesCached = estimatedPagesCached
            EstimatedAvailableDegreeOfParallelism = estimatedAvailableDegreeOfParallelism
            MaxCompileMemory = maxCompileMemory
        }
    }

let parseStatsInfo (statsInfo : Linq.XElement) : PResult<StatsInfoType, _> =
    PResult.builder {
        let! database = xAttr "Database" statsInfo
        let! schema = xAttr "Schema" statsInfo
        let! table = xAttr "Table" statsInfo
        let! statistics = xAttrReq "Statistics" statsInfo
        let! modificationCount = xAttrReq "ModificationCount" statsInfo
        let! samplingPercent = xAttrReq "SamplingPercent" statsInfo
        let! lastUpdate = xAttrP "LastUpdate" parseDateTime statsInfo
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

let parseOptimizerStatsUsage (optimizerStatsUsage : Linq.XElement) : PResult<OptimizerStatsUsageType, _> =
    PResult.builder {
        let! statisticsInfo = xElementsAllP parseStatsInfo optimizerStatsUsage
        return { StatisticsInfo = statisticsInfo }
    }

let parseBaseStmtInfoType (stmtSimple : Linq.XElement) : PResult<BaseStmtInfoType * _, _> =
    PResult.builder {
        let allElements = xElementsAll stmtSimple
        let! (statementSetOptions, elements') = xElement (ensureName ("StatementSetOptions", ns) parseSetOptionsType) allElements
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
        },
        elements'
    }

// ========================================
// Single mutual recursion block covering:
//   ColumnReference ↔ ScalarType ↔ SubqueryType ↔ RelOpType
// ========================================

let rec parseColumnReferenceType (columnReference : Linq.XElement) : PResult<ColumnReferenceType, _> =
    PResult.builder {
        let! scalarOperator = xElementP ("ScalarOperator", ns) parseScalarType columnReference
        let! internalInfo = xElementP ("InternalInfo", ns) parseInternalInfoType columnReference
        let! server = xAttr "Server" columnReference
        let! database = xAttr "Database" columnReference
        let! table = xAttr "Table" columnReference
        let! schema = xAttr "Schema" columnReference
        let! alias = xAttr "Alias" columnReference
        let! column = xAttrReq "Column" columnReference
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

and parseIdentType (ident : Linq.XElement) : PResult<IdentType, _> =
    PResult.builder {
        let! columnReference = xElementP ("ColumnReference", ns) parseColumnReferenceType ident
        let! table = xAttr "Table" ident
        return { 
            ColumnReference = columnReference
            Table = table 
        }
    }

and parseCompareType (compare : Linq.XElement) : PResult<CompareType, _> =
    PResult.builder {
        let! compareOp = xAttrReqP "CompareOp" parseCompareOp compare 
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType compare
        return { 
            CompareOp = compareOp
            ScalarOperators = scalarOperators 
        }
    }

and parseConvertType (convert : Linq.XElement) : PResult<ConvertType, _> =
    PResult.builder {
        let! dataType = xAttrReq "DataType" convert
        let! length = xAttr "Length" convert
        let! precision = xAttr "Precision" convert
        let! scale = xAttr "Scale" convert
        let! style = xAttrReq "Style" convert
        let! implicit = xAttrReq "Implicit" convert
        let! styleExpression = xElementP ("Style", ns) parseScalarExpressionType convert
        let! scalarOperator = xElementReqP ("ScalarOperator", ns) parseScalarType convert 
        return { 
            DataType = dataType
            Length = length
            Precision = precision
            Scale = scale
            Style = style
            Implicit = implicit
            StyleExpression = styleExpression
            ScalarOperator = scalarOperator 
        }
    }

and parseArithmeticType (arithmetic : Linq.XElement) : PResult<ArithmeticType, _> =
    PResult.builder {
        let! operation = xAttrReqP "Operation" parseArithmeticOperation arithmetic 
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType arithmetic
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseLogicalType (logical : Linq.XElement) : PResult<LogicalType, _> =
    PResult.builder {
        let! operation = xAttrReqP "Operation" parseLogicalOperation logical 
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType logical
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseAggregateType (aggregate : Linq.XElement) : PResult<AggregateType, _> =
    PResult.builder {
        let! aggType = xAttrReq "AggType" aggregate
        let! distinct = xAttrReq "Distinct" aggregate
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType aggregate
        return { 
            AggType = aggType
            Distinct = distinct
            ScalarOperators = scalarOperators 
        }
    }

and parseUDAggregateType (udAggregate : Linq.XElement) : PResult<UDAggregateType, _> =
    PResult.builder {
        let! distinct = xAttrReq "Distinct" udAggregate
        let! udAggObject = xElementP ("UDAggObject", ns) parseObjectType udAggregate
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType udAggregate
        return { 
            Distinct = distinct
            UDAggObject = udAggObject
            ScalarOperators = scalarOperators 
        }
    }

and parseIntrinsicType (intrinsic : Linq.XElement) : PResult<IntrinsicType, _> =
    PResult.builder {
        let! functionName = xAttrReq "FunctionName" intrinsic
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType intrinsic
        return { 
            FunctionName = functionName
            ScalarOperators = scalarOperators 
        }
    }

and parseUDFType (udf : Linq.XElement) : PResult<UDFType, _> =
    PResult.builder {
        let! functionName = xAttrReq "FunctionName" udf
        let! isClrFunction = xAttr "IsClrFunction" udf
        let! clrFunction = xElementP ("CLRFunction", ns) parseCLRFunctionType udf
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType udf 
        return { 
            FunctionName = functionName
            IsClrFunction = isClrFunction
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseUDTMethodType (udtMethod : Linq.XElement) : PResult<UDTMethodType, _> =
    PResult.builder {
        let! clrFunction = xElementP ("CLRFunction", ns) parseCLRFunctionType udtMethod
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType udtMethod 
        return { 
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseConditionalType (ifExpr : Linq.XElement) : PResult<ConditionalType, _> =
    PResult.builder {
        let! condition = xElementReqP ("Condition", ns) parseScalarExpressionType ifExpr 
        let! then_ = xElementReqP ("Then", ns) parseScalarExpressionType ifExpr 
        let! else_ = xElementReqP ("Else", ns) parseScalarExpressionType ifExpr 
        return { 
            Condition = condition
            Then = then_
            Else = else_ 
        }
    }

and parseScalarExpressionType (scalarExpr : Linq.XElement) : PResult<ScalarExpressionType, _> =
    PResult.builder {
        let! scalarOperator = xElementReqP ("ScalarOperator", ns) parseScalarType scalarExpr
        return { ScalarOperator = scalarOperator }
    }

and parseAssignTargetType (element : Linq.XElement) : PResult<AssignTargetType, _> =
    match element.Name.LocalName with
    | "ColumnReference" -> 
        parseColumnReferenceType element |> PResult.map AssignTargetType.ColumnRef
    | "ScalarOperator" -> 
        parseScalarType element |> PResult.map AssignTargetType.ScalarOp
    | name -> Failf "Expected ColumnReference or ScalarOperator but got '%s'" name

and parseAssignType (assign : Linq.XElement) : PResult<AssignType, _> =
    PResult.builder {
        let children = assign.Elements() |> Seq.toList
        match children with
        | [] -> return! Failf "Assign must have at least 2 children"
        | [_] -> return! Failf "Assign must have at least 2 children"
        | firstChild :: secondChild :: rest ->
            let! target = parseAssignTargetType firstChild
            let! value = parseScalarType secondChild
            let! sourceColumns = 
                rest 
                |> List.filter (fun e -> e.Name.LocalName = "SourceColumn")
                |> forAll parseColumnReferenceType
            let! targetColumns = 
                rest 
                |> List.filter (fun e -> e.Name.LocalName = "TargetColumn")
                |> forAll parseColumnReferenceType
            return { 
                Target = target
                Value = value
                SourceColumns = sourceColumns
                TargetColumns = targetColumns 
            }
    }

and parseMultAssignType (multiAssign : Linq.XElement) : PResult<MultAssignType, _> =
    PResult.builder {
        let! assigns = xElementsP ("Assign", ns) parseAssignType multiAssign
        return { Assigns = assigns }
    }

and parseScalarExpressionListType (scalarExprList : Linq.XElement) : PResult<ScalarExpressionListType, _> =
    PResult.builder {
        let! scalarOperators = xElementsP ("ScalarOperator", ns) parseScalarType scalarExprList
        return { ScalarOperators = scalarOperators }
    }

and parseSubqueryType (subquery : Linq.XElement) : PResult<SubqueryType, _> =
    PResult.builder {
        let! operation = xAttrReqP "Operation" parseSubqueryOperation subquery 
        let! scalarOperator = xElementP ("ScalarOperator", ns) parseScalarType subquery
        let! relOp = xElementReqP ("RelOp", ns) parseRelOpType subquery
        return { 
            Operation = operation
            ScalarOperator = scalarOperator
            RelOp = relOp 
        }
    }

and parseScalarType (scalarOperator : Linq.XElement) : PResult<ScalarType, _> =
    // TODO: Should be refactored
    let parseChild () =
        let childElements = scalarOperator.Elements() |> Seq.toList
        match childElements with
        | [] -> Failf "ScalarOperator must have at least one child element"
        | child :: _ ->
            match child.Name.LocalName with
            | "Aggregate" -> parseAggregateType child |> PResult.map ScalarOperatorKind.Aggregate
            | "Arithmetic" -> parseArithmeticType child |> PResult.map ScalarOperatorKind.Arithmetic
            | "Assign" -> parseAssignType child |> PResult.map ScalarOperatorKind.Assign
            | "Compare" -> parseCompareType child |> PResult.map ScalarOperatorKind.Compare
            | "Const" -> parseConstType child |> PResult.map ScalarOperatorKind.Const
            | "Convert" -> parseConvertType child |> PResult.map ScalarOperatorKind.Convert
            | "Identifier" -> parseIdentType child |> PResult.map ScalarOperatorKind.Identifier
            | "IF" -> parseConditionalType child |> PResult.map ScalarOperatorKind.IF
            | "Intrinsic" -> parseIntrinsicType child |> PResult.map ScalarOperatorKind.Intrinsic
            | "Logical" -> parseLogicalType child |> PResult.map ScalarOperatorKind.Logical
            | "MultipleAssign" -> parseMultAssignType child |> PResult.map ScalarOperatorKind.MultipleAssign
            | "ScalarExpressionList" -> parseScalarExpressionListType child |> PResult.map ScalarOperatorKind.ScalarExpressionList
            | "Sequence" -> parseScalarSequenceType child |> PResult.map ScalarOperatorKind.Sequence
            | "Subquery" -> parseSubqueryType child |> PResult.map ScalarOperatorKind.Subquery
            | "UDTMethod" -> parseUDTMethodType child |> PResult.map ScalarOperatorKind.UDTMethod
            | "UserDefinedAggregate" -> parseUDAggregateType child |> PResult.map ScalarOperatorKind.UserDefinedAggregate
            | "UserDefinedFunction" -> parseUDFType child |> PResult.map ScalarOperatorKind.UserDefinedFunction
            | name -> Failf "Unknown scalar operator type: '%s'" name
    PResult.builder {
        let! kind = parseChild ()
        let! scalarString = xAttr "ScalarString" scalarOperator
        let! internalInfo = xElementP ("InternalInfo", ns) parseInternalInfoType scalarOperator
        return { 
            Kind = kind
            ScalarString = scalarString
            InternalInfo = internalInfo 
        }
    }

and parseRelOpType (relOp : Linq.XElement) : PResult<RelOpType, _> =
    PResult.builder {
        let! outputList = xElementReqP ("OutputList", ns) (xElementsP ("ColumnReference", ns) parseColumnReferenceType) relOp
        let! warnings = xElementP ("Warnings", ns) parseWarningsType relOp
        let! memoryFractions = xElementP ("MemoryFractions", ns) parseMemoryFractionsType relOp

        let! avgRowSize = xAttrReq "AvgRowSize" relOp
        let! estimateCPU = xAttrReq "EstimateCPU" relOp
        let! estimateIO = xAttrReq "EstimateIO" relOp
        let! estimateRebinds = xAttrReq "EstimateRebinds" relOp
        let! estimateRewinds = xAttrReq "EstimateRewinds" relOp
        let! estimateRows = xAttrReq "EstimateRows" relOp
        let! logicalOp = xAttrReqP "LogicalOp" parseLogicalOp relOp 
        let! parallel_ = xAttrReq "Parallel" relOp
        let! physicalOp = xAttrReqP "PhysicalOp" parsePhysicalOp relOp
        let! estimatedTotalSubtreeCost = xAttrReq "EstimatedTotalSubtreeCost" relOp

        let! estimatedExecutionMode = xAttrP "EstimatedExecutionMode" parseExecutionMode relOp
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
        let! estimatedJoinType = xAttrP "EstimatedJoinType" parsePhysicalOp relOp
        let! hyperScaleOptimizedQueryProcessing = xAttr "HyperScaleOptimizedQueryProcessing" relOp
        let! hyperScaleOptimizedQueryProcessingUnusedReason = xAttr "HyperScaleOptimizedQueryProcessingUnusedReason" relOp
        let! pdwAccumulativeCost = xAttr "PDWAccumulativeCost" relOp

        return {
            OutputList = outputList
            Warnings = warnings
            MemoryFractions = memoryFractions
            RunTimeInformation = None
            RunTimePartitionSummary = None
            InternalInfo = None
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

// ========================================
// Non-recursive parsers that depend on the block above
// ========================================

let parseQueryPlanType (queryPlan : Linq.XElement) : PResult<QueryPlanType, _> =
    PResult.builder {
        let! degreeOfParallelism = xAttr "DegreeOfParallelism" queryPlan
        let! nonParallelPlanReason = xAttr "NonParallelPlanReason" queryPlan
        let! memoryGrant = xAttr "MemoryGrant" queryPlan
        let! cachedPlanSize = xAttr "CachedPlanSize" queryPlan
        let! compileTime = xAttr "CompileTime" queryPlan
        let! compileCPU = xAttr "CompileCPU" queryPlan
        let! compileMemory = xAttr "CompileMemory" queryPlan
        let! warnings = xElementP ("Warnings", ns) parseWarningsType queryPlan 
        let! memoryGrantInfo = xElementP ("MemoryGrantInfo", ns) parseMemoryGrantType queryPlan
        let! optimizerHardwareDependentProperties = xElementP ("OptimizerHardwareDependentProperties", ns) parseOptimizerHardwareDependentProperties queryPlan
        let! optimizerStatsUsage = xElementP ("OptimizerStatsUsage", ns) parseOptimizerStatsUsage queryPlan
        let! relOpType = xElementReqP ("RelOp", ns) parseRelOpType queryPlan
        let! columnReferenceType = xElementP ("ParameterList", ns) (xElementsP ("ColumnReference", ns) parseColumnReferenceType) queryPlan
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

let parseReceiveOperation (receiveOperation : Linq.XElement) : PResult<ReceiveOperationType, _> =
    PResult.builder {
        let! operationType = xAttrReq "OperationType" receiveOperation
        let! queryPlan = xElementReqP ("QueryPlan", ns) parseQueryPlanType receiveOperation
        return {
            OperationType = operationType
            QueryPlan = queryPlan
        }
    }

let parseStmtSimple (stmtSimple : Linq.XElement) : PResult<StmtSimpleType, _> =
    PResult.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtSimple
        let! (queryPlanType, rest) = xElement (ensureName ("QueryPlan", ns) parseQueryPlanType) rest 
        do! xElementEnsureEmpty rest
        return { BaseInfo = baseInfo; QueryPlan = queryPlanType }
    }

let parseStmtCursor (stmtCursor : Linq.XElement) : PResult<StmtCursorType, _> =
    PResult.builder {
        let! baseInfo = parseBaseStmtInfoType stmtCursor
        return failwith "NYI: StmtCursor parsing not yet implemented"
    }

let parseStmtReceive (stmtReceive : Linq.XElement) : PResult<StmtReceiveType, _> =
    PResult.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtReceive
        let! operations = xElementsP ("Operation", ns) parseReceiveOperation stmtReceive
        do! xElementEnsureEmpty rest
        return { 
            BaseInfo = baseInfo
            Operations = operations
        }
    }

let parseStmtUseDb (stmtUseDb : Linq.XElement) : PResult<StmtUseDbType, _> =
    PResult.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtUseDb
        let! database = xAttrReq "Database" stmtUseDb
        do! xElementEnsureEmpty rest
        return { 
            BaseInfo = baseInfo
            Database = database
        }
    }

let parseExternalDistributedComputation (externalDistributedComputation : Linq.XElement) : PResult<ExternalDistributedComputationType, _> =
    PResult.builder {
        let! edcShowplanXml = xAttrReq "EdcShowplanXml" externalDistributedComputation
        return { 
            EdcShowplanXml = edcShowplanXml 
        }
    }

// ========================================
// Mutual recursion for statement block types
// ========================================

let rec parseFunctionType (functionType : Linq.XElement) : PResult<FunctionType, _> =
    PResult.builder {
        let! statements = xElementsP ("Statements", ns) parseStmtBlockType functionType
        let! procName = xAttrReq "ProcName" functionType
        let! isNativelyCompiled = xAttr "IsNativelyCompiled" functionType
        return {
            Statements = statements
            ProcName = procName
            IsNativelyCompiled = isNativelyCompiled 
        }
    }

and parseStmtCond (stmtCond: Linq.XElement) : PResult<StmtCondType, _> =
    PResult.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtCond
        let! (condition, rest) =
            xElementReq 
                (ensureName ("Condition", ns)
                    (fun (condition : Linq.XElement) ->
                        PResult.builder {
                            let allElements = xElementsAll condition
                            let! (queryPlan, rest) = xElement (ensureName ("QueryPlan", ns) parseQueryPlanType) allElements
                            let! (udfFunctions, rest) = xElementMany (ensureName ("UDF", ns) parseFunctionType) rest
                            do! xElementEnsureEmpty rest
                            return {| QueryPlan = queryPlan; UDFs = udfFunctions |}
                        }))
                rest
        let! (thenStmt, rest) = 
            xElementReq 
                (ensureName ("Then", ns) 
                    (fun stmts -> 
                        xElementsAll stmts 
                        |> xElementReq (ensureName ("Statements", ns) parseStatements)
                        // Should use xElementEnsureEmpty
                        |> PResult.map fst))
                rest
        let! (elseStmt, rest) = 
            xElement 
                (ensureName ("Else", ns) 
                    (fun stmts -> 
                        xElementsAll stmts 
                        |> xElementReq (ensureName ("Statements", ns) parseStatements)
                        // Should use xElementEnsureEmpty
                        |> PResult.map fst))
                rest
        do! xElementEnsureEmpty rest
        return { 
            BaseInfo = baseInfo
            Condition = condition
            Then = thenStmt
            Else = match elseStmt with Some es -> es | None -> []
        }
    }

and parseStmtBlockType (stmtType : Linq.XElement) : PResult<StmtBlockType, _> =
    match stmtType.Name.LocalName with
    | "StmtSimple" -> parseStmtSimple stmtType |> PResult.map StmtBlockType.StmtSimple 
    | "StmtCond" -> ensureName ("StmtCond", ns) parseStmtCond stmtType |> PResult.map StmtBlockType.StmtCond 
    | "StmtCursor" -> parseStmtCursor stmtType |> PResult.map StmtBlockType.StmtCursor 
    | "StmtReceive" -> parseStmtReceive stmtType |> PResult.map StmtBlockType.StmtReceive 
    | "StmtUseDb" -> parseStmtUseDb stmtType |> PResult.map StmtBlockType.StmtUseDb 
    | "ExternalDistributedComputation" -> parseExternalDistributedComputation stmtType |> PResult.map StmtBlockType.ExternalDistributedComputation 
    | n -> Failf "Expected valid StmtType but got '%s'" n 

and parseStatements (statements : Linq.XElement) : PResult<StmtBlockType list, _> =
    PResult.builder {
        let! (stmtBlockTypes, rest) = xElementMany parseStmtBlockType (xElementsAll statements) 
        do! xElementEnsureEmpty rest
        return stmtBlockTypes
    }

let parseBatch (batch : Linq.XElement) : PResult<Batch, _> =
    PResult.builder {
        let! (stmtBlockTypes, rest) = xElementMany (ensureName ("Statements", ns) parseStatements) (xElementsAll batch) 
        do! xElementEnsureEmpty rest
        return { Statements = stmtBlockTypes |> List.collect id }
    }

let parseBatchSequence (batchSequence : Linq.XElement) =
    PResult.builder {
        let! (batches, rest) = 
            xElementMany1 (ensureName ("Batch", ns) parseBatch) (xElementsAll batchSequence)
        do! xElementEnsureEmpty rest
        return batches
    }
