module DbFlow.Plans.Parser

open System.Xml
open DbFlow
open DbFlow.XmlParser
open DbFlow.ShowPlanXml
open DbFlow.Plans.EnumsParsers

let ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan"

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
    | false, _ -> Failf "Invalid DateTime format: '%s'" dateTimeStr

// ========================================
// Simple parsers - no circular dependencies
// ========================================

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

// ObjectType only uses xAttr/xAttrTr + enum parsers - no circular deps
let parseObjectType (object_ : Linq.XElement) : Result<ObjectType, _> =
    Result.builder {
        let! server = xAttr "Server" object_
        let! database = xAttr "Database" object_
        let! schema = xAttr "Schema" object_
        let! table = xAttr "Table" object_
        let! index = xAttr "Index" object_
        let! filtered = xAttr "Filtered" object_
        let! alias = xAttr "Alias" object_
        let! tableReferenceId = xAttr "TableReferenceId" object_
        let! indexKind = xAttrTr parseIndexKind "IndexKind" object_
        let! cloneAccessScope = xAttrTr parseCloneAccessScope "CloneAccessScope" object_
        let! storage = xAttrTr parseStorage "Storage" object_
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

let parseInternalInfoType (internalInfo : Linq.XElement) : Result<InternalInfoType, _> =
    Result.builder {
        let content = internalInfo.ToString() |> Some
        return { Content = content }
    }

let parseConstType (const_ : Linq.XElement) : Result<ConstType, _> =
    Result.builder {
        let! constValue = xAttrRequire "ConstValue" const_
        return { ConstValue = constValue }
    }

let parseScalarSequenceType (sequence : Linq.XElement) : Result<ScalarSequenceType, _> =
    Result.builder {
        let! functionName = xAttrRequire "FunctionName" sequence
        return { FunctionName = functionName }
    }

let parseCLRFunctionType (clrFunction : Linq.XElement) : Result<CLRFunctionType, _> =
    Result.builder {
        let! assembly = xAttr "Assembly" clrFunction
        let! class_ = xAttrRequire "Class" clrFunction
        let! method = xAttr "Method" clrFunction
        return { 
            Assembly = assembly
            Class = class_
            Method = method 
        }
    }

let parseAffectingConvertWarning (element : Linq.XElement) : Result<AffectingConvertWarningType, _> =
    Result.builder {
        let! convertIssue = xAttrRequire "ConvertIssue" element
        let! expression = xAttrRequire "Expression" element
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

let parseMemoryFractionsType (memoryFractions : Linq.XElement) : Result<MemoryFractionsType, _> =
    Result.builder {
        let! input = xAttrRequire "Input" memoryFractions
        let! output = xAttrRequire "Output" memoryFractions
        return { Input = input; Output = output }
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
            RequestedMemory = requestedMemory
            GrantWaitTime = grantWaitTime
            GrantedMemory = grantedMemory
            MaxUsedMemory = maxUsedMemory
            MaxQueryMemory = maxQueryMemory
            LastRequestedMemory = lastRequestedMemory
            IsMemoryGrantFeedbackAdjusted = isMemoryGrantFeedbackAdjusted
        }
    }

let parseOptimizerHardwareDependentProperties (props : Linq.XElement) : Result<OptimizerHardwareDependentPropertiesType, _> =
    Result.builder {
        let! estimatedAvailableMemoryGrant = xAttrRequire "EstimatedAvailableMemoryGrant" props
        let! estimatedPagesCached = xAttrRequire "EstimatedPagesCached" props
        let! estimatedAvailableDegreeOfParallelism = xAttr "EstimatedAvailableDegreeOfParallelism" props
        let! maxCompileMemory = xAttr "MaxCompileMemory" props
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

// ========================================
// Single mutual recursion block covering:
//   ColumnReference ↔ ScalarType ↔ SubqueryType ↔ RelOpType
// ========================================

let rec parseColumnReferenceType (columnReference : Linq.XElement) : Result<ColumnReferenceType, _> =
    Result.builder {
        let! scalarOperator = xElementOptional parseScalarType ("ScalarOperator", ns) columnReference
        let! internalInfo = xElementOptional parseInternalInfoType ("InternalInfo", ns) columnReference
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

and parseIdentType (ident : Linq.XElement) : Result<IdentType, _> =
    Result.builder {
        let! columnReference = xElementOptional parseColumnReferenceType ("ColumnReference", ns) ident
        let! table = xAttr "Table" ident
        return { 
            ColumnReference = columnReference
            Table = table 
        }
    }

and parseCompareType (compare : Linq.XElement) : Result<CompareType, _> =
    Result.builder {
        let! compareOp = xAttrRequire "CompareOp" compare |> Result.bind parseCompareOp
        let! scalarOperators = xElements ("ScalarOperator", ns) compare |> forAll parseScalarType
        return { 
            CompareOp = compareOp
            ScalarOperators = scalarOperators 
        }
    }

and parseConvertType (convert : Linq.XElement) : Result<ConvertType, _> =
    Result.builder {
        let! dataType = xAttrRequire "DataType" convert
        let! length = xAttr "Length" convert
        let! precision = xAttr "Precision" convert
        let! scale = xAttr "Scale" convert
        let! style = xAttrRequire "Style" convert
        let! implicit = xAttrRequire "Implicit" convert
        let! styleExpression = xElementOptional parseScalarExpressionType ("Style", ns) convert
        let! scalarOperator = xElementRequire ("ScalarOperator", ns) convert |> Result.bind parseScalarType
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

and parseArithmeticType (arithmetic : Linq.XElement) : Result<ArithmeticType, _> =
    Result.builder {
        let! operation = xAttrRequire "Operation" arithmetic |> Result.bind parseArithmeticOperation
        let! scalarOperators = xElements ("ScalarOperator", ns) arithmetic |> forAll parseScalarType
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseLogicalType (logical : Linq.XElement) : Result<LogicalType, _> =
    Result.builder {
        let! operation = xAttrRequire "Operation" logical |> Result.bind parseLogicalOperation
        let! scalarOperators = xElements ("ScalarOperator", ns) logical |> forAll parseScalarType
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseAggregateType (aggregate : Linq.XElement) : Result<AggregateType, _> =
    Result.builder {
        let! aggType = xAttrRequire "AggType" aggregate
        let! distinct = xAttrRequire "Distinct" aggregate
        let! scalarOperators = xElements ("ScalarOperator", ns) aggregate |> forAll parseScalarType
        return { 
            AggType = aggType
            Distinct = distinct
            ScalarOperators = scalarOperators 
        }
    }

and parseUDAggregateType (udAggregate : Linq.XElement) : Result<UDAggregateType, _> =
    Result.builder {
        let! distinct = xAttrRequire "Distinct" udAggregate
        let! udAggObject = xElementOptional parseObjectType ("UDAggObject", ns) udAggregate
        let! scalarOperators = xElements ("ScalarOperator", ns) udAggregate |> forAll parseScalarType
        return { 
            Distinct = distinct
            UDAggObject = udAggObject
            ScalarOperators = scalarOperators 
        }
    }

and parseIntrinsicType (intrinsic : Linq.XElement) : Result<IntrinsicType, _> =
    Result.builder {
        let! functionName = xAttrRequire "FunctionName" intrinsic
        let! scalarOperators = xElements ("ScalarOperator", ns) intrinsic |> forAll parseScalarType
        return { 
            FunctionName = functionName
            ScalarOperators = scalarOperators 
        }
    }

and parseUDFType (udf : Linq.XElement) : Result<UDFType, _> =
    Result.builder {
        let! functionName = xAttrRequire "FunctionName" udf
        let! isClrFunction = xAttr "IsClrFunction" udf
        let! clrFunction = xElementOptional parseCLRFunctionType ("CLRFunction", ns) udf
        let! scalarOperators = xElements ("ScalarOperator", ns) udf |> forAll parseScalarType
        return { 
            FunctionName = functionName
            IsClrFunction = isClrFunction
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseUDTMethodType (udtMethod : Linq.XElement) : Result<UDTMethodType, _> =
    Result.builder {
        let! clrFunction = xElementOptional parseCLRFunctionType ("CLRFunction", ns) udtMethod
        let! scalarOperators = xElements ("ScalarOperator", ns) udtMethod |> forAll parseScalarType
        return { 
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseConditionalType (ifExpr : Linq.XElement) : Result<ConditionalType, _> =
    Result.builder {
        let! condition = xElementRequire ("Condition", ns) ifExpr |> Result.bind parseScalarExpressionType
        let! then_ = xElementRequire ("Then", ns) ifExpr |> Result.bind parseScalarExpressionType
        let! else_ = xElementRequire ("Else", ns) ifExpr |> Result.bind parseScalarExpressionType
        return { 
            Condition = condition
            Then = then_
            Else = else_ 
        }
    }

and parseScalarExpressionType (scalarExpr : Linq.XElement) : Result<ScalarExpressionType, _> =
    Result.builder {
        let! scalarOperator = xElementRequire ("ScalarOperator", ns) scalarExpr |> Result.bind parseScalarType
        return { ScalarOperator = scalarOperator }
    }

and parseAssignTargetType (element : Linq.XElement) : Result<AssignTargetType, _> =
    match element.Name.LocalName with
    | "ColumnReference" -> 
        parseColumnReferenceType element |> Result.map AssignTargetType.ColumnRef
    | "ScalarOperator" -> 
        parseScalarType element |> Result.map AssignTargetType.ScalarOp
    | name -> Result.Errorf "Expected ColumnReference or ScalarOperator but got '%s'" name

and parseAssignType (assign : Linq.XElement) : Result<AssignType, _> =
    Result.builder {
        let children = assign.Elements() |> Seq.toList
        match children with
        | [] -> return! Result.Errorf "Assign must have at least 2 children"
        | [_] -> return! Result.Errorf "Assign must have at least 2 children"
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

and parseMultAssignType (multiAssign : Linq.XElement) : Result<MultAssignType, _> =
    Result.builder {
        let! assigns = xElements ("Assign", ns) multiAssign |> forAll parseAssignType
        return { Assigns = assigns }
    }

and parseScalarExpressionListType (scalarExprList : Linq.XElement) : Result<ScalarExpressionListType, _> =
    Result.builder {
        let! scalarOperators = xElements ("ScalarOperator", ns) scalarExprList |> forAll parseScalarType
        return { ScalarOperators = scalarOperators }
    }

and parseSubqueryType (subquery : Linq.XElement) : Result<SubqueryType, _> =
    Result.builder {
        let! operation = xAttrRequire "Operation" subquery |> Result.bind parseSubqueryOperation
        let! scalarOperator = xElementOptional parseScalarType ("ScalarOperator", ns) subquery
        let! relOp = xElementRequire ("RelOp", ns) subquery |> Result.bind parseRelOpType
        return { 
            Operation = operation
            ScalarOperator = scalarOperator
            RelOp = relOp 
        }
    }

and parseScalarType (scalarOperator : Linq.XElement) : Result<ScalarType, _> =
    let parseChild () =
        let childElements = scalarOperator.Elements() |> Seq.toList
        match childElements with
        | [] -> Result.Errorf "ScalarOperator must have at least one child element"
        | child :: _ ->
            match child.Name.LocalName with
            | "Aggregate" -> parseAggregateType child |> Result.map ScalarOperatorKind.Aggregate
            | "Arithmetic" -> parseArithmeticType child |> Result.map ScalarOperatorKind.Arithmetic
            | "Assign" -> parseAssignType child |> Result.map ScalarOperatorKind.Assign
            | "Compare" -> parseCompareType child |> Result.map ScalarOperatorKind.Compare
            | "Const" -> parseConstType child |> Result.map ScalarOperatorKind.Const
            | "Convert" -> parseConvertType child |> Result.map ScalarOperatorKind.Convert
            | "Identifier" -> parseIdentType child |> Result.map ScalarOperatorKind.Identifier
            | "IF" -> parseConditionalType child |> Result.map ScalarOperatorKind.IF
            | "Intrinsic" -> parseIntrinsicType child |> Result.map ScalarOperatorKind.Intrinsic
            | "Logical" -> parseLogicalType child |> Result.map ScalarOperatorKind.Logical
            | "MultipleAssign" -> parseMultAssignType child |> Result.map ScalarOperatorKind.MultipleAssign
            | "ScalarExpressionList" -> parseScalarExpressionListType child |> Result.map ScalarOperatorKind.ScalarExpressionList
            | "Sequence" -> parseScalarSequenceType child |> Result.map ScalarOperatorKind.Sequence
            | "Subquery" -> parseSubqueryType child |> Result.map ScalarOperatorKind.Subquery
            | "UDTMethod" -> parseUDTMethodType child |> Result.map ScalarOperatorKind.UDTMethod
            | "UserDefinedAggregate" -> parseUDAggregateType child |> Result.map ScalarOperatorKind.UserDefinedAggregate
            | "UserDefinedFunction" -> parseUDFType child |> Result.map ScalarOperatorKind.UserDefinedFunction
            | name -> Result.Errorf "Unknown scalar operator type: '%s'" name
    Result.builder {
        let! kind = parseChild()
        let! scalarString = xAttr "ScalarString" scalarOperator
        let! internalInfo = xElementOptional parseInternalInfoType ("InternalInfo", ns) scalarOperator
        return { 
            Kind = kind
            ScalarString = scalarString
            InternalInfo = internalInfo 
        }
    }

and parseRelOpType (relOp : Linq.XElement) : Result<RelOpType, _> =
    Result.builder {
        let! outputList = xElementRequire ("OutputList", ns) relOp |> Result.bind (fun ol -> xElements ("ColumnReference", ns) ol |> forAll parseColumnReferenceType)
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

let parseQueryPlanType (queryPlan : Linq.XElement) : Result<QueryPlanType, _> =
    Result.builder {
        let! degreeOfParallelism = xAttr "DegreeOfParallelism" queryPlan
        let! nonParallelPlanReason = xAttr "NonParallelPlanReason" queryPlan
        let! memoryGrant = xAttr "MemoryGrant" queryPlan
        let! cachedPlanSize = xAttr "CachedPlanSize" queryPlan
        let! compileTime = xAttr "CompileTime" queryPlan
        let! compileCPU = xAttr "CompileCPU" queryPlan
        let! compileMemory = xAttr "CompileMemory" queryPlan
        let! warnings = xElementOptional parseWarningsType ("Warnings", ns) queryPlan 
        let! memoryGrantInfo = xElementOptional parseMemoryGrantType ("MemoryGrantInfo", ns) queryPlan
        let! optimizerHardwareDependentProperties = xElementOptional parseOptimizerHardwareDependentProperties ("OptimizerHardwareDependentProperties", ns) queryPlan
        let! optimizerStatsUsage = xElementOptional parseOptimizerStatsUsage ("OptimizerStatsUsage", ns) queryPlan
        let! relOpType' = xElementRequire ("RelOp", ns) queryPlan
        let! relOpType = parseRelOpType relOpType'
        let! columnReferenceType = xElementOptional (fun e -> xElements ("ColumnReference", ns) e |> forAll parseColumnReferenceType) ("ParameterList", ns) queryPlan
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
        return failwith "NYI: StmtCursor parsing not yet implemented"
    }

let parseStmtReceive (stmtReceive : Linq.XElement) : Result<StmtReceiveType, _> =
    Result.builder {
        let! baseInfo = parseBaseStmtInfoType stmtReceive
        let! operations = xElements ("Operation", ns) stmtReceive |> forAll parseReceiveOperation
        return { 
            BaseInfo = baseInfo
            Operations = operations
        }
    }

let parseStmtUseDb (stmtUseDb : Linq.XElement) : Result<StmtUseDbType, _> =
    Result.builder {
        let! baseInfo = parseBaseStmtInfoType stmtUseDb
        let! database = xAttrRequire "Database" stmtUseDb
        return { 
            BaseInfo = baseInfo
            Database = database
        }
    }

let parseExternalDistributedComputation (externalDistributedComputation : Linq.XElement) : Result<ExternalDistributedComputationType, _> =
    Result.builder {
        let! edcShowplanXml = xAttrRequire "EdcShowplanXml" externalDistributedComputation
        return { 
            EdcShowplanXml = edcShowplanXml 
        }
    }

// ========================================
// Mutual recursion for statement block types
// ========================================

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
        let! thenStmt = xElementRequire ("Then", ns) stmtCond |> Result.bind (fun e -> xElementRequire ("Statements", ns) e |> Result.bind parseStmtBlockType)
        let! elseStmt = xElementOptional (fun e -> xElementRequire ("Statements", ns) e |> Result.bind parseStmtBlockType) ("Else", ns) stmtCond 
        return { 
            BaseInfo = baseInfo
            Condition = {| QueryPlan = queryPlan; UDFs = udfFunctions |}
            Then = thenStmt
            Else = elseStmt
        }
    }

and parseStmtBlockType (stmtType : Linq.XElement) : Result<StmtBlockType, _> =
    match stmtType.Name.LocalName with
    | "StmtSimple" -> parseStmtSimple stmtType |> Result.map StmtBlockType.StmtSimple 
    | "StmtCond" -> parseStmtCond stmtType |> Result.map StmtBlockType.StmtCond 
    | "StmtCursor" -> parseStmtCursor stmtType |> Result.map StmtBlockType.StmtCursor 
    | "StmtReceive" -> parseStmtReceive stmtType |> Result.map StmtBlockType.StmtReceive 
    | "StmtUseDb" -> parseStmtUseDb stmtType |> Result.map StmtBlockType.StmtUseDb 
    | "ExternalDistributedComputation" -> parseExternalDistributedComputation stmtType |> Result.map StmtBlockType.ExternalDistributedComputation 
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
        let! batches = xElements ("Batch", ns) batchSequence |> forAll parseBatch 
        return {
            Version = version
            Build = build
            ClusteredMode = clusteredMode
            BatchSequence = { Batches = batches }
        }
    }