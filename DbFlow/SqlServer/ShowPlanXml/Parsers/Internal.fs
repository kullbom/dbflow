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
        let! onlineInbuildIndex = xAttr "OnlineInbuildIndex" o
        let! onlineIndexBuildMappingIndex = xAttr "OnlineIndexBuildMappingIndex"  o
        let! graphWorkTableType = xAttr "GraphWorkTableType" o
        let! graphWorkTableIdentifier = xAttr "GraphWorkTableIdentifier"  o
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
            OnlineInbuildIndex = onlineInbuildIndex
            OnlineIndexBuildMappingIndex = onlineIndexBuildMappingIndex
            GraphWorkTableType = graphWorkTableType
            GraphWorkTableIdentifier = graphWorkTableIdentifier
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

let parseSpillOccurredWarning (element : Linq.XElement) : PResult<SpillOccurredType, _> =
    PResult.builder {
        let! detail = xAttr "Detail" element
        return { Detail = detail }
    }

let parseSpillToTempDb (element : Linq.XElement) : PResult<SpillToTempDbType, _> =
    PResult.builder {
        let! spillReason = xAttr "SpillReason" element
        let! spilledThreadCount = xAttr "SpilledThreadCount" element
        return { SpillLevel = spillReason; SpilledThreadCount = spilledThreadCount }
    }

let parseWaitWarning (element : Linq.XElement) : PResult<WaitWarningType, _> =
    PResult.builder {
        let! waitType = xAttrReq "WaitType" element
        let! waitTime = xAttr "WaitTime" element
        return { 
            WaitType = waitType
            WaitTime = waitTime
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

let parseSortSpillDetailsWarning (element : Linq.XElement) : PResult<SortSpillDetailsType, _> =
    PResult.builder {
        let! grantedMemoryKb = xAttr "GrantedMemoryKb" element
        let! usedMemoryKb = xAttr "UsedMemoryKb" element
        let! writesToTempDb = xAttr "WritesToTempDb" element
        let! readsFromTempDb = xAttr "ReadsFromTempDb" element
        return { 
            GrantedMemoryKb = grantedMemoryKb 
            UsedMemoryKb = usedMemoryKb
            WritesToTempDb = writesToTempDb 
            ReadsFromTempDb = readsFromTempDb 
        }
    }

let parseHashSpillDetailsWarning (element : Linq.XElement) : PResult<HashSpillDetailsType, _> =
    PResult.builder {
        let! grantedMemoryKb = xAttr "GrantedMemoryKb" element
        let! usedMemoryKb = xAttr "UsedMemoryKb" element
        let! writesToTempDb = xAttr "WritesToTempDb" element
        let! readsFromTempDb = xAttr "ReadsFromTempDb" element
        return { 
            GrantedMemoryKb = grantedMemoryKb 
            UsedMemoryKb = usedMemoryKb
            WritesToTempDb = writesToTempDb 
            ReadsFromTempDb = readsFromTempDb 
        }
    }

let parseExchangeSpillDetailsWarning (element : Linq.XElement) : PResult<ExchangeSpillDetailsType, _> =
    PResult.builder {
        let! writesToTempDb = xAttr "WritesToTempDb" element
        return { WritesToTempDb = writesToTempDb }
    }

let parseMemoryGrantWarning (element : Linq.XElement) : PResult<MemoryGrantWarningInfo, _> =
    PResult.builder {
        let! grantWarningKind = xAttrReq "GrantWarningKind" element
        let! requestedMemory = xAttrReq "RequestedMemory" element
        let! grantedMemory = xAttrReq "GrantedMemory" element
        let! maxUsedMemory = xAttrReq "MaxUsedMemory" element
        return { 
            GrantWarningKind = grantWarningKind
            RequestedMemory = requestedMemory
            GrantedMemory = grantedMemory
            MaxUsedMemory = maxUsedMemory 
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
        let! (statisticsInfo, rest) = xElementMany (ensureName ("StatisticsInfo", ns) parseStatsInfo) (xElementsAll optimizerStatsUsage)
        do! xElementEnsureEmpty rest

        return { StatisticsInfo = statisticsInfo }
    }

let parseBaseStmtInfoType (stmtSimple : Linq.XElement) : PResult<BaseStmtInfoType * _, _> =
    PResult.builder {
        let! (statementSetOptions, rest) = xElement (ensureName ("StatementSetOptions", ns) parseSetOptionsType) (xElementsAll stmtSimple)
        
        let! statementCompId = xAttr "StatementCompId" stmtSimple
        let! statementEstRows = xAttr "StatementEstRows" stmtSimple
        let! statementId = xAttr "StatementId" stmtSimple
        let! queryCompilationReplay = xAttr "QueryCompilationReplay" stmtSimple
        let! statementOptmLevel = xAttr "StatementOptmLevel" stmtSimple
        let! statementOptmEarlyAbortReason = xAttr "StatementOptmEarlyAbortReason" stmtSimple
        let! cardinalityEstimationModelVersion = xAttr "CardinalityEstimationModelVersion" stmtSimple
        let! statementSubTreeCost = xAttr "StatementSubTreeCost" stmtSimple
        let! statementText = xAttr "StatementText" stmtSimple
        let! statementType = xAttr "StatementType" stmtSimple
        let! templatePlanGuideDB = xAttr "TemplatePlanGuideDB" stmtSimple
        let! templatePlanGuideName = xAttr "TemplatePlanGuideName" stmtSimple
        let! planGuideDB = xAttr "PlanGuideDB" stmtSimple
        let! planGuideName = xAttr "PlanGuideName" stmtSimple
        let! parameterizedText = xAttr "ParameterizedText" stmtSimple
        let! parameterizedPlanHandle = xAttr "ParameterizedPlanHandle" stmtSimple
        let! queryHash = xAttr "QueryHash" stmtSimple
        let! queryPlanHash = xAttr "QueryPlanHash" stmtSimple
        let! retrievedFromCache = xAttr "RetrievedFromCache" stmtSimple
        let! statementSqlHandle = xAttr "StatementSqlHandle" stmtSimple
        let! databaseContextSettingsId = xAttr "DatabaseContextSettingsId" stmtSimple
        let! parentObjectId = xAttr "ParentObjectId" stmtSimple
        let! batchSqlHandle = xAttr "BatchSqlHandle" stmtSimple
        let! statementParameterizationType = xAttr "StatementParameterizationType" stmtSimple
        let! securityPolicyApplied = xAttr "SecurityPolicyApplied" stmtSimple
        let! batchModeOnRowStoreUsed = xAttr "BatchModeOnRowStoreUsed" stmtSimple
        let! queryStoreStatementHintId = xAttr "QueryStoreStatementHintId" stmtSimple
        let! queryStoreStatementHintText = xAttr "QueryStoreStatementHintText" stmtSimple
        let! queryStoreStatementHintSource = xAttr "QueryStoreStatementHintSource" stmtSimple
        let! containsLedgerTables = xAttr "ContainsLedgerTables" stmtSimple

        return {
            StatementSetOptions = statementSetOptions
            StatementCompId = statementCompId
            StatementEstRows = statementEstRows
            StatementId = statementId
            QueryCompilationReplay = queryCompilationReplay
            StatementOptmLevel = statementOptmLevel
            StatementOptmEarlyAbortReason = statementOptmEarlyAbortReason
            CardinalityEstimationModelVersion = cardinalityEstimationModelVersion
            StatementSubTreeCost = statementSubTreeCost
            StatementText = statementText
            StatementType = statementType
            TemplatePlanGuideDB = templatePlanGuideDB
            TemplatePlanGuideName = templatePlanGuideName
            PlanGuideDB = planGuideDB
            PlanGuideName = planGuideName
            ParameterizedText = parameterizedText
            ParameterizedPlanHandle = parameterizedPlanHandle
            QueryHash = queryHash
            QueryPlanHash = queryPlanHash
            RetrievedFromCache = retrievedFromCache
            StatementSqlHandle = statementSqlHandle
            DatabaseContextSettingsId = databaseContextSettingsId
            ParentObjectId = parentObjectId
            BatchSqlHandle = batchSqlHandle
            StatementParameterizationType = statementParameterizationType
            SecurityPolicyApplied = securityPolicyApplied
            BatchModeOnRowStoreUsed = batchModeOnRowStoreUsed
            QueryStoreStatementHintId = queryStoreStatementHintId
            QueryStoreStatementHintText = queryStoreStatementHintText
            QueryStoreStatementHintSource = queryStoreStatementHintSource
            ContainsLedgerTables = containsLedgerTables
        },
        rest
    }

let parseRunTimeInformation (runTimeInformation : Linq.XElement) : PResult<RunTimeInformationType, _> = Failf "NYI"

let parseRunTimePartitionSummary (runTimePartitionSummary : Linq.XElement) : PResult<RunTimePartitionSummaryType, _> = Failf "NYI"

// ========================================
// Single mutual recursion block covering:
//   ColumnReference ↔ ScalarType ↔ SubqueryType ↔ RelOpType
// ========================================



let rec parseColumnReferenceType (columnReference : Linq.XElement) : PResult<ColumnReferenceType, _> =
    PResult.builder {
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
        
        let! (scalarOperator, rest) = xElement (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll columnReference)
        let! (internalInfo, rest) = xElement (ensureName ("InternalInfo", ns) parseInternalInfoType) rest
        do! xElementEnsureEmpty rest
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
        let! table = xAttr "Table" ident
        
        let! (columnReference, rest) = xElement (ensureName ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll ident)
        do! xElementEnsureEmpty rest

        return { 
            ColumnReference = columnReference
            Table = table 
        }
    }

and parseCompareType (compare : Linq.XElement) : PResult<CompareType, _> =
    PResult.builder {
        let! compareOp = xAttrReqP "CompareOp" parseCompareOp compare 
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll compare)
        do! xElementEnsureEmpty rest
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
        
        let! (styleExpression, rest) = xElement (ensureName ("Style", ns) parseScalarExpressionType) (xElementsAll convert)
        let! (scalarOperator, rest) = xElementReq (ensureName ("ScalarOperator", ns) parseScalarType) rest
        do! xElementEnsureEmpty rest
        
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
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll arithmetic)
        do! xElementEnsureEmpty rest
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseLogicalType (logical : Linq.XElement) : PResult<LogicalType, _> =
    PResult.builder {
        let! operation = xAttrReqP "Operation" parseLogicalOperation logical 
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll logical)
        do! xElementEnsureEmpty rest
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseAggregateType (aggregate : Linq.XElement) : PResult<AggregateType, _> =
    PResult.builder {
        let! aggType = xAttrReq "AggType" aggregate
        let! distinct = xAttrReq "Distinct" aggregate
        
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll aggregate)
        do! xElementEnsureEmpty rest

        return { 
            AggType = aggType
            Distinct = distinct
            ScalarOperators = scalarOperators 
        }
    }

and parseUDAggregateType (udAggregate : Linq.XElement) : PResult<UDAggregateType, _> =
    PResult.builder {
        let! distinct = xAttrReq "Distinct" udAggregate
        
        let! (udAggObject, rest) = xElement (ensureName ("UDAggObject", ns) parseObjectType) (xElementsAll udAggregate)
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) rest
        do! xElementEnsureEmpty rest

        return { 
            Distinct = distinct
            UDAggObject = udAggObject
            ScalarOperators = scalarOperators 
        }
    }

and parseIntrinsicType (intrinsic : Linq.XElement) : PResult<IntrinsicType, _> =
    PResult.builder {
        let! functionName = xAttrReq "FunctionName" intrinsic
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll intrinsic)
        do! xElementEnsureEmpty rest
        return { 
            FunctionName = functionName
            ScalarOperators = scalarOperators 
        }
    }

and parseUDFType (udf : Linq.XElement) : PResult<UDFType, _> =
    PResult.builder {
        let! functionName = xAttrReq "FunctionName" udf
        let! isClrFunction = xAttr "IsClrFunction" udf
        
        let! (clrFunction, rest) = xElement (ensureName ("CLRFunction", ns) parseCLRFunctionType) (xElementsAll udf)
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) rest
        do! xElementEnsureEmpty rest

        return { 
            FunctionName = functionName
            IsClrFunction = isClrFunction
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseUDTMethodType (udtMethod : Linq.XElement) : PResult<UDTMethodType, _> =
    PResult.builder {
        let! (clrFunction, rest) = xElement (ensureName ("CLRFunction", ns) parseCLRFunctionType) (xElementsAll udtMethod)
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) rest 
        do! xElementEnsureEmpty rest

        return { 
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseConditionalType (ifExpr : Linq.XElement) : PResult<ConditionalType, _> =
    PResult.builder {
        let! (condition, rest) = xElementReq (ensureName ("Condition", ns) parseScalarExpressionType) (xElementsAll ifExpr)
        let! (then_, rest) = xElementReq (ensureName ("Then", ns) parseScalarExpressionType) rest
        let! (else_, rest) = xElementReq (ensureName ("Else", ns) parseScalarExpressionType) rest
        do! xElementEnsureEmpty rest
        return { 
            Condition = condition
            Then = then_
            Else = else_ 
        }
    }

and parseScalarExpressionType (scalarExpr : Linq.XElement) : PResult<ScalarExpressionType, _> =
    PResult.builder {
        let! (scalarOperator, rest) = xElementReq (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll scalarExpr)
        do! xElementEnsureEmpty rest

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
        let! (target, rest) = xElementReq parseAssignTargetType (xElementsAll assign)
        let! (scalarOperator, rest) = xElementReq (ensureName ("ScalarOperator", ns) parseScalarType) rest
        let! (sourceColumns, rest) = xElementMany (ensureName ("SourceColumn", ns) parseColumnReferenceType) rest
        let! (targetColumns, rest) = xElementMany (ensureName ("TargetColumn", ns) parseColumnReferenceType) rest
        do! xElementEnsureEmpty rest
        return { 
            Target = target
            ScalarOperator = scalarOperator
            SourceColumns = sourceColumns
            TargetColumns = targetColumns 
        }
    }

and parseMultAssignType (multiAssign : Linq.XElement) : PResult<MultAssignType, _> =
    PResult.builder {
        let! (assigns, rest) = xElementMany (ensureName ("Assign", ns) parseAssignType) (xElementsAll multiAssign)
        do! xElementEnsureEmpty rest

        return { Assigns = assigns }
    }

and parseScalarExpressionListType (scalarExprList : Linq.XElement) : PResult<ScalarExpressionListType, _> =
    PResult.builder {
        let! (scalarOperators, rest) = xElementMany (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll scalarExprList)
        do! xElementEnsureEmpty rest
        
        return { ScalarOperators = scalarOperators }
    }

and parseSubqueryType (subquery : Linq.XElement) : PResult<SubqueryType, _> =
    PResult.builder {
        let! operation = xAttrReqP "Operation" parseSubqueryOperation subquery 
        
        let! (scalarOperator, rest) = xElement (ensureName ("ScalarOperator", ns) parseScalarType) (xElementsAll subquery)
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest

        return { 
            Operation = operation
            ScalarOperator = scalarOperator
            RelOp = relOp 
        }
    }

and parseScalarTypeKind (scalarKind : Linq.XElement) : PResult<ScalarOperatorKind, _> =
    match scalarKind.Name.LocalName with
    | "Aggregate" -> parseAggregateType scalarKind |> PResult.map ScalarOperatorKind.Aggregate
    | "Arithmetic" -> parseArithmeticType scalarKind |> PResult.map ScalarOperatorKind.Arithmetic
    | "Assign" -> parseAssignType scalarKind |> PResult.map ScalarOperatorKind.Assign
    | "Compare" -> parseCompareType scalarKind |> PResult.map ScalarOperatorKind.Compare
    | "Const" -> parseConstType scalarKind |> PResult.map ScalarOperatorKind.Const
    | "Convert" -> parseConvertType scalarKind |> PResult.map ScalarOperatorKind.Convert
    | "Identifier" -> parseIdentType scalarKind |> PResult.map ScalarOperatorKind.Identifier
    | "IF" -> parseConditionalType scalarKind |> PResult.map ScalarOperatorKind.IF
    | "Intrinsic" -> parseIntrinsicType scalarKind |> PResult.map ScalarOperatorKind.Intrinsic
    | "Logical" -> parseLogicalType scalarKind |> PResult.map ScalarOperatorKind.Logical
    | "MultipleAssign" -> parseMultAssignType scalarKind |> PResult.map ScalarOperatorKind.MultipleAssign
    | "ScalarExpressionList" -> parseScalarExpressionListType scalarKind |> PResult.map ScalarOperatorKind.ScalarExpressionList
    | "Sequence" -> parseScalarSequenceType scalarKind |> PResult.map ScalarOperatorKind.Sequence
    | "Subquery" -> parseSubqueryType scalarKind |> PResult.map ScalarOperatorKind.Subquery
    | "UDTMethod" -> parseUDTMethodType scalarKind |> PResult.map ScalarOperatorKind.UDTMethod
    | "UserDefinedAggregate" -> parseUDAggregateType scalarKind |> PResult.map ScalarOperatorKind.UserDefinedAggregate
    | "UserDefinedFunction" -> parseUDFType scalarKind |> PResult.map ScalarOperatorKind.UserDefinedFunction
    | name -> Failf "Unknown scalar operator type: '%s'" name

and parseScalarType (scalarOperator : Linq.XElement) : PResult<ScalarType, _> =
    PResult.builder {
        let! scalarString = xAttr "ScalarString" scalarOperator
        
        let! (kind, rest) = xElementReq parseScalarTypeKind (xElementsAll scalarOperator)
        let! (internalInfo, rest) = xElement (ensureName ("InternalInfo", ns) parseInternalInfoType) rest
        do! xElementEnsureEmpty rest

        return { 
            ScalarString = scalarString
            Kind = kind
            InternalInfo = internalInfo 
        }
    }

and parseColumnReferenceListType (columnReferenceList : Linq.XElement) : PResult<ColumnReferenceListType, _> =
    PResult.builder {
        let! (columnReferences, rest) = xElementMany (ensureName ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll columnReferenceList)
        do! xElementEnsureEmpty rest
        return { ColumnReferences = columnReferences }
    }

and parseSingleColumnReferenceType (singleColumnReference : Linq.XElement) : PResult<SingleColumnReferenceType, _> =
    PResult.builder {
        let! (columnReference, rest) = xElementReq (ensureName ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll singleColumnReference)
        do! xElementEnsureEmpty rest
        return { ColumnReference = columnReference }
    }

and parseWarning (warning : Linq.XElement) : PResult<Warning, _> =
    match warning.Name.LocalName with
    | "SpillOccurred" -> 
        parseSpillOccurredWarning warning |> PResult.map Warning.SpillOccurred
    | "ColumnsWithNoStatistics" -> 
        parseColumnReferenceListType warning |> PResult.map Warning.ColumnsWithNoStatistics
    | "ColumnsWithStaleStatistics" -> 
        parseColumnReferenceListType warning |> PResult.map Warning.ColumnsWithStaleStatistics
    | "SpillToTempDb" -> 
        parseSpillToTempDb warning |> PResult.map Warning.SpillToTempDb
    | "Wait" -> 
        parseWaitWarning warning |> PResult.map Warning.Wait
    | "PlanAffectingConvert" -> 
        parseAffectingConvertWarning warning |> PResult.map Warning.PlanAffectingConvert
    | "SortSpillDetails" -> 
        parseSortSpillDetailsWarning warning |> PResult.map Warning.SortSpillDetails
    | "HashSpillDetails" -> 
        parseHashSpillDetailsWarning warning |> PResult.map Warning.HashSpillDetails
    | "ExchangeSpillDetails" -> 
        parseExchangeSpillDetailsWarning warning |> PResult.map Warning.ExchangeSpillDetails 
    | "MemoryGrantWarning" -> 
        parseMemoryGrantWarning warning |> PResult.map Warning.MemoryGrantWarning
    
    | name -> 
        Failf "Expected warning but got '%s'" name

and parseWarningsType (warningsType : Linq.XElement) : PResult<WarningsType, _> =
    PResult.builder {
        let warningElements = xElementsAll warningsType
        let! (warnings, rest) = xElementMany parseWarning warningElements
        do! xElementEnsureEmpty rest
        let! noJoinPredicate = xAttr "NoJoinPredicate" warningsType
        let! spatialGuess = xAttr "SpatialGuess" warningsType
        let! unmatchedIndexes = xAttr "UnmatchedIndexes" warningsType
        let! fullUpdateForOnlineIndexBuild = xAttr "FullUpdateForOnlineIndexBuild" warningsType
        return { 
            Warnings = warnings
            NoJoinPredicate = noJoinPredicate
            SpatialGuess = spatialGuess
            UnmatchedIndexes = unmatchedIndexes
            FullUpdateForOnlineIndexBuild = fullUpdateForOnlineIndexBuild
        }
    }
and parseDefinedValueType (definedValue : Linq.XElement) : PResult<DefinedValueType, _> =
    PResult.builder {
        let! (value, rest) = xElementReq (ensureName ("Value", ns) parseScalarType) (xElementsAll definedValue)
        let! (sourceColumns, rest) = xElement (ensureName ("SourceColumn", ns) parseColumnReferenceListType) rest
        let! (targetColumns, rest) = xElement (ensureName ("TargetColumn", ns) parseColumnReferenceListType) rest
        return { 
            Value = value
            SourceColumns = sourceColumns
            TargetColumns = targetColumns
        }
    }
and parseDefinedValuesListType (definedValuesList : Linq.XElement) : PResult<DefinedValuesListType, _> =
    PResult.builder {
        let! (definedValues, rest) = xElementMany (ensureName ("DefinedValue", ns) parseDefinedValueType) (xElementsAll definedValuesList)
        do! xElementEnsureEmpty rest
        return { DefinedValues = definedValues }
    }

and parseRelOpBase (relOp : Linq.XElement) : PResult<RelOpBaseType*_, _> =
    PResult.builder {
        let! (definedValues, rest) = xElement (ensureName ("DefinedValues", ns) parseDefinedValuesListType) (xElementsAll relOp)
        let! (internalInfo, rest) = xElement (ensureName ("InternalInfo", ns) parseInternalInfoType) rest
        return { 
            DefinedValues = definedValues
            InternalInfo = internalInfo
        }, rest
    }

and parseStarJoinInfoType (starJoinInfo : Linq.XElement) : PResult<StarJoinInfoType, _> =
    PResult.builder {
        let! root = xAttr "Root" starJoinInfo
        let! operationType = xAttrReq "OperationType" starJoinInfo
        return { 
            Root = root
            OperationType = operationType
        }
    }

and parseAdaptiveJoinType (adaptiveJoin : Linq.XElement) : PResult<AdaptiveJoinType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase adaptiveJoin 
        let! (hashKeysBuild, rest) = xElement (ensureName ("HashKeysBuild", ns) parseColumnReferenceListType) rest
        let! (hashKeysProbe, rest) = xElement (ensureName ("HashKeysProbe", ns) parseColumnReferenceListType) rest
        let! (buildResidual, rest) = xElement (ensureName ("BuildResidual", ns) parseScalarExpressionType) rest 
        let! (probeResidual, rest) = xElement (ensureName ("ProbeResidual", ns) parseScalarExpressionType) rest 
        let! (starJoinInfo, rest) = xElement (ensureName ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (predicate, rest) = xElement (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        let! (passThru, rest) = xElement (ensureName ("PassThru", ns) parseScalarExpressionType) rest
        let! (outerReferences, rest) = xElement (ensureName ("OuterReference", ns) parseColumnReferenceListType) rest
        let! (partitionId, rest) = xElement (ensureName ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest

        let! bitmapCreator = xAttr "BitmapCreator" adaptiveJoin
        let! optimized = xAttrReq "Optimized" adaptiveJoin
        let! withOrderedPrefetch = xAttr "WithOrderedPrefetch" adaptiveJoin
        let! withUnorderedPrefetch = xAttr "WithUnorderedPrefetch" adaptiveJoin
        return { 
            Base = relOpBase
            HashKeysBuild = hashKeysBuild
            HashKeysProbe = hashKeysProbe
            BuildResidual = buildResidual
            ProbeResidual = probeResidual
            StarJoinInfo = starJoinInfo
            Predicate = predicate
            PassThru = passThru
            OuterReferences = outerReferences
            PartitionId = partitionId
            RelOps = relOps // #3#

            BitmapCreator = bitmapCreator
            Optimized = optimized
            WithOrderedPrefetch = withOrderedPrefetch
            WithUnorderedPrefetch = withUnorderedPrefetch
        }
    }

and parseJoinType (join : Linq.XElement) : PResult<JoinType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase join 
        let! (predicates, rest) = xElementMany (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        let! (probes, rest) = xElementMany (ensureName ("Probe", ns) parseSingleColumnReferenceType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        return { 
            Base = relOpBase
            Predicates = predicates
            Probes = probes
            RelOps = relOps
        }
    }

and parseFilterType (filter : Linq.XElement) : PResult<FilterType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase filter 
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        let! (predicate, rest) = xElementReq (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        do! xElementEnsureEmpty rest
        
        let! startupExpression = xAttrReq "StartupExpression" filter
        return { 
            Base = relOpBase
            RelOp = relOp
            Predicate = predicate
            
            StartupExpression = startupExpression
        }
    }

and parseBatchHashTableBuildType (batchHashTableBuild : Linq.XElement) : PResult<BatchHashTableBuildType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase batchHashTableBuild 
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! bitmapCreator = xAttr "BitmapCreator" batchHashTableBuild
        return { 
            Base = relOpBase
            RelOp = relOp
            BitmapCreator = bitmapCreator
        }
    }

and parseBitmapType (bitmap : Linq.XElement) : PResult<BitmapType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase bitmap 
        let! (hashKeys, rest) = xElementReq (ensureName ("HashKeys", ns) parseColumnReferenceListType) rest
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            HashKeys = hashKeys
            RelOp = relOp
        }
    }

and parseCollapseType (collapse : Linq.XElement) : PResult<CollapseType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase collapse 
        let! (groupBy, rest) = xElementReq (ensureName ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            GroupBy = groupBy
            RelOp = relOp
        }
    }

and parseComputeScalarType (computeScalar : Linq.XElement) : PResult<ComputeScalarType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase computeScalar 
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! computeSequence = xAttr "ComputeSequence" computeScalar

        return { 
            Base = relOpBase
            RelOp = relOp
            ComputeSequence = computeSequence
        }
    }

and parseConcatType (concat : Linq.XElement) : PResult<ConcatType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase concat 
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            RelOps = relOps
        }
    }

and parseConstantScanType (constantScan : Linq.XElement) : PResult<ConstantScanType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase constantScan 
        let! (values, rest) = xElementMany (ensureName ("Value", ns) parseScalarExpressionListType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            Values = values
        }
    }

and parseOutputColumns (outputColumns : Linq.XElement) : PResult<OutputColumnsType, _> =
    PResult.builder {
        let! (definedValues, rest) = xElement (ensureName ("DefinedValues", ns) parseDefinedValuesListType) (xElementsAll outputColumns)
        let! (objects, rest) = xElementMany (ensureName ("Objects", ns) parseObjectType) rest
        do! xElementEnsureEmpty rest
        return { DefinedValues = definedValues; Objects = objects }
    }

and parseGetType (get : Linq.XElement) : PResult<GetType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase get 
        let! (bookmarks, rest) = xElement (ensureName ("Bookmarks", ns) parseColumnReferenceListType) rest
        let! (outputColumns, rest) = xElement (ensureName ("OutputColumns", ns) parseOutputColumns) rest
        let! (generatedData, rest) = xElement (ensureName ("GeneratedData", ns) parseScalarExpressionListType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! numRows = xAttr "NumRows" get
        let! isExternal = xAttr "IsExternal" get
        let! isDistributed = xAttr "IsDistributed" get
        let! isHashDistributed = xAttr "IsHashDistributed" get 
        let! isReplicated = xAttr "IsReplicated" get
        let! isRoundRobin = xAttr "IsRoundRobin" get

        return { 
            Base = relOpBase
            Bookmarks = bookmarks 
            OutputColumns = outputColumns
            GeneratedData = generatedData
            RelOps = relOps

            NumRows = numRows
            IsExternal = isExternal
            IsDistributed = isDistributed
            IsHashDistributed = isHashDistributed
            IsReplicated = isReplicated
            IsRoundRobin = isRoundRobin
        }
    }

and parseRowsetBase (rowset : Linq.XElement) : PResult<RowsetType*_, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase rowset 
        let! (objects, rest) = xElementMany (ensureName ("Object", ns) parseObjectType) rest
        return { 
            Base = relOpBase
            Objects = objects
        }, rest
    }

and parseCreateIndexType (createIndex : Linq.XElement) : PResult<CreateIndexType, _> =
    PResult.builder {
        let! (rowSet, rest) = parseRowsetBase createIndex 
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            RowsetBase = rowSet
            RelOp = relOp
        }
    }

and parseAssignmentMapType (assignmentMap : Linq.XElement) : PResult<AssignmentMapType, _> =
    PResult.builder {
        let! (assigns, rest) = xElementMany (ensureName ("Assign", ns) parseAssignType) (xElementsAll assignmentMap)
        do! xElementEnsureEmpty rest
        return { Assigns = assigns }
    }

and parseParameterizationType (parameterization : Linq.XElement) : PResult<ParameterizationType, _> =
    PResult.builder {
        let! (objects, rest) = xElementMany (ensureName ("Object", ns) parseObjectType) (xElementsAll parameterization)
        do! xElementEnsureEmpty rest
        return { Objects = objects }
    }

and parseDMLOpType (dmlOp : Linq.XElement) : PResult<DMLOpType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase dmlOp 
        let! (assignmentMap, rest) = xElement (ensureName ("AssignmentMap", ns) parseAssignmentMapType) rest
        let! (sourceTable, rest) = xElement (ensureName ("SourceTable", ns) parseParameterizationType) rest
        let! (targetTable, rest) = xElement (ensureName ("TargetTable", ns) parseParameterizationType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            AssignmentMap = assignmentMap 
            SourceTable = sourceTable
            TargetTable = targetTable
            RelOps = relOps
        }
    }

and parseRowsetType (rowset : Linq.XElement) : PResult<RowsetType, _> =
    PResult.builder {
        let! (relopBase, rest) = parseRelOpBase rowset 
        let! (objects, rest) = xElementMany (ensureName ("Object", ns) parseObjectType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relopBase
            Objects = objects
        }
    }

and parseUDXType (udx : Linq.XElement) : PResult<UDXType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase udx 
        let! (usedUDXColumns, rest) = xElement (ensureName ("UsedUDXColumns", ns) parseColumnReferenceListType) rest
        let! (relOp, rest) = xElement (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! udxName = xAttrReq "UDXName" udx

        return { 
            Base = relOpBase
            UsedUDXColumns = usedUDXColumns
            RelOp = relOp
            UDXName = udxName
        }
    }

and parseExternalSelectType (externalSelect : Linq.XElement) : PResult<ExternalSelectType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase externalSelect 
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! materializeOperation = xAttr "MaterializeOperation" externalSelect
        let! distributionType = xAttr "DistributionType" externalSelect
        let! isDistributed = xAttr "IsDistributed" externalSelect
        let! isExternal = xAttr "IsExternal" externalSelect
        let! isFull = xAttr "IsFull" externalSelect
        return { 
            Base = relOpBase
            RelOps = relOps
            
            MaterializeOperation = materializeOperation
            DistributionType = distributionType
            IsDistributed = isDistributed
            IsExternal = isExternal
            IsFull = isFull
        }
    }

and parseRemoteType (remote : Linq.XElement) : PResult<RemoteType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase remote 
        do! xElementEnsureEmpty rest
        
        let! remoteDestination = xAttr "RemoteDestination" remote
        let! remoteSource = xAttr "RemoteSource" remote
        let! remoteObject = xAttr "RemoteObject" remote
        return { 
            Base = relOpBase
            RemoteDestination = remoteDestination
            RemoteSource = remoteSource 
            RemoteObject = remoteObject 
        }
    }

and parseScanRangeType (scanRange : Linq.XElement) : PResult<ScanRangeType, _> =
    PResult.builder {
        let! (rangeColumns, rest) = xElementReq (ensureName ("RangeColumns", ns) parseColumnReferenceListType) (xElementsAll scanRange)
        let! (rangeExpressions, rest) = xElementReq (ensureName ("RangeExpressions", ns) parseScalarExpressionListType) rest
        do! xElementEnsureEmpty rest

        let! scanType = xAttrReq "ScanType" scanRange |> PResult.bind parseCompareOp

        return { 
            RangeColumns = rangeColumns
            RangeExpressions = rangeExpressions
            ScanType = scanType
        }
    }

and parseSeekPredicate (seekPredicate : Linq.XElement) : PResult<SeekPredicateType, _> =
    PResult.builder {
        let! (prefix , rest) = xElement (ensureName ("Prefix", ns) parseScanRangeType) (xElementsAll seekPredicate)
        let! (startRange, rest) = xElement (ensureName ("StartRange", ns) parseScanRangeType) rest
        let! (endRange, rest) = xElement (ensureName ("EndRange", ns) parseScanRangeType) rest
        let! (isNotNull, rest) = xElement (ensureName ("IsNotNull", ns) parseSingleColumnReferenceType) rest
        do! xElementEnsureEmpty rest
        return { 
            Prefix = prefix
            StartRange = startRange
            EndRange = endRange
            IsNotNull = isNotNull
        }
    }

and parseSeekPredicateNew (seekPredicateNew : Linq.XElement) : PResult<SeekPredicateNewType, _> =
    PResult.builder {
        let! (seekKeys, rest) = xElementMany1 (ensureName ("SeekKey", ns) parseSeekPredicate) (xElementsAll seekPredicateNew)
        do! xElementEnsureEmpty rest
        return { 
            SeekKeys = seekKeys // 1..2
        }
    }

and parseSeekPredicatePart (seekPredicatePart : Linq.XElement) : PResult<SeekPredicatePartType, _> =
    PResult.builder {
        let! (seekPredicatesNew, rest) = xElementMany1 (ensureName ("SeekPredicatesNew", ns) parseSeekPredicateNew) (xElementsAll seekPredicatePart)
        do! xElementEnsureEmpty rest
        return { 
            SeekPredicatesNew = seekPredicatesNew
        }
    }

and parseSeekPredicatesType (seekPredicates : Linq.XElement) : PResult<SeekPredicatesType, _> =
    PResult.builder {
        let! (seekPredicatesNodes, rest) = xElementMany (ensureName ("SeekPredicateType", ns) parseSeekPredicate) (xElementsAll seekPredicates)
        let! (seekPredicateNew, rest) = xElementMany (ensureName ("SeekPredicateNew", ns) parseSeekPredicateNew) rest
        let! (seekPredicatePart, rest) = xElementMany (ensureName ("SeekPredicatePart", ns) parseSeekPredicatePart) rest
        do! xElementEnsureEmpty rest

        return! 
            match seekPredicatesNodes, seekPredicateNew, seekPredicatePart with
            | _, [], [] -> POk (SeekPredicatesType.SeekPredicate seekPredicatesNodes)
            | [], _, [] -> POk (SeekPredicatesType.SeekPredicateNew seekPredicateNew)
            | [], [], _ -> POk (SeekPredicatesType.SeekPredicatePart seekPredicatePart)
            | _ -> Failf "SeekPredicates can only contain one of SeekPredicate, SeekPredicateNew or SeekPredicatePart"
    }

and parseIndexedViewInfoType (indexedViewInfo : Linq.XElement) : PResult<IndexedViewInfoType, _> =
    PResult.builder {
        let! (objects, rest) = xElementMany (ensureName ("Object", ns) parseObjectType) (xElementsAll indexedViewInfo)
        do! xElementEnsureEmpty rest
        return { Objects = objects }
    }

and parseIndexScanType (indexScan : Linq.XElement) : PResult<IndexScanType, _> =
    PResult.builder {
        let! (rowsetBase, rest) = parseRowsetBase indexScan 
        let! (seekPredicates, rest) = xElement (ensureName ("SeekPredicates", ns) parseSeekPredicatesType) rest
        let! (predicates, rest) = xElementMany (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        let! (partitionId, rest) = xElement (ensureName ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (indexedViewInfo, rest) = xElement (ensureName ("IndexedViewInfo", ns) parseIndexedViewInfoType) rest
        do! xElementEnsureEmpty rest
        
        let! lookup = xAttr "Lookup" indexScan
        let! ordered = xAttrReq "Ordered" indexScan
        let! scanDirection = 
            xAttr "ScanDirection" indexScan
            |> PResult.bind
                 (function 
                    | Some "FORWARD" -> OrderType.FORWARD |> Some |> POk
                    | Some "BACKWARD" -> OrderType.BACKWARD |> Some |> POk
                    | Some name -> Failf "Invalid ScanDirection value: '%s'" name
                    | None -> POk None)
        let! forcedIndex = xAttr "ForcedIndex" indexScan
        let! forceSeek = xAttr "ForceSeek" indexScan
        let! forceSeekColumnCount = xAttr "ForceSeekColumnCount" indexScan
        let! forceScan = xAttr "ForceScan" indexScan
        let! noExpandHint = xAttr "NoExpandHint" indexScan
        let! storage = 
            xAttr "Storage" indexScan
            |> PResult.bind
                 (function 
                    | Some "ColumnStore" -> StorageType.ColumnStore |> Some |> POk
                    | Some "MemoryOptimized" -> StorageType.MemoryOptimized |> Some |> POk
                    | Some "RowStore" -> StorageType.RowStore |> Some |> POk
                    | Some name -> Failf "Invalid StorageType value: '%s'" name
                    | None -> POk None)
        let! dynamicSeek = xAttr "DynamicSeek" indexScan
        let! sbsFileUrl = xAttr "SBSFileUrl" indexScan
        return { 
            RowsetBase = rowsetBase 
            SeekPredicates = seekPredicates 
            Predicates = predicates
            PartitionId = partitionId
            IndexedViewInfo = indexedViewInfo
            
            Lookup = lookup
            Ordered = ordered 
            ScanDirection = scanDirection
            ForcedIndex = forcedIndex
            ForceSeek = forceSeek
            ForceSeekColumnCount = forceSeekColumnCount
            ForceScan = forceScan
            NoExpandHint = noExpandHint
            Storage = storage
            DynamicSeek = dynamicSeek
            SBSFileUrl = sbsFileUrl
        }
    }
and parseForeignKeyReferenceCheckType (fkCheck : Linq.XElement) : PResult<ForeignKeyReferenceCheckType, _> =
    PResult.builder {
        let! (indexScan, rest) = xElementReq (ensureName ("IndexScan", ns) parseIndexScanType) (xElementsAll fkCheck) 
        do! xElementEnsureEmpty rest
        return { 
            IndexScan = indexScan
        }
    }
and parseForeignKeyReferencesCheckType (fkCheck : Linq.XElement) : PResult<ForeignKeyReferencesCheckType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase fkCheck 
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        let! (foreignKeyReferenceChecks, rest) = xElementMany (ensureName ("ForeignKeyReferenceCheck", ns) parseForeignKeyReferenceCheckType) rest
        do! xElementEnsureEmpty rest
        
        let! foreignKeyReferencesCount = xAttr "ForeignKeyReferencesCount" fkCheck
        let! noMatchingIndexCount = xAttr "NoMatchingIndexCount" fkCheck
        let! partialMatchingIndexCount = xAttr "PartialMatchingIndexCount" fkCheck
        return { 
            Base = relOpBase
            RelOp = relOp
            ForeignKeyReferenceChecks = foreignKeyReferenceChecks

            ForeignKeyReferencesCount = foreignKeyReferencesCount
            NoMatchingIndexCount = noMatchingIndexCount
            PartialMatchingIndexCount = partialMatchingIndexCount
        }
    }

and parseGbAggType (gbAgg : Linq.XElement) : PResult<GbAggType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase gbAgg 
        let! (groupBy, rest) = xElement (ensureName ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (aggFunctions, rest) = xElement (ensureName ("Aggregate", ns) parseDefinedValuesListType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! isScalar = xAttr "IsScalar" gbAgg
        let! aggType = xAttr "AggType" gbAgg
        let! hintType = xAttr "HintType" gbAgg

        return { 
            Base = relOpBase
            GroupBy = groupBy
            AggFunctions = aggFunctions
            RelOps = relOps
            
            IsScalar = isScalar
            AggType = aggType
            HintType = hintType
        }
    }

and parseGbApplyType (gbApply : Linq.XElement) : PResult<GbApplyType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase gbApply 
        let! (predicates, rest) = xElementMany (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        let! (aggFunctions, rest) = xElement (ensureName ("AggFunctions", ns) parseDefinedValuesListType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! joinType = xAttr "JoinType" gbApply
        let! aggType = xAttr "AggType" gbApply
        return { 
            Base = relOpBase
            Predicates = predicates
            AggFunctions = aggFunctions
            RelOps = relOps
    
            JoinType = joinType
            AggType = aggType
        }
    }

and parseGenericType (generic : Linq.XElement) : PResult<GenericType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase generic 
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            RelOps = relOps
        }
    }

and parseHashType (hash : Linq.XElement) : PResult<HashType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase hash 
        let! (hashKeysBuild, rest) = xElement (ensureName ("HashKeysBuild", ns) parseColumnReferenceListType) rest
        let! (hashKeysProbe, rest) = xElement (ensureName ("HashKeysProbe", ns) parseColumnReferenceListType) rest
        let! (buildResidual, rest) = xElement (ensureName ("BuildResidual", ns) parseScalarExpressionType) rest
        let! (probeResidual, rest) = xElement (ensureName ("ProbeResidual", ns) parseScalarExpressionType) rest
        let! (starJoinInfo, rest) = xElement (ensureName ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! bitmapCreator = xAttr "BitmapCreator" hash
        return { 
            Base = relOpBase
            HashKeysBuild = hashKeysBuild
            HashKeysProbe = hashKeysProbe
            BuildResidual = buildResidual
            ProbeResidual = probeResidual
            StarJoinInfo = starJoinInfo
            RelOps = relOps // 1-2
            
            BitmapCreator = bitmapCreator
        }
    }

and parseGroupingSetReferenceType (groupingSetReference : Linq.XElement) : PResult<GroupingSetReferenceType, _> =
    PResult.builder {
        let! value = xAttrReq "Value" groupingSetReference
        return { Value = value }
    }
and parseGroupingSetListType (groupingSetList : Linq.XElement) : PResult<GroupingSetListType, _> =
    PResult.builder {
        let! (groupingSets, rest) = xElementMany (ensureName ("GroupingSet", ns) parseGroupingSetReferenceType) (xElementsAll groupingSetList)
        do! xElementEnsureEmpty rest
        return { GroupingSets = groupingSets }
    }

and parseLocalCubeType (localCube : Linq.XElement) : PResult<LocalCubeType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase localCube 
        let! (groupBy, rest) = xElement (ensureName ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (groupingSets, rest) = xElement (ensureName ("GroupingSets", ns) parseGroupingSetListType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            GroupBy = groupBy
            GroupingSets = groupingSets
            RelOps = relOps
        }
    }

and parseMergeType (merge : Linq.XElement) : PResult<MergeType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase merge 
        let! (innerSideJoinColumns, rest) = xElement (ensureName ("InnerSideJoinColumns", ns) parseColumnReferenceListType) rest
        let! (outerSideJoinColumns, rest) = xElement (ensureName ("OuterSideJoinColumns", ns) parseColumnReferenceListType) rest
        let! (residual, rest) = xElement (ensureName ("Residual", ns) parseScalarExpressionType) rest
        let! (passThru, rest) = xElement (ensureName ("PassThru", ns) parseScalarExpressionType) rest
        let! (starJoinInfo, rest) = xElement (ensureName ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! manyToMany = xAttr "ManyToMany" merge
        return { 
            Base = relOpBase
            InnerSideJoinColumns = innerSideJoinColumns
            OuterSideJoinColumns = outerSideJoinColumns
            Residual = residual
            PassThru = passThru
            StarJoinInfo = starJoinInfo
            RelOps = relOps

            ManyToMany = manyToMany
        }
    }

and parseSimpleIteratorOneChildType (simpleIteratorOneChild : Linq.XElement) : PResult<SimpleIteratorOneChildType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase simpleIteratorOneChild 
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        return { 
            Base = relOpBase
            RelOp = relOp
        }
    }

and parseResourceEstimateType (resourceEstimate : Linq.XElement) : PResult<ResourceEstimateType, _> =
    PResult.builder {
        let! nodeCount = xAttr "NodeCount" resourceEstimate
        let! dop = xAttr "Dop" resourceEstimate
        let! memoryInBytes = xAttr "MemoryInBytes" resourceEstimate
        let! diskWrittenInBytes = xAttr "DiskWrittenInBytes" resourceEstimate
        let! scalable = xAttr "Scalable" resourceEstimate
        return { 
            NodeCount = nodeCount
            Dop = dop
            MemoryInBytes = memoryInBytes
            DiskWrittenInBytes = diskWrittenInBytes
            Scalable = scalable
        }
    }

and parseMoveType (move : Linq.XElement) : PResult<MoveType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase move 
        let! (distributionKey, rest) = xElement (ensureName ("DistributionKey", ns) parseColumnReferenceListType) rest
        let! (resourceEstimate, rest) = xElement (ensureName ("ResourceEstimate", ns) parseResourceEstimateType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest

        let! moveType = xAttr "MoveType" move
        let! distributionType = xAttr "DistributionType" move
        let! isDistributed = xAttr "IsDistributed" move
        let! isExternal = xAttr "IsExternal" move
        let! isFull = xAttr "IsFull" move
        return { 
            Base = relOpBase
            DistributionKey = distributionKey
            ResourceEstimate = resourceEstimate
            RelOps = relOps
            
            MoveType = moveType
            DistributionType = distributionType
            IsDistributed = isDistributed
            IsExternal = isExternal
            IsFull = isFull
        }
    }

and parseNestedLoopsType (nestedLoops : Linq.XElement) : PResult<NestedLoopsType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase nestedLoops 
        let! (predicate, rest) = xElement (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        let! (passThru, rest) = xElement (ensureName ("PassThru", ns) parseScalarExpressionType) rest
        let! (outerReferences, rest) = xElement (ensureName ("OuterReferences", ns) parseColumnReferenceListType) rest
        let! (partitionId, rest) = xElement (ensureName ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (probeColumn, rest) = xElement (ensureName ("ProbeColumn", ns) parseSingleColumnReferenceType) rest
        let! (starJoinInfo, rest) = xElement (ensureName ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (relOps, rest) = xElementMany (ensureName ("RelOp", ns) parseRelOpType) rest
        do! xElementEnsureEmpty rest
        
        let! optimized = xAttrReq "Optimized" nestedLoops
        let! withOrderedPrefetch = xAttr "WithOrderedPrefetch" nestedLoops
        let! withUnorderedPrefetch = xAttr "WithUnorderedPrefetch" nestedLoops
        return { 
            Base = relOpBase
            Predicate = predicate
            PassThru = passThru
            OuterReferences = outerReferences
            PartitionId = partitionId
            ProbeColumn = probeColumn
            StarJoinInfo = starJoinInfo
            RelOps = relOps // 2
            
            Optimized = optimized
            WithOrderedPrefetch = withOrderedPrefetch
            WithUnorderedPrefetch = withUnorderedPrefetch
        }
    }

and parseOrderByColumnType (orderByColumn : Linq.XElement) : PResult<OrderByColumnType, _> =
    PResult.builder {
        let! (columnReference, rest) = xElementReq (ensureName ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll orderByColumn)
        do! xElementEnsureEmpty rest
        let! ascending = xAttrReq "Ascending" orderByColumn
        return { ColumnReference = columnReference; Ascending = ascending }
    }

and parseOrderByType (orderBy : Linq.XElement) : PResult<OrderByType, _> =
    PResult.builder {
        let! (orderByColumns, rest) = xElementMany (ensureName ("OrderColumn", ns) parseOrderByColumnType) (xElementsAll orderBy)
        do! xElementEnsureEmpty rest
        return { OrderByColumns = orderByColumns }
    }

and parseActivationInfoType (activationInfo : Linq.XElement) : PResult<ActivationInfoType, _> =
    PResult.builder {
        let! object = 
            xAttr "Object" activationInfo
            |> PResult.bind (function
                | Some "" -> Some ObjectType |> POk
                | None -> POk None)
        let! type_ = xAttrReq "Type" activationInfo
        let! fragmentElimination = xAttr "FragmentElimination" activationInfo
        return { 
            Object = object 
            Type = type_
            FragmentElimination = fragmentElimination 
        }
    }

and parseParallelismType (parallelism : Linq.XElement) : PResult<ParallelismType, _> =
    PResult.builder {
        let! (relOpBase, rest) = parseRelOpBase parallelism 
        let! (partitionColumns, rest) = xElement (ensureName ("PartitionColumns", ns) parseColumnReferenceListType) rest
        let! (orderBy, rest) = xElement (ensureName ("OrderBy", ns) parseOrderByType) rest
        let! (hashKeys, rest) = xElement (ensureName ("HashKeys", ns) parseColumnReferenceListType) rest
        let! (probeColumn, rest) = xElement (ensureName ("ProbeColumn", ns) parseSingleColumnReferenceType) rest
        let! (predicate, rest) = xElement (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        let! (activation, rest) = xElement (ensureName ("Activation", ns) parseActivationInfoType) rest
        let! (brickRouting, rest) = xElement (ensureName ("BrickRouting", ns) parseBrickRoutingType) rest
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        let! (partitioningType, rest) = xElement (ensureName ("PartitioningType", ns) parsePartitionType) rest
        do! xElementEnsureEmpty rest
        
        let! remoting = xAttr "Remoting" parallelism
        let! localParallelism = xAttr "LocalParallelism" parallelism
        let! inRow = xAttr "InRow" parallelism
        return { 
            Base = relOpBase
            PartitionColumns = partitionColumns
            OrderBy = orderBy
            HashKeys = hashKeys
            ProbeColumn = probeColumn
            Predicate = predicate
            Activation = activation
            BrickRouting = brickRouting
            RelOp = relOp
            PartitioningType = partitioningType
        
            Remoting = remoting
            LocalParallelism = localParallelism
            InRow = inRow
        }
    }

and parseRelOpDetails (relOpDetails : Linq.XElement) : PResult<RelOpDetails, _> =
    match relOpDetails.Name.LocalName with
    | "AdaptiveJoin" -> parseAdaptiveJoinType relOpDetails |> PResult.map RelOpDetails.AdaptiveJoin
    | "Apply" -> parseJoinType relOpDetails |> PResult.map RelOpDetails.Apply
    | "Assert" -> parseFilterType relOpDetails |> PResult.map RelOpDetails.Assert
    | "BatchHashTableBuild" -> parseBatchHashTableBuildType relOpDetails |> PResult.map RelOpDetails.BatchHashTableBuild
    | "Bitmap" -> parseBitmapType relOpDetails |> PResult.map RelOpDetails.Bitmap
    | "Collapse" -> parseCollapseType relOpDetails |> PResult.map RelOpDetails.Collapse
    | "ComputeScalar" -> parseComputeScalarType relOpDetails |> PResult.map RelOpDetails.ComputeScalar
    | "Concat" -> parseConcatType relOpDetails |> PResult.map RelOpDetails.Concat
    | "ConstantScan" -> parseConstantScanType relOpDetails |> PResult.map RelOpDetails.ConstantScan
    | "ConstTableGet" -> parseGetType relOpDetails |> PResult.map RelOpDetails.ConstTableGet
    | "CreateIndex" -> parseCreateIndexType relOpDetails |> PResult.map RelOpDetails.CreateIndex
    | "Delete" -> parseDMLOpType relOpDetails |> PResult.map RelOpDetails.Delete
    | "DeletedScan" -> parseRowsetType relOpDetails |> PResult.map RelOpDetails.DeletedScan
    | "Extension" -> parseUDXType relOpDetails |> PResult.map RelOpDetails.Extension
    | "ExternalSelect" -> parseExternalSelectType relOpDetails |> PResult.map RelOpDetails.ExternalSelect
    | "ExtExtractScan" -> parseRemoteType relOpDetails |> PResult.map RelOpDetails.ExtExtractScan
    | "Filter" -> parseFilterType relOpDetails |> PResult.map RelOpDetails.Filter
    | "ForeignKeyReferencesCheck" -> parseForeignKeyReferencesCheckType relOpDetails |> PResult.map RelOpDetails.ForeignKeyReferencesCheck
    | "GbAgg" -> parseGbAggType relOpDetails |> PResult.map RelOpDetails.GbAgg
    | "GbApply" -> parseGbApplyType relOpDetails |> PResult.map RelOpDetails.GbApply
    | "Generic" -> parseGenericType relOpDetails |> PResult.map RelOpDetails.Generic
    | "Get" -> parseGetType relOpDetails |> PResult.map RelOpDetails.Get
    | "Hash" -> parseHashType relOpDetails |> PResult.map RelOpDetails.Hash
    | "IndexScan" -> parseIndexScanType relOpDetails |> PResult.map RelOpDetails.IndexScan
    | "InsertedScan" -> parseRowsetType relOpDetails |> PResult.map RelOpDetails.InsertedScan
    | "Insert" -> parseDMLOpType relOpDetails |> PResult.map RelOpDetails.Insert
    | "Join" -> parseJoinType relOpDetails |> PResult.map RelOpDetails.Join
    | "LocalCube" -> parseLocalCubeType relOpDetails |> PResult.map RelOpDetails.LocalCube
    | "LogRowScan" -> parseRelOpBase relOpDetails |>  PResult.bind (function r, [] -> RelOpDetails.LogRowScan r |> POk | _ , rest -> Failf "LogRowScan should not have child elements, but had %d" (List.length rest))
    | "Merge" -> parseMergeType relOpDetails |> PResult.map RelOpDetails.Merge
    | "MergeInterval" -> parseSimpleIteratorOneChildType relOpDetails |> PResult.map RelOpDetails.MergeInterval
    | "Move" -> parseMoveType relOpDetails |> PResult.map RelOpDetails.Move
    | "NestedLoops" -> parseNestedLoopsType relOpDetails |> PResult.map RelOpDetails.NestedLoops
    | "OnlineIndex" -> parseCreateIndexType relOpDetails |> PResult.map RelOpDetails.OnlineIndex
    | "Parallelism" -> parseParallelismType relOpDetails |> PResult.map RelOpDetails.Parallelism
    | "ParameterTableScan" -> parseRelOpBase relOpDetails |>  PResult.bind (function r, [] -> RelOpDetails.ParameterTableScan r |> POk | _ , rest -> Failf "ParameterTableScan should not have child elements, but had %d" (List.length rest)) 
    | "PrintDataflow" -> parseRelOpBase relOpDetails |>  PResult.bind (function r, [] -> RelOpDetails.PrintDataflow r |> POk | _ , rest -> Failf "PrintDataflow should not have child elements, but had %d" (List.length rest))
    | "Project" -> parseProjectType relOpDetails |> PResult.map RelOpDetails.Project
    | "Put" -> parsePutType relOpDetails |> PResult.map RelOpDetails.Put
    | "RemoteFetch" -> parseRemoteFetchType relOpDetails |> PResult.map RelOpDetails.RemoteFetch
    | "RemoteModify" -> parseRemoteModifyType relOpDetails |> PResult.map RelOpDetails.RemoteModify
    | "RemoteQuery" -> parseRemoteQueryType relOpDetails |> PResult.map RelOpDetails.RemoteQuery
    | "RemoteRange" -> parseRemoteRangeType relOpDetails |> PResult.map RelOpDetails.RemoteRange
    | "RemoteScan" -> parseRemoteType relOpDetails |> PResult.map RelOpDetails.RemoteScan
    | "RowCountSpool" -> parseSpoolType relOpDetails |> PResult.map RelOpDetails.RowCountSpool
    | "ScalarInsert" -> parseScalarInsertType relOpDetails |> PResult.map RelOpDetails.ScalarInsert
    | "Segment" -> parseSegmentType relOpDetails |> PResult.map RelOpDetails.Segment
    | "Sequence" -> parseSequenceType relOpDetails |> PResult.map RelOpDetails.Sequence
    | "SequenceProject" -> parseComputeScalarType relOpDetails |> PResult.map RelOpDetails.SequenceProject
    | "SimpleUpdate" -> parseSimpleUpdateType relOpDetails |> PResult.map RelOpDetails.SimpleUpdate
    | "Sort" -> parseSortType relOpDetails |> PResult.map RelOpDetails.Sort
    | "Split" -> parseSplitType relOpDetails |> PResult.map RelOpDetails.Split
    | "Spool" -> parseSpoolType relOpDetails |> PResult.map RelOpDetails.Spool
    | "StreamAggregate" -> parseStreamAggregateType relOpDetails |> PResult.map RelOpDetails.StreamAggregate
    | "Switch" -> parseSwitchType relOpDetails |> PResult.map RelOpDetails.Switch
    | "TableScan" -> parseTableScanType relOpDetails |> PResult.map RelOpDetails.TableScan
    | "TableValuedFunction" -> parseTableValuedFunctionType relOpDetails |> PResult.map RelOpDetails.TableValuedFunction
    | "Top" -> parseTopType relOpDetails |> PResult.map RelOpDetails.Top
    | "TopSort" -> parseTopSortType relOpDetails |> PResult.map RelOpDetails.TopSort
    | "Update" -> parseUpdateType relOpDetails |> PResult.map RelOpDetails.Update
    | "Union" -> parseConcatType relOpDetails |> PResult.map RelOpDetails.Union
    | "UnionAll" -> parseConcatType relOpDetails |> PResult.map RelOpDetails.UnionAll
    | "WindowSpool" -> parseWindowType relOpDetails |> PResult.map RelOpDetails.WindowSpool
    | "WindowAggregate" -> parseWindowAggregateType relOpDetails |> PResult.map RelOpDetails.WindowAggregate
    | "XcsScan" -> parseXcsScanType relOpDetails |> PResult.map RelOpDetails.XcsScan
    | name -> Failf "Unknown RelOpDetails type: '%s'" name

and parseRelOpType (relOp : Linq.XElement) : PResult<RelOpType, _> =
    PResult.builder {
        let! (columnsReferences, rest) = xElementReq (ensureName ("OutputList", ns) parseColumnReferenceListType) (xElementsAll relOp)
        let! (warnings, rest) = xElement (ensureName ("Warnings", ns) parseWarningsType) rest
        let! (memoryFractions, rest) = xElement (ensureName ("MemoryFractions", ns) parseMemoryFractionsType) rest
        let! (runTimeInformation, rest) = xElement (ensureName ("RunTimeInformation", ns) parseRunTimeInformation) rest
        let! (runTimePartitionSummary, rest) = xElement (ensureName ("RunTimePartitionSummary", ns) parseRunTimePartitionSummary) rest
        let! (internalInfoType, rest) = xElement (ensureName ("InternalInfo", ns) parseInternalInfoType) rest
        let! (operatorDetails, rest) = xElement parseRelOpDetails rest
        do! xElementEnsureEmpty rest
        
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
            OutputList = columnsReferences
            Warnings = warnings
            MemoryFractions = memoryFractions
            RunTimeInformation = runTimeInformation
            RunTimePartitionSummary = runTimePartitionSummary
            InternalInfo = internalInfoType
            OperatorDetails = operatorDetails

            
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

let parseOptimizationReplay (optimizationReplay : Linq.XElement) : PResult<OptimizationReplayType, _> =
    PResult.builder {
        let! script = xAttrReq "Script" optimizationReplay
        return { Script = script }
    }

let parseThreadReservations (threadReservation : Linq.XElement) : PResult<ThreadReservationType, _> =
    PResult.builder {
        let! nodeId = xAttr "NodeId" threadReservation
        let! reservedThreads = xAttrReq "ReservedThreads" threadReservation
        return { 
            NodeId = nodeId
            ReservedThreads = reservedThreads
        }
    }

let parseThreadStatType (threadStat : Linq.XElement) : PResult<ThreadStatType, _> =
    PResult.builder {
        let! branches = xAttrReq "ThreadId" threadStat
        let! usedThreads = xAttr "UsedThreads" threadStat
        let! (threadReservations, rest) = xElementMany (ensureName ("ThreadReservations", ns) parseThreadReservations) (xElementsAll threadStat)
        do! xElementEnsureEmpty rest
        return { 
            Branches = branches
            UsedThreads = usedThreads
            ThreadReservations = threadReservations
        }
    }

let parseMissingColumnType (missingColumn : Linq.XElement) : PResult<MissingColumnType, _> =
    PResult.builder {
        let! name = xAttrReq "Name" missingColumn
        let! columnId = xAttrReq "ColumnId" missingColumn
        return { Name = name; ColumnId = columnId }
    }
let parseColumnGroup (columnGroup : Linq.XElement) : PResult<ColumnGroupType, _> =
    PResult.builder {
        let! usage = xAttrReq "Usage" columnGroup
        let! (columns, rest) = xElementMany (ensureName ("Column", ns) parseMissingColumnType) (xElementsAll columnGroup)
        do! xElementEnsureEmpty rest
        return { 
            Usage = usage
            Columns = columns
        }
    }
let parseMissingIndexType (missingIndex : Linq.XElement) : PResult<MissingIndexType, _> =
    PResult.builder {
        let! database = xAttrReq "Database" missingIndex
        let! schema = xAttrReq "Schema" missingIndex
        let! table = xAttrReq "Table" missingIndex
        let! (columnGroups, rest) = xElementMany (ensureName ("ColumnGroup", ns) parseColumnGroup) (xElementsAll missingIndex)
        do! xElementEnsureEmpty rest
        return { 
            Database = database
            Schema = schema
            Table = table
            ColumnGroups = columnGroups
        }
    }

let parseMissingIndexGroup (missingIndexGroup : Linq.XElement) : PResult<MissingIndexGroupType, _> =
    PResult.builder {
        let! impact = xAttrReq "Impact" missingIndexGroup
        let! (missingIndexes, rest) = xElementMany (ensureName ("MissingIndex", ns) parseMissingIndexType) (xElementsAll missingIndexGroup)
        do! xElementEnsureEmpty rest
        return { 
            Impact = impact
            MissingIndexes = missingIndexes
        }
    }

let parseMissingIndexesType (missingIndexes : Linq.XElement) : PResult<MissingIndexesType, _> =
    PResult.builder {
        let! (missingIndexGroups, rest) = xElementMany (ensureName ("MissingIndexGroup", ns) parseMissingIndexGroup) (xElementsAll missingIndexes)
        do! xElementEnsureEmpty rest
        return { MissingIndexGroups = missingIndexGroups }
    }

let parseGuessedSelectivityType (guessedSelectivity : Linq.XElement) : PResult<GuessedSelectivityType, _> =
    PResult.builder {
        let! (spatial, rest) = xElementReq (ensureName ("Spatial", ns) parseObjectType) (xElementsAll guessedSelectivity)
        do! xElementEnsureEmpty rest
        return {  Spatial = spatial }
    }

let parseParameterizationType (parameterization : Linq.XElement) : PResult<ParameterizationType, _> =
    PResult.builder {
        let! (objects, rest) = xElementMany (ensureName ("Object", ns) parseObjectType) (xElementsAll parameterization)
        do! xElementEnsureEmpty rest
        return { Objects = objects }
    }
let parseUnmatchedIndexesType (unmatchedIndexes : Linq.XElement) : PResult<UnmatchedIndexesType, _> =
    PResult.builder {
        let! (parameterization, rest) = xElementReq (ensureName ("Parameterization", ns) parseParameterizationType) (xElementsAll unmatchedIndexes)
        do! xElementEnsureEmpty rest
        return { Parameterization = parameterization }
    }

let parseTraceFlag (traceFlag : Linq.XElement) : PResult<TraceFlag, _> =
    PResult.builder {
        let! value = xAttrReq "Value" traceFlag
        let! scope = xAttrReq "Scope" traceFlag
        return { Value = value; Scope = scope }
    }
let parseTraceFlagsType (traceFlags : Linq.XElement) : PResult<TraceFlagListType, _> =
    PResult.builder {
        let! isCompileTime = xAttrReq "IsCompileTime" traceFlags
        let! (traceFlags, rest) = xElementMany (ensureName ("TraceFlag", ns) parseTraceFlag) (xElementsAll traceFlags)
        do! xElementEnsureEmpty rest
        return { IsCompileTime = isCompileTime; TraceFlags = traceFlags }
    }

let parseWaitStatType (waitStat : Linq.XElement) : PResult<WaitStatType, _> =
    PResult.builder {
        let! waitType = xAttrReq "WaitType" waitStat
        let! waitTimeMs = xAttrReq "WaitTimeMs" waitStat
        let! waitCount = xAttrReq "WaitCount" waitStat
        return { WaitType = waitType; WaitTimeMs = waitTimeMs; WaitCount = waitCount }
    }
let parseWaitStatListType (waitStats : Linq.XElement) : PResult<WaitStatListType, _> =
    PResult.builder {
        let! (waitStats, rest) = xElementMany (ensureName ("Wait", ns) parseWaitStatType) (xElementsAll waitStats)
        do! xElementEnsureEmpty rest
        return { WaitStats = waitStats }
    }

let parseQueryTimeStatsType (queryTimeStats : Linq.XElement) : PResult<QueryExecTimeType, _> =
    PResult.builder {
        let! cpuTime = xAttrReq "QueryExecutionTimeMs" queryTimeStats
        let! elapsedTime = xAttrReq "QueryCompilationTimeMs" queryTimeStats
        let! udfCpuTime = xAttr "UdfCpuTime" queryTimeStats
        let! udfElapsedTime = xAttr "UdfElapsedTime" queryTimeStats
        return { CpuTime = cpuTime; ElapsedTime = elapsedTime; UdfCpuTime = udfCpuTime; UdfElapsedTime = udfElapsedTime }
    }

let parseQueryPlanType (queryPlan : Linq.XElement) : PResult<QueryPlanType, _> =
    PResult.builder {
        let! degreeOfParallelism = xAttr "DegreeOfParallelism" queryPlan
        let! effectiveDegreeOfParallelism = xAttr "EffectiveDegreeOfParallelism" queryPlan
        let! nonParallelPlanReason = xAttr "NonParallelPlanReason" queryPlan
        let! dopFeedbackAdjusted = xAttr "DOPFeedbackAdjusted" queryPlan
        let! memoryGrant = xAttr "MemoryGrant" queryPlan
        let! cachedPlanSize = xAttr "CachedPlanSize" queryPlan
        let! compileTime = xAttr "CompileTime" queryPlan
        let! compileCPU = xAttr "CompileCPU" queryPlan
        let! compileMemory = xAttr "CompileMemory" queryPlan
        let! usePlan = xAttr "UsePlan" queryPlan
        let! containsInterleavedExecutionCandidates = xAttr "ContainsInterleavedExecutionCandidates" queryPlan
        let! containsInlineScalarTsqlUdfs = xAttr "ContainsInlineScalarTsqlUdfs" queryPlan
        let! queryVariantID = xAttr "QueryVariantID" queryPlan
        let! dispatcherPlanHandle = xAttr "DispatcherPlanHandle" queryPlan
        let! exclusiveProfileTimeActive = xAttr "ExclusiveProfileTimeActive" queryPlan

        let! (internalInfo, rest) = xElement (ensureName ("InternalInfo", ns) parseInternalInfoType) (xElementsAll queryPlan)
        let! (optimizationReplay, rest) = xElement (ensureName ("OptimizationReplay", ns) parseOptimizationReplay) rest
        let! (threadStat, rest) = xElement (ensureName ("ThreadStat", ns) parseThreadStatType) rest
        let! (missingIndexes, rest) = xElement (ensureName ("MissingIndexes", ns) parseMissingIndexesType) rest
        let! (guessedSelectivity, rest) = xElement (ensureName ("GuessedSelectivity", ns) parseGuessedSelectivityType) rest
        let! (unmatchedIndexes, rest) = xElement (ensureName ("UnmatchedIndexes", ns) parseUnmatchedIndexesType) rest
        let! (warnings, rest) = xElement (ensureName ("Warnings", ns) parseWarningsType) rest
        let! (memoryGrantInfo, rest) = xElement (ensureName ("MemoryGrantInfo", ns) parseMemoryGrantType) rest
        let! (optimizerHardwareDependentProperties, rest) = 
            xElement (ensureName ("OptimizerHardwareDependentProperties", ns) parseOptimizerHardwareDependentProperties) rest
        let! (optimizerStatsUsage, rest) = xElement (ensureName ("OptimizerStatsUsage", ns) parseOptimizerStatsUsage) rest
        let! (traceFlags, rest) = xElementMany (ensureName ("TraceFlags", ns) parseTraceFlagsType) rest
        let! (waitStats, rest) = xElement (ensureName ("WaitStats", ns) parseWaitStatListType) rest
        let! (queryTimeStats, rest) = xElement (ensureName ("QueryTimeStats", ns) parseQueryTimeStatsType) rest
        let! (relOp, rest) = xElementReq (ensureName ("RelOp", ns) parseRelOpType) rest
        let! (parameterList, rest) = xElement (ensureName ("ParameterList", ns) parseColumnReferenceListType) rest
        do! xElementEnsureEmpty rest
        return {
            DegreeOfParallelism = degreeOfParallelism   
            EffectiveDegreeOfParallelism = effectiveDegreeOfParallelism
            NonParallelPlanReason = nonParallelPlanReason
            DOPFeedbackAdjusted = dopFeedbackAdjusted
            MemoryGrant = memoryGrant
            CachedPlanSize = cachedPlanSize
            CompileTime = compileTime
            CompileCPU = compileCPU
            CompileMemory = compileMemory
            UsePlan = usePlan
            ContainsInterleavedExecutionCandidates = containsInterleavedExecutionCandidates
            ContainsInlineScalarTsqlUdfs = containsInlineScalarTsqlUdfs
            QueryVariantID = queryVariantID
            DispatcherPlanHandle = dispatcherPlanHandle
            ExclusiveProfileTimeActive = exclusiveProfileTimeActive

            InternalInfo = internalInfo
            OptimizationReplay = optimizationReplay
            ThreadStat = threadStat
            MissingIndexes = missingIndexes
            GuessedSelectivity = guessedSelectivity
            UnmatchedIndexes = unmatchedIndexes
            Warnings = warnings
            MemoryGrantInfo = memoryGrantInfo
            OptimizerHardwareDependentProperties = optimizerHardwareDependentProperties
            OptimizerStatsUsage = optimizerStatsUsage
            TraceFlags = traceFlags
            WaitStats = waitStats
            QueryTimeStats = queryTimeStats
            RelOp = relOp
            ParameterList = parameterList
        }
    }

let parseReceivePlanType (receiveOperation : Linq.XElement) : PResult<ReceivePlanDetailType, _> =
    PResult.builder {
        let! operationType' = xAttrReq "OperationType" receiveOperation
        let! operationType =    
            match operationType' with
            | "ReceivePlanSelect" -> ReceivePlanOperationType.ReceivePlanSelect |> POk
            | "ReceivePlanUpdate" -> ReceivePlanOperationType.ReceivePlanUpdate |> POk
            | opType -> Failf "Unknown ReceivePlan OperationType: '%s'" opType
        let xmlElements = xElementsAll receiveOperation
        let! (queryPlan,rest) = xElementReq (ensureName ("QueryPlan", ns) parseQueryPlanType) xmlElements
        do! xElementEnsureEmpty rest
        return {
            OperationType = operationType
            QueryPlan = queryPlan
        }
    }

let parseParameterSensitivePredicateType (parameterSensitivePredicate : Linq.XElement) : PResult<ParameterSensitivePredicateType, _> =
    PResult.builder {
        let! lowBoundery = xAttrReq "LowBoundery" parameterSensitivePredicate
        let! highBoundery = xAttrReq "HighBoundery" parameterSensitivePredicate
        let! (statisticsInfo, rest) = xElementMany1 (ensureName ("StatisticsInfo", ns) parseStatsInfo) (xElementsAll parameterSensitivePredicate)
        let! (predicate, rest) = xElementReq (ensureName ("Predicate", ns) parseScalarExpressionType) rest
        do! xElementEnsureEmpty rest
        return { 
            StatisticsInfo = statisticsInfo
            Predicate = predicate
            LowBoundery = lowBoundery
            HighBoundary = highBoundery
        }
    }
let parseDispatcherType (dispatcher : Linq.XElement) : PResult<DispatcherType, _> =
    PResult.builder {
        let! (parameterSensitivePredicats, rest) = 
            xElementMany (ensureName ("ParameterSensitivePredicate", ns) parseParameterSensitivePredicateType) (xElementsAll dispatcher)
        do! xElementEnsureEmpty rest
        return { 
            ParameterSensitivePredicate = parameterSensitivePredicats
        }
    }


let parseStmtCursor (stmtCursor : Linq.XElement) : PResult<StmtCursorType, _> =
    PResult.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtCursor
        return failwith "NYI: StmtCursor parsing not yet implemented"
    }

let parseStmtReceive (stmtReceive : Linq.XElement) : PResult<StmtReceiveType, _> =
    PResult.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtReceive
        let! (receivePlans,rest) = xElementMany (ensureName ("ReceivePlan", ns) parseReceivePlanType) rest
        do! xElementEnsureEmpty rest
        return { 
            BaseInfo = baseInfo
            ReceivePlans = receivePlans
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

let rec parseStmtSimple (stmtSimple : Linq.XElement) : PResult<StmtSimpleType, _> =
    PResult.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtSimple
        let! (dispatcher, rest) = xElement (ensureName ("Dispatcher", ns) parseDispatcherType) rest 
        let! (queryPlan, rest) = xElement (ensureName ("QueryPlan", ns) parseQueryPlanType) rest 
        let! (udfs, rest) = xElementMany (ensureName ("UDF", ns) parseFunctionType) rest 
        let! (storedProc, rest) = xElement (ensureName ("StoredProc", ns) parseFunctionType) rest 
        do! xElementEnsureEmpty rest
        return { 
            BaseInfo = baseInfo; 
            Dispatcher = dispatcher
            QueryPlan = queryPlan 
            UDFs = udfs
            StoredProc = storedProc
        }
    }

and parseFunctionType (functionType : Linq.XElement) : PResult<FunctionType, _> =
    PResult.builder {
        let xmlElements = xElementsAll functionType
        let! (statements, rest) = xElementMany (ensureName ("Statements", ns) parseStmtBlockType) xmlElements
        do! xElementEnsureEmpty rest

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
            Else = elseStmt 
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
