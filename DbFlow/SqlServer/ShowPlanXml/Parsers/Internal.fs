module DbFlow.SqlServer.Experimental.ShowPlanXml.Parsers.Internal

open System.Xml

open DbFlow.XmlParser
open DbFlow.SqlServer.Experimental.ShowPlanXml
open DbFlow.SqlServer.Experimental.ShowPlanXml.Parsers.Primitives

let ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan"

// ========================================
// Known but undocumented additions to what is specified in the latest xsd:
//
// - ... -> QueryPlan -> CardinalityFeedback
// 
// ========================================

let parseUndocumentedElement (e : Linq.XElement) : Result<unit, _> =
    // Some elements in the actual XML plans are not documented - in those cases the following "parser" is used to simply ignore the elements 
    Ok ()



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
let parseObjectType (o : Linq.XElement) : Result<ObjectType, _> =
    Result.builder {
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

let parseInternalInfoType (internalInfo : Linq.XElement) : Result<InternalInfoType, _> =
    Result.builder {
        let content = internalInfo.ToString() |> Some
        return { Content = content }
    }

let parseConstType (const_ : Linq.XElement) : Result<ConstType, _> =
    Result.builder {
        let! constValue = xAttrReq "ConstValue" const_
        return { ConstValue = constValue }
    }

let parseScalarSequenceType (sequence : Linq.XElement) : Result<ScalarSequenceType, _> =
    Result.builder {
        let! functionName = xAttrReq "FunctionName" sequence
        return { FunctionName = functionName }
    }

let parseCLRFunctionType (clrFunction : Linq.XElement) : Result<CLRFunctionType, _> =
    Result.builder {
        let! assembly = xAttr "Assembly" clrFunction
        let! class_ = xAttrReq "Class" clrFunction
        let! method = xAttr "Method" clrFunction
        return { 
            Assembly = assembly
            Class = class_
            Method = method 
        }
    }

let parseSpillOccurredWarning (element : Linq.XElement) : Result<SpillOccurredType, _> =
    Result.builder {
        let! detail = xAttr "Detail" element
        return { Detail = detail }
    }

let parseSpillToTempDb (element : Linq.XElement) : Result<SpillToTempDbType, _> =
    Result.builder {
        let! spillReason = xAttr "SpillReason" element
        let! spilledThreadCount = xAttr "SpilledThreadCount" element
        return { SpillLevel = spillReason; SpilledThreadCount = spilledThreadCount }
    }

let parseWaitWarning (element : Linq.XElement) : Result<WaitWarningType, _> =
    Result.builder {
        let! waitType = xAttrReq "WaitType" element
        let! waitTime = xAttr "WaitTime" element
        return { 
            WaitType = waitType
            WaitTime = waitTime
        }
    }
    
let parseAffectingConvertWarning (element : Linq.XElement) : Result<AffectingConvertWarningType, _> =
    Result.builder {
        let! convertIssue = xAttrReq "ConvertIssue" element
        let! expression = xAttrReq "Expression" element
        return { 
            ConvertIssue = convertIssue
            Expression = expression
        }
    }

let parseSortSpillDetailsWarning (element : Linq.XElement) : Result<SortSpillDetailsType, _> =
    Result.builder {
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

let parseHashSpillDetailsWarning (element : Linq.XElement) : Result<HashSpillDetailsType, _> =
    Result.builder {
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

let parseExchangeSpillDetailsWarning (element : Linq.XElement) : Result<ExchangeSpillDetailsType, _> =
    Result.builder {
        let! writesToTempDb = xAttr "WritesToTempDb" element
        return { WritesToTempDb = writesToTempDb }
    }

let parseMemoryGrantWarning (element : Linq.XElement) : Result<MemoryGrantWarningInfo, _> =
    Result.builder {
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

let parseMemoryFractionsType (memoryFractions : Linq.XElement) : Result<MemoryFractionsType, _> =
    Result.builder {
        let! input = xAttrReq "Input" memoryFractions
        let! output = xAttrReq "Output" memoryFractions
        return { Input = input; Output = output }
    }

let parseMemoryGrantType (memoryGrantInfo : Linq.XElement) : Result<MemoryGrantType, _> =
    Result.builder {
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

let parseOptimizerHardwareDependentProperties (props : Linq.XElement) : Result<OptimizerHardwareDependentPropertiesType, _> =
    Result.builder {
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

let parseStatsInfo (statsInfo : Linq.XElement) : Result<StatsInfoType, _> =
    Result.builder {
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

let parseOptimizerStatsUsage (optimizerStatsUsage : Linq.XElement) : Result<OptimizerStatsUsageType, _> =
    Result.builder {
        let! (statisticsInfo, rest) = xElementMany (nameGuard ("StatisticsInfo", ns) parseStatsInfo) (xElementsAll optimizerStatsUsage)
        do! ensureEmpty rest

        return { StatisticsInfo = statisticsInfo }
    }

let parseBaseStmtInfoType (stmtSimple : Linq.XElement) : Result<BaseStmtInfoType * _, _> =
    Result.builder {
        let! (statementSetOptions, rest) = xElement (nameGuard ("StatementSetOptions", ns) parseSetOptionsType) (xElementsAll stmtSimple)
        
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

let parseRunTimeCountersPerThread (runTimeCountersPerThread : Linq.XElement) : Result<RunTimeCountersPerThread, _> = 
    Result.builder {
        let! thread = xAttrReq "Thread" runTimeCountersPerThread
        let! brickId = xAttr "BrickId" runTimeCountersPerThread
        let! actualRebinds = xAttr "ActualRebinds" runTimeCountersPerThread
        let! actualRewinds = xAttr "ActualRewinds" runTimeCountersPerThread
        let! actualRows = xAttrReq "ActualRows" runTimeCountersPerThread
        let! rowRequalifications = xAttr "RowRequalifications" runTimeCountersPerThread
        let! actualRowsRead = xAttr "ActualRowsRead" runTimeCountersPerThread
        let! batches = xAttr "Batches" runTimeCountersPerThread
        let! actualEndOfScans = xAttrReq "ActualEndOfScans" runTimeCountersPerThread
        let! actualExecutions = xAttrReq "ActualExecutions" runTimeCountersPerThread
        let! actualExecutionMode = xAttrP "ActualExecutionMode" parseExecutionMode runTimeCountersPerThread
        let! taskAddr = xAttr "TaskAddr" runTimeCountersPerThread
        let! schedulerId = xAttr "SchedulerId" runTimeCountersPerThread
        let! firstActiveTime = xAttr "FirstActiveTime" runTimeCountersPerThread
        let! lastActiveTime = xAttr "LastActiveTime" runTimeCountersPerThread
        let! openTime = xAttr "OpenTime" runTimeCountersPerThread
        let! firstRowTime = xAttr "FirstRowTime" runTimeCountersPerThread
        let! lastRowTime = xAttr "LastRowTime" runTimeCountersPerThread
        let! closeTime = xAttr "CloseTime" runTimeCountersPerThread
        let! actualElapsedms = xAttr "ActualElapsedms" runTimeCountersPerThread
        let! actualCPUms = xAttr "ActualCPUms" runTimeCountersPerThread
        let! actualScans = xAttr "ActualScans" runTimeCountersPerThread
        let! actualLogicalReads = xAttr "ActualLogicalReads" runTimeCountersPerThread
        let! actualPhysicalReads = xAttr "ActualPhysicalReads" runTimeCountersPerThread
        let! actualPageServerReads = xAttr "ActualPageServerReads" runTimeCountersPerThread
        let! actualReadAheads = xAttr "ActualReadAheads" runTimeCountersPerThread
        let! actualPageServerReadAheads = xAttr "ActualPageServerReadAheads" runTimeCountersPerThread
        let! actualLobLogicalReads = xAttr "ActualLobLogicalReads" runTimeCountersPerThread
        let! actualLobPhysicalReads = xAttr "ActualLobPhysicalReads" runTimeCountersPerThread
        let! actualLobPageServerReads = xAttr "ActualLobPageServerReads" runTimeCountersPerThread
        let! actualLobReadAheads = xAttr "ActualLobReadAheads" runTimeCountersPerThread
        let! actualLobPageServerReadAheads = xAttr "ActualLobPageServerReadAheads" runTimeCountersPerThread
        let! segmentReads = xAttr "SegmentReads" runTimeCountersPerThread
        let! segmentSkips = xAttr "SegmentSkips" runTimeCountersPerThread
        let! actualLocallyAggregatedRows = xAttr "ActualLocallyAggregatedRows" runTimeCountersPerThread
        let! inputMemoryGrant = xAttr "InputMemoryGrant" runTimeCountersPerThread
        let! outputMemoryGrant = xAttr "OutputMemoryGrant" runTimeCountersPerThread
        let! usedMemoryGrant = xAttr "UsedMemoryGrant" runTimeCountersPerThread
        let! isInterleavedExecuted = xAttr "IsInterleavedExecuted" runTimeCountersPerThread
        let! actualJoinType = xAttrP "ActualJoinType" parsePhysicalOp runTimeCountersPerThread
        let! hpcRowCount = xAttr "HpcRowCount" runTimeCountersPerThread
        let! hpcKernelElapsedUs = xAttr "HpcKernelElapsedUs" runTimeCountersPerThread
        let! hpcHostToDeviceBytes = xAttr "HpcHostToDeviceBytes" runTimeCountersPerThread
        let! hpcDeviceToHostBytes = xAttr "HpcDeviceToHostBytes" runTimeCountersPerThread
        let! actualPageServerPushedPageIDs = xAttr "ActualPageServerPushedPageIDs" runTimeCountersPerThread
        let! actualPageServerRowsReturned = xAttr "ActualPageServerRowsReturned" runTimeCountersPerThread
        let! actualPageServerRowsRead = xAttr "ActualPageServerRowsRead" runTimeCountersPerThread
        let! actualPageServerPushedReads = xAttr "ActualPageServerPushedReads" runTimeCountersPerThread

        return { 
            Thread = thread
            BrickId = brickId
            ActualRebinds = actualRebinds
            ActualRewinds = actualRewinds
            ActualRows = actualRows
            RowRequalifications = rowRequalifications
            ActualRowsRead = actualRowsRead
            Batches = batches
            ActualEndOfScans = actualEndOfScans
            ActualExecutions = actualExecutions
            ActualExecutionMode = actualExecutionMode
            TaskAddr = taskAddr
            SchedulerId = schedulerId
            FirstActiveTime = firstActiveTime
            LastActiveTime = lastActiveTime
            OpenTime = openTime
            FirstRowTime = firstRowTime
            LastRowTime = lastRowTime
            CloseTime = closeTime
            ActualElapsedms = actualElapsedms
            ActualCPUms = actualCPUms
            ActualScans = actualScans
            ActualLogicalReads = actualLogicalReads
            ActualPhysicalReads = actualPhysicalReads
            ActualPageServerReads = actualPageServerReads
            ActualReadAheads = actualReadAheads
            ActualPageServerReadAheads = actualPageServerReadAheads
            ActualLobLogicalReads = actualLobLogicalReads
            ActualLobPhysicalReads = actualLobPhysicalReads
            ActualLobPageServerReads = actualLobPageServerReads
            ActualLobReadAheads = actualLobReadAheads
            ActualLobPageServerReadAheads = actualLobPageServerReadAheads
            SegmentReads = segmentReads
            SegmentSkips = segmentSkips
            ActualLocallyAggregatedRows = actualLocallyAggregatedRows
            InputMemoryGrant = inputMemoryGrant
            OutputMemoryGrant = outputMemoryGrant
            UsedMemoryGrant = usedMemoryGrant
            IsInterleavedExecuted = isInterleavedExecuted
            ActualJoinType = actualJoinType
            HpcRowCount = hpcRowCount
            HpcKernelElapsedUs = hpcKernelElapsedUs
            HpcHostToDeviceBytes = hpcHostToDeviceBytes
            HpcDeviceToHostBytes = hpcDeviceToHostBytes
            ActualPageServerPushedPageIDs = actualPageServerPushedPageIDs
            ActualPageServerRowsReturned = actualPageServerRowsReturned
            ActualPageServerRowsRead = actualPageServerRowsRead
            ActualPageServerPushedReads = actualPageServerPushedReads
        }
    }

let parseRunTimeInformation (runTimeInformation : Linq.XElement) : Result<RunTimeInformationType, _> = 
    Result.builder {
        let! (runTimeCountersPerThread, rest) = 
            xElementMany (nameGuard ("RunTimeCountersPerThread", ns) parseRunTimeCountersPerThread) (xElementsAll runTimeInformation)
        do! ensureEmpty rest

        return { 
            RunTimeCountersPerThread = runTimeCountersPerThread
        }   
    }

let parsePartitionRangeType (partitionRange : Linq.XElement) : Result<PartitionRangeType, _> = 
    Result.builder {
        let! partitionStart = xAttrReq "Start" partitionRange
        let! partitionEnd = xAttrReq "End" partitionRange
        return { 
            Start = partitionStart
            End = partitionEnd
        }   
    }

let parsePartitionsAccessedType (partitionsAccessed : Linq.XElement) : Result<PartitionsAccessedType, _> = 
    Result.builder {
        let! (partitionRange, rest) = 
            xElementMany (nameGuard ("PartitionRange", ns) parsePartitionRangeType) (xElementsAll partitionsAccessed)
        do! ensureEmpty rest
        let! partitionCount = xAttrReq "PartitionCount" partitionsAccessed
        return { 
            PartitionRange = partitionRange
            PartitionCount = partitionCount
        }   
    }

let parseRunTimePartitionSummary (runTimePartitionSummary : Linq.XElement) : Result<RunTimePartitionSummaryType, _> = 
    Result.builder {
        let! (partitionsAccessed, rest) = 
            xElementReq (nameGuard ("PartitionsAccessed", ns) parsePartitionsAccessedType) (xElementsAll runTimePartitionSummary)
        do! ensureEmpty rest

        return { 
            PartitionsAccessed = partitionsAccessed
        }   
    }

// ========================================
// Single mutual recursion block covering:
//   ColumnReference ↔ ScalarType ↔ SubqueryType ↔ RelOpType
// ========================================



let rec parseColumnReferenceType (columnReference : Linq.XElement) : Result<ColumnReferenceType, _> =
    Result.builder {
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
        
        let! (scalarOperator, rest) = xElement (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll columnReference)
        let! (internalInfo, rest) = xElement (nameGuard ("InternalInfo", ns) parseInternalInfoType) rest
        do! ensureEmpty rest
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
        let! table = xAttr "Table" ident
        
        let! (columnReference, rest) = xElement (nameGuard ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll ident)
        do! ensureEmpty rest

        return { 
            ColumnReference = columnReference
            Table = table 
        }
    }

and parseCompareType (compare : Linq.XElement) : Result<CompareType, _> =
    Result.builder {
        let! compareOp = xAttrReqP "CompareOp" parseCompareOp compare 
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll compare)
        do! ensureEmpty rest
        return { 
            CompareOp = compareOp
            ScalarOperators = scalarOperators 
        }
    }

and parseConvertType (convert : Linq.XElement) : Result<ConvertType, _> =
    Result.builder {
        let! dataType = xAttrReq "DataType" convert
        let! length = xAttr "Length" convert
        let! precision = xAttr "Precision" convert
        let! scale = xAttr "Scale" convert
        let! style = xAttrReq "Style" convert
        let! implicit = xAttrReq "Implicit" convert
        
        let! (styleExpression, rest) = xElement (nameGuard ("Style", ns) parseScalarExpressionType) (xElementsAll convert)
        let! (scalarOperator, rest) = xElementReq (nameGuard ("ScalarOperator", ns) parseScalarType) rest
        do! ensureEmpty rest
        
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
        let! operation = xAttrReqP "Operation" parseArithmeticOperation arithmetic 
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll arithmetic)
        do! ensureEmpty rest
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseLogicalType (logical : Linq.XElement) : Result<LogicalType, _> =
    Result.builder {
        let! operation = xAttrReqP "Operation" parseLogicalOperation logical 
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll logical)
        do! ensureEmpty rest
        return { 
            Operation = operation
            ScalarOperators = scalarOperators 
        }
    }

and parseAggregateType (aggregate : Linq.XElement) : Result<AggregateType, _> =
    Result.builder {
        let! aggType = xAttrReq "AggType" aggregate
        let! distinct = xAttrReq "Distinct" aggregate
        
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll aggregate)
        do! ensureEmpty rest

        return { 
            AggType = aggType
            Distinct = distinct
            ScalarOperators = scalarOperators 
        }
    }

and parseUDAggregateType (udAggregate : Linq.XElement) : Result<UDAggregateType, _> =
    Result.builder {
        let! distinct = xAttrReq "Distinct" udAggregate
        
        let! (udAggObject, rest) = xElement (nameGuard ("UDAggObject", ns) parseObjectType) (xElementsAll udAggregate)
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) rest
        do! ensureEmpty rest

        return { 
            Distinct = distinct
            UDAggObject = udAggObject
            ScalarOperators = scalarOperators 
        }
    }

and parseIntrinsicType (intrinsic : Linq.XElement) : Result<IntrinsicType, _> =
    Result.builder {
        let! functionName = xAttrReq "FunctionName" intrinsic
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll intrinsic)
        do! ensureEmpty rest
        return { 
            FunctionName = functionName
            ScalarOperators = scalarOperators 
        }
    }

and parseUDFType (udf : Linq.XElement) : Result<UDFType, _> =
    Result.builder {
        let! functionName = xAttrReq "FunctionName" udf
        let! isClrFunction = xAttr "IsClrFunction" udf
        
        let! (clrFunction, rest) = xElement (nameGuard ("CLRFunction", ns) parseCLRFunctionType) (xElementsAll udf)
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) rest
        do! ensureEmpty rest

        return { 
            FunctionName = functionName
            IsClrFunction = isClrFunction
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseUDTMethodType (udtMethod : Linq.XElement) : Result<UDTMethodType, _> =
    Result.builder {
        let! (clrFunction, rest) = xElement (nameGuard ("CLRFunction", ns) parseCLRFunctionType) (xElementsAll udtMethod)
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) rest 
        do! ensureEmpty rest

        return { 
            CLRFunction = clrFunction
            ScalarOperators = scalarOperators 
        }
    }

and parseConditionalType (ifExpr : Linq.XElement) : Result<ConditionalType, _> =
    Result.builder {
        let! (condition, rest) = xElementReq (nameGuard ("Condition", ns) parseScalarExpressionType) (xElementsAll ifExpr)
        let! (then_, rest) = xElementReq (nameGuard ("Then", ns) parseScalarExpressionType) rest
        let! (else_, rest) = xElementReq (nameGuard ("Else", ns) parseScalarExpressionType) rest
        do! ensureEmpty rest
        return { 
            Condition = condition
            Then = then_
            Else = else_ 
        }
    }

and parseScalarExpressionType (scalarExpr : Linq.XElement) : Result<ScalarExpressionType, _> =
    Result.builder {
        let! (scalarOperator, rest) = xElementReq (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll scalarExpr)
        do! ensureEmpty rest

        return { ScalarOperator = scalarOperator }
    }

#nowarn 40
and parseAssignTargetType =
    createCaseParser
        [
             "ColumnReference", parseColumnReferenceType >> Result.map AssignTargetType.ColumnRef
             "ScalarOperator", parseScalarType >> Result.map AssignTargetType.ScalarOp
        ]

and parseAssignType (assign : Linq.XElement) : Result<AssignType, _> =
    Result.builder {
        let! (target, rest) = xElementReq parseAssignTargetType (xElementsAll assign)
        let! (scalarOperator, rest) = xElementReq (nameGuard ("ScalarOperator", ns) parseScalarType) rest
        let! (sourceColumns, rest) = xElementMany (nameGuard ("SourceColumn", ns) parseColumnReferenceType) rest
        let! (targetColumns, rest) = xElementMany (nameGuard ("TargetColumn", ns) parseColumnReferenceType) rest
        do! ensureEmpty rest
        return { 
            Target = target
            ScalarOperator = scalarOperator
            SourceColumns = sourceColumns
            TargetColumns = targetColumns 
        }
    }

and parseMultAssignType (multiAssign : Linq.XElement) : Result<MultAssignType, _> =
    Result.builder {
        let! (assigns, rest) = xElementMany (nameGuard ("Assign", ns) parseAssignType) (xElementsAll multiAssign)
        do! ensureEmpty rest

        return { Assigns = assigns }
    }

and parseScalarExpressionListType (scalarExprList : Linq.XElement) : Result<ScalarExpressionListType, _> =
    Result.builder {
        let! (scalarOperators, rest) = xElementMany (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll scalarExprList)
        do! ensureEmpty rest
        
        return { ScalarOperators = scalarOperators }
    }

and parseSubqueryType (subquery : Linq.XElement) : Result<SubqueryType, _> =
    Result.builder {
        let! operation = xAttrReqP "Operation" parseSubqueryOperation subquery 
        
        let! (scalarOperator, rest) = xElement (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll subquery)
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest

        return { 
            Operation = operation
            ScalarOperator = scalarOperator
            RelOp = relOp 
        }
    }

and parseScalarTypeKind =
    createCaseParser
        [
            "Aggregate", parseAggregateType >> Result.map ScalarOperatorKind.Aggregate
            "Arithmetic", parseArithmeticType >> Result.map ScalarOperatorKind.Arithmetic
            "Assign", parseAssignType >> Result.map ScalarOperatorKind.Assign
            "Compare", parseCompareType >> Result.map ScalarOperatorKind.Compare
            "Const", parseConstType >> Result.map ScalarOperatorKind.Const
            "Convert", parseConvertType >> Result.map ScalarOperatorKind.Convert
            "Identifier", parseIdentType >> Result.map ScalarOperatorKind.Identifier
            "IF", parseConditionalType >> Result.map ScalarOperatorKind.IF
            "Intrinsic", parseIntrinsicType >> Result.map ScalarOperatorKind.Intrinsic
            "Logical", parseLogicalType >> Result.map ScalarOperatorKind.Logical
            "MultipleAssign", parseMultAssignType >> Result.map ScalarOperatorKind.MultipleAssign
            "ScalarExpressionList", parseScalarExpressionListType >> Result.map ScalarOperatorKind.ScalarExpressionList
            "Sequence", parseScalarSequenceType >> Result.map ScalarOperatorKind.Sequence
            "Subquery", parseSubqueryType >> Result.map ScalarOperatorKind.Subquery
            "UDTMethod", parseUDTMethodType >> Result.map ScalarOperatorKind.UDTMethod
            "UserDefinedAggregate", parseUDAggregateType >> Result.map ScalarOperatorKind.UserDefinedAggregate
            "UserDefinedFunction", parseUDFType >> Result.map ScalarOperatorKind.UserDefinedFunction
        ]

and parseScalarType (scalarOperator : Linq.XElement) : Result<ScalarType, _> =
    Result.builder {
        let! scalarString = xAttr "ScalarString" scalarOperator
        
        let! (kind, rest) = xElementReq parseScalarTypeKind (xElementsAll scalarOperator)
        let! (internalInfo, rest) = xElement (nameGuard ("InternalInfo", ns) parseInternalInfoType) rest
        do! ensureEmpty rest

        return { 
            ScalarString = scalarString
            Kind = kind
            InternalInfo = internalInfo 
        }
    }

and parseColumnReferenceListType (columnReferenceList : Linq.XElement) : Result<ColumnReferenceType list, _> =
    Result.builder {
        let! (columnReferences, rest) = xElementMany (nameGuard ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll columnReferenceList)
        do! ensureEmpty rest
        return columnReferences
    }

and parseSingleColumnReferenceType (singleColumnReference : Linq.XElement) : Result<ColumnReferenceType, _> =
    Result.builder {
        let! (columnReference, rest) = xElementReq (nameGuard ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll singleColumnReference)
        do! ensureEmpty rest
        return columnReference
    }

and parseWarning =
    createCaseParser
        [
            "SpillOccurred",  parseSpillOccurredWarning >> Result.map Warning.SpillOccurred
            "ColumnsWithNoStatistics", parseColumnReferenceListType >> Result.map Warning.ColumnsWithNoStatistics
            "ColumnsWithStaleStatistics", parseColumnReferenceListType >> Result.map Warning.ColumnsWithStaleStatistics
            "SpillToTempDb", parseSpillToTempDb >> Result.map Warning.SpillToTempDb
            "Wait", parseWaitWarning >> Result.map Warning.Wait
            "PlanAffectingConvert", parseAffectingConvertWarning >> Result.map Warning.PlanAffectingConvert
            "SortSpillDetails", parseSortSpillDetailsWarning >> Result.map Warning.SortSpillDetails
            "HashSpillDetails", parseHashSpillDetailsWarning >> Result.map Warning.HashSpillDetails
            "ExchangeSpillDetails", parseExchangeSpillDetailsWarning >> Result.map Warning.ExchangeSpillDetails 
            "MemoryGrantWarning", parseMemoryGrantWarning >> Result.map Warning.MemoryGrantWarning
        ]

and parseWarningsType (warningsType : Linq.XElement) : Result<WarningsType, _> =
    Result.builder {
        let warningElements = xElementsAll warningsType
        let! (warnings, rest) = xElementMany parseWarning warningElements
        do! ensureEmpty rest
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

and parseDefinedValueType (definedValue : Linq.XElement) : Result<DefinedValueType, _> =
    
    let parseValueVector (valueVector : Linq.XElement) : Result<ColumnReferenceType list, _> =
        Result.builder {
            let! (valueVector, rest) = xElementMany (nameGuard ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll valueVector)
            do! ensureEmpty rest
            return valueVector
        }

    Result.builder {
        let allElements = xElementsAll definedValue
        // What is defined
        let! (valueVector, rest) = xElement (nameGuard ("ValueVector", ns) parseValueVector) allElements
        let! (defined, rest) = 
            match valueVector with
            | Some vv -> Ok (vv, rest)
            | None -> 
                xElementReq (nameGuard ("ColumnReference", ns) parseColumnReferenceType) rest 
                |> Result.map (fun (cr, rest) -> [cr], rest)
        // What is the value
        let! (cRefs, rest) = xElementMany (nameGuard ("ColumnReference", ns) parseColumnReferenceType) rest
        let! (value, rest) =
            match cRefs with
            | _ :: _ -> Ok (DefinedValueTypeValue.ColumnReferences cRefs |> Some, rest)
            | [] -> 
                xElement (nameGuard ("ScalarOperator", ns) parseScalarType) rest 
                |> Result.map (fun (sc, rest) -> sc |> Option.map DefinedValueTypeValue.ScalarOperator, rest)
                
        return { 
            Defined = defined
            Value = value
        }
    }
and parseDefinedValuesListType (definedValuesList : Linq.XElement) : Result<DefinedValuesListType, _> =
    Result.builder {
        let! (definedValues, rest) = xElementMany (nameGuard ("DefinedValue", ns) parseDefinedValueType) (xElementsAll definedValuesList)
        do! ensureEmpty rest
        return { DefinedValues = definedValues }
    }

and parseRelOpBase (relOp : Linq.XElement) : Result<RelOpBaseType*_, _> =
    Result.builder {
        let! (definedValues, rest) = xElement (nameGuard ("DefinedValues", ns) parseDefinedValuesListType) (xElementsAll relOp)
        let! (internalInfo, rest) = xElement (nameGuard ("InternalInfo", ns) parseInternalInfoType) rest
        return { 
            DefinedValues = definedValues
            InternalInfo = internalInfo
        }, rest
    }

and parseStarJoinInfoType (starJoinInfo : Linq.XElement) : Result<StarJoinInfoType, _> =
    Result.builder {
        let! root = xAttr "Root" starJoinInfo
        let! operationType = xAttrReq "OperationType" starJoinInfo
        return { 
            Root = root
            OperationType = operationType
        }
    }

and parseAdaptiveJoinType (adaptiveJoin : Linq.XElement) : Result<AdaptiveJoinType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase adaptiveJoin 
        let! (hashKeysBuild, rest) = xElement (nameGuard ("HashKeysBuild", ns) parseColumnReferenceListType) rest
        let! (hashKeysProbe, rest) = xElement (nameGuard ("HashKeysProbe", ns) parseColumnReferenceListType) rest
        let! (buildResidual, rest) = xElement (nameGuard ("BuildResidual", ns) parseScalarExpressionType) rest 
        let! (probeResidual, rest) = xElement (nameGuard ("ProbeResidual", ns) parseScalarExpressionType) rest 
        let! (starJoinInfo, rest) = xElement (nameGuard ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (predicate, rest) = xElement (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (passThru, rest) = xElement (nameGuard ("PassThru", ns) parseScalarExpressionType) rest
        let! (outerReferences, rest) = xElement (nameGuard ("OuterReference", ns) parseColumnReferenceListType) rest
        let! (partitionId, rest) = xElement (nameGuard ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest

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

and parseJoinType (join : Linq.XElement) : Result<JoinType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase join 
        let! (predicates, rest) = xElementMany (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (probes, rest) = xElementMany (nameGuard ("Probe", ns) parseSingleColumnReferenceType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        return { 
            Base = relOpBase
            Predicates = predicates
            Probes = probes
            RelOps = relOps
        }
    }

and parseFilterType (filter : Linq.XElement) : Result<FilterType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase filter 
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        let! (predicate, rest) = xElementReq (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        do! ensureEmpty rest
        
        let! startupExpression = xAttrReq "StartupExpression" filter
        return { 
            Base = relOpBase
            RelOp = relOp
            Predicate = predicate
            
            StartupExpression = startupExpression
        }
    }

and parseBatchHashTableBuildType (batchHashTableBuild : Linq.XElement) : Result<BatchHashTableBuildType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase batchHashTableBuild 
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! bitmapCreator = xAttr "BitmapCreator" batchHashTableBuild
        return { 
            Base = relOpBase
            RelOp = relOp
            BitmapCreator = bitmapCreator
        }
    }

and parseBitmapType (bitmap : Linq.XElement) : Result<BitmapType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase bitmap 
        let! (hashKeys, rest) = xElementReq (nameGuard ("HashKeys", ns) parseColumnReferenceListType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            HashKeys = hashKeys
            RelOp = relOp
        }
    }

and parseCollapseType (collapse : Linq.XElement) : Result<CollapseType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase collapse 
        let! (groupBy, rest) = xElementReq (nameGuard ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            GroupBy = groupBy
            RelOp = relOp
        }
    }

and parseComputeScalarType (computeScalar : Linq.XElement) : Result<ComputeScalarType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase computeScalar 
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! computeSequence = xAttr "ComputeSequence" computeScalar

        return { 
            Base = relOpBase
            RelOp = relOp
            ComputeSequence = computeSequence
        }
    }

and parseConcatType (concat : Linq.XElement) : Result<ConcatType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase concat 
        let! (relOps, rest) = xElementMany2 (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            RelOps = relOps
        }
    }

and parseConstantScanType (constantScan : Linq.XElement) : Result<ConstantScanType, _> =
    let parseValues (values : Linq.XElement) : Result<ScalarExpressionListType list, _> =
        Result.builder {
            let! (rows, rest) = xElementMany (nameGuard ("Row", ns) parseScalarExpressionListType) (xElementsAll values)
            do! ensureEmpty rest
            return rows
        }
    
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase constantScan 
        let! (values, rest) = xElement (nameGuard ("Values", ns) parseValues) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            Values = match values with Some v -> v | None -> []
        }
    }

and parseOutputColumns (outputColumns : Linq.XElement) : Result<OutputColumnsType, _> =
    Result.builder {
        let! (definedValues, rest) = xElement (nameGuard ("DefinedValues", ns) parseDefinedValuesListType) (xElementsAll outputColumns)
        let! (objects, rest) = xElementMany (nameGuard ("Objects", ns) parseObjectType) rest
        do! ensureEmpty rest
        return { DefinedValues = definedValues; Objects = objects }
    }

and parseGetType (get : Linq.XElement) : Result<GetType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase get 
        let! (bookmarks, rest) = xElement (nameGuard ("Bookmarks", ns) parseColumnReferenceListType) rest
        let! (outputColumns, rest) = xElement (nameGuard ("OutputColumns", ns) parseOutputColumns) rest
        let! (generatedData, rest) = xElement (nameGuard ("GeneratedData", ns) parseScalarExpressionListType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
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

and parseRowsetBase (rowset : Linq.XElement) : Result<RowsetType*_, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase rowset 
        let! (objects, rest) = xElementMany (nameGuard ("Object", ns) parseObjectType) rest
        return { 
            Base = relOpBase
            Objects = objects
        }, rest
    }

and parseCreateIndexType (createIndex : Linq.XElement) : Result<CreateIndexType, _> =
    Result.builder {
        let! (rowSet, rest) = parseRowsetBase createIndex 
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            RowsetBase = rowSet
            RelOp = relOp
        }
    }

and parseAssignmentMapType (assignmentMap : Linq.XElement) : Result<AssignmentMapType, _> =
    Result.builder {
        let! (assigns, rest) = xElementMany (nameGuard ("Assign", ns) parseAssignType) (xElementsAll assignmentMap)
        do! ensureEmpty rest
        return { Assigns = assigns }
    }

and parseParameterizationType (parameterization : Linq.XElement) : Result<ParameterizationType, _> =
    Result.builder {
        let! (objects, rest) = xElementMany (nameGuard ("Object", ns) parseObjectType) (xElementsAll parameterization)
        do! ensureEmpty rest
        return { Objects = objects }
    }

and parseDMLOpType (dmlOp : Linq.XElement) : Result<DMLOpType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase dmlOp 
        let! (assignmentMap, rest) = xElement (nameGuard ("AssignmentMap", ns) parseAssignmentMapType) rest
        let! (sourceTable, rest) = xElement (nameGuard ("SourceTable", ns) parseParameterizationType) rest
        let! (targetTable, rest) = xElement (nameGuard ("TargetTable", ns) parseParameterizationType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            AssignmentMap = assignmentMap 
            SourceTable = sourceTable
            TargetTable = targetTable
            RelOps = relOps
        }
    }

and parseRowsetType (rowset : Linq.XElement) : Result<RowsetType, _> =
    Result.builder {
        let! (relopBase, rest) = parseRelOpBase rowset 
        let! (objects, rest) = xElementMany (nameGuard ("Object", ns) parseObjectType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relopBase
            Objects = objects
        }
    }

and parseUDXType (udx : Linq.XElement) : Result<UDXType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase udx 
        let! (usedUDXColumns, rest) = xElement (nameGuard ("UsedUDXColumns", ns) parseColumnReferenceListType) rest
        let! (relOp, rest) = xElement (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! udxName = xAttrReq "UDXName" udx

        return { 
            Base = relOpBase
            UsedUDXColumns = usedUDXColumns
            RelOp = relOp
            UDXName = udxName
        }
    }

and parseExternalSelectType (externalSelect : Linq.XElement) : Result<ExternalSelectType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase externalSelect 
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
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

and parseRemoteTypeBase (remote : Linq.XElement) : Result<RemoteType*_, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase remote 
        
        let! remoteDestination = xAttr "RemoteDestination" remote
        let! remoteSource = xAttr "RemoteSource" remote
        let! remoteObject = xAttr "RemoteObject" remote
        return { 
            Base = relOpBase
            RemoteDestination = remoteDestination
            RemoteSource = remoteSource 
            RemoteObject = remoteObject 
        }, rest
    }

and parseRemoteType (remote : Linq.XElement) : Result<RemoteType, _> =
    Result.builder {
        let! (remoteTypeBase, rest) = parseRemoteTypeBase remote 
        do! ensureEmpty rest
        
        return remoteTypeBase
    }

and parseScanRangeType (scanRange : Linq.XElement) : Result<ScanRangeType, _> =
    Result.builder {
        let! (rangeColumns, rest) = xElementReq (nameGuard ("RangeColumns", ns) parseColumnReferenceListType) (xElementsAll scanRange)
        let! (rangeExpressions, rest) = xElementReq (nameGuard ("RangeExpressions", ns) parseScalarExpressionListType) rest
        do! ensureEmpty rest

        let! scanType = xAttrReqP "ScanType" parseCompareOp scanRange

        return { 
            RangeColumns = rangeColumns
            RangeExpressions = rangeExpressions
            ScanType = scanType
        }
    }

and parseSeekPredicate (seekPredicate : Linq.XElement) : Result<SeekPredicateType, _> =
    Result.builder {
        let! (prefix , rest) = xElement (nameGuard ("Prefix", ns) parseScanRangeType) (xElementsAll seekPredicate)
        let! (startRange, rest) = xElement (nameGuard ("StartRange", ns) parseScanRangeType) rest
        let! (endRange, rest) = xElement (nameGuard ("EndRange", ns) parseScanRangeType) rest
        let! (isNotNull, rest) = xElement (nameGuard ("IsNotNull", ns) parseSingleColumnReferenceType) rest
        do! ensureEmpty rest
        return { 
            Prefix = prefix
            StartRange = startRange
            EndRange = endRange
            IsNotNull = isNotNull
        }
    }

and parseSeekPredicateNew (seekPredicateNew : Linq.XElement) : Result<SeekPredicateNewType, _> =
    Result.builder {
        let! (seekKeys, rest) = xElementMany1 (nameGuard ("SeekKeys", ns) parseSeekPredicate) (xElementsAll seekPredicateNew)
        do! ensureEmpty rest
        return { 
            SeekKeys = seekKeys // 1..2
        }
    }

and parseSeekPredicatePart (seekPredicatePart : Linq.XElement) : Result<SeekPredicatePartType, _> =
    Result.builder {
        let! (seekPredicatesNew, rest) = xElementMany1 (nameGuard ("SeekPredicatesNew", ns) parseSeekPredicateNew) (xElementsAll seekPredicatePart)
        do! ensureEmpty rest
        return { 
            SeekPredicatesNew = seekPredicatesNew
        }
    }

and parseSeekPredicatesType (seekPredicates : Linq.XElement) : Result<SeekPredicatesType, _> =
    Result.builder {
        let! (seekPredicatesNodes, rest) = xElementMany (nameGuard ("SeekPredicateType", ns) parseSeekPredicate) (xElementsAll seekPredicates)
        let! (seekPredicateNew, rest) = xElementMany (nameGuard ("SeekPredicateNew", ns) parseSeekPredicateNew) rest
        let! (seekPredicatePart, rest) = xElementMany (nameGuard ("SeekPredicatePart", ns) parseSeekPredicatePart) rest
        do! ensureEmpty rest

        return! 
            match seekPredicatesNodes, seekPredicateNew, seekPredicatePart with
            | _, [], [] -> Ok (SeekPredicatesType.SeekPredicate seekPredicatesNodes)
            | [], _, [] -> Ok (SeekPredicatesType.SeekPredicateNew seekPredicateNew)
            | [], [], _ -> Ok (SeekPredicatesType.SeekPredicatePart seekPredicatePart)
            | _ -> Errorf "SeekPredicates can only contain one of SeekPredicate, SeekPredicateNew or SeekPredicatePart"
    }

and parseIndexedViewInfoType (indexedViewInfo : Linq.XElement) : Result<IndexedViewInfoType, _> =
    Result.builder {
        let! (objects, rest) = xElementMany (nameGuard ("Object", ns) parseObjectType) (xElementsAll indexedViewInfo)
        do! ensureEmpty rest
        return { Objects = objects }
    }

and parseIndexScanType (indexScan : Linq.XElement) : Result<IndexScanType, _> =
    Result.builder {
        let! (rowsetBase, rest) = parseRowsetBase indexScan 
        let! (seekPredicates, rest) = xElement (nameGuard ("SeekPredicates", ns) parseSeekPredicatesType) rest
        let! (predicates, rest) = xElementMany (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (partitionId, rest) = xElement (nameGuard ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (indexedViewInfo, rest) = xElement (nameGuard ("IndexedViewInfo", ns) parseIndexedViewInfoType) rest
        do! ensureEmpty rest
        
        let! lookup = xAttr "Lookup" indexScan
        let! ordered = xAttrReq "Ordered" indexScan
        let! scanDirection = 
            xAttrP "ScanDirection" 
                (function 
                    | "FORWARD" -> OrderType.FORWARD |> Ok
                    | "BACKWARD" -> OrderType.BACKWARD |> Ok
                    | name -> Errorf "Invalid ScanDirection value: '%s'" name)
                indexScan
                 
        let! forcedIndex = xAttr "ForcedIndex" indexScan
        let! forceSeek = xAttr "ForceSeek" indexScan
        let! forceSeekColumnCount = xAttr "ForceSeekColumnCount" indexScan
        let! forceScan = xAttr "ForceScan" indexScan
        let! noExpandHint = xAttr "NoExpandHint" indexScan
        let! storage = 
            xAttrP "Storage" 
                (function 
                    | "ColumnStore" -> StorageType.ColumnStore |> Ok
                    | "MemoryOptimized" -> StorageType.MemoryOptimized |> Ok
                    | "RowStore" -> StorageType.RowStore |> Ok
                    | name -> Errorf "Invalid StorageType value: '%s'" name)
                indexScan
                 
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
and parseForeignKeyReferenceCheckType (fkCheck : Linq.XElement) : Result<ForeignKeyReferenceCheckType, _> =
    Result.builder {
        let! (indexScan, rest) = xElementReq (nameGuard ("IndexScan", ns) parseIndexScanType) (xElementsAll fkCheck) 
        do! ensureEmpty rest
        return { 
            IndexScan = indexScan
        }
    }
and parseForeignKeyReferencesCheckType (fkCheck : Linq.XElement) : Result<ForeignKeyReferencesCheckType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase fkCheck 
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        let! (foreignKeyReferenceChecks, rest) = xElementMany (nameGuard ("ForeignKeyReferenceCheck", ns) parseForeignKeyReferenceCheckType) rest
        do! ensureEmpty rest
        
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

and parseGbAggType (gbAgg : Linq.XElement) : Result<GbAggType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase gbAgg 
        let! (groupBy, rest) = xElement (nameGuard ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (aggFunctions, rest) = xElement (nameGuard ("Aggregate", ns) parseDefinedValuesListType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
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

and parseGbApplyType (gbApply : Linq.XElement) : Result<GbApplyType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase gbApply 
        let! (predicates, rest) = xElementMany (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (aggFunctions, rest) = xElement (nameGuard ("AggFunctions", ns) parseDefinedValuesListType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
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

and parseGenericType (generic : Linq.XElement) : Result<GenericType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase generic 
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            RelOps = relOps
        }
    }

and parseHashType (hash : Linq.XElement) : Result<HashType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase hash 
        let! (hashKeysBuild, rest) = xElement (nameGuard ("HashKeysBuild", ns) parseColumnReferenceListType) rest
        let! (hashKeysProbe, rest) = xElement (nameGuard ("HashKeysProbe", ns) parseColumnReferenceListType) rest
        let! (buildResidual, rest) = xElement (nameGuard ("BuildResidual", ns) parseScalarExpressionType) rest
        let! (probeResidual, rest) = xElement (nameGuard ("ProbeResidual", ns) parseScalarExpressionType) rest
        let! (starJoinInfo, rest) = xElement (nameGuard ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
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

and parseGroupingSetReferenceType (groupingSetReference : Linq.XElement) : Result<GroupingSetReferenceType, _> =
    Result.builder {
        let! value = xAttrReq "Value" groupingSetReference
        return { Value = value }
    }
and parseGroupingSetListType (groupingSetList : Linq.XElement) : Result<GroupingSetListType, _> =
    Result.builder {
        let! (groupingSets, rest) = xElementMany (nameGuard ("GroupingSet", ns) parseGroupingSetReferenceType) (xElementsAll groupingSetList)
        do! ensureEmpty rest
        return { GroupingSets = groupingSets }
    }

and parseLocalCubeType (localCube : Linq.XElement) : Result<LocalCubeType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase localCube 
        let! (groupBy, rest) = xElement (nameGuard ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (groupingSets, rest) = xElement (nameGuard ("GroupingSets", ns) parseGroupingSetListType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            GroupBy = groupBy
            GroupingSets = groupingSets
            RelOps = relOps
        }
    }

and parseMergeType (merge : Linq.XElement) : Result<MergeType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase merge 
        let! (innerSideJoinColumns, rest) = xElement (nameGuard ("InnerSideJoinColumns", ns) parseColumnReferenceListType) rest
        let! (outerSideJoinColumns, rest) = xElement (nameGuard ("OuterSideJoinColumns", ns) parseColumnReferenceListType) rest
        let! (residual, rest) = xElement (nameGuard ("Residual", ns) parseScalarExpressionType) rest
        let! (passThru, rest) = xElement (nameGuard ("PassThru", ns) parseScalarExpressionType) rest
        let! (starJoinInfo, rest) = xElement (nameGuard ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
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

and parseSimpleIteratorOneChildType (simpleIteratorOneChild : Linq.XElement) : Result<SimpleIteratorOneChildType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase simpleIteratorOneChild 
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            RelOp = relOp
        }
    }

and parseResourceEstimateType (resourceEstimate : Linq.XElement) : Result<ResourceEstimateType, _> =
    Result.builder {
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

and parseMoveType (move : Linq.XElement) : Result<MoveType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase move 
        let! (distributionKey, rest) = xElement (nameGuard ("DistributionKey", ns) parseColumnReferenceListType) rest
        let! (resourceEstimate, rest) = xElement (nameGuard ("ResourceEstimate", ns) parseResourceEstimateType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest

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

and parseNestedLoopsType (nestedLoops : Linq.XElement) : Result<NestedLoopsType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase nestedLoops 
        let! (predicate, rest) = xElement (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (passThru, rest) = xElement (nameGuard ("PassThru", ns) parseScalarExpressionType) rest
        let! (outerReferences, rest) = xElement (nameGuard ("OuterReferences", ns) parseColumnReferenceListType) rest
        let! (partitionId, rest) = xElement (nameGuard ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (probeColumn, rest) = xElement (nameGuard ("ProbeColumn", ns) parseSingleColumnReferenceType) rest
        let! (starJoinInfo, rest) = xElement (nameGuard ("StarJoinInfo", ns) parseStarJoinInfoType) rest
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
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

and parseOrderByColumnType (orderByColumn : Linq.XElement) : Result<OrderByColumnType, _> =
    Result.builder {
        let! (columnReference, rest) = xElementReq (nameGuard ("ColumnReference", ns) parseColumnReferenceType) (xElementsAll orderByColumn)
        do! ensureEmpty rest
        let! ascending = xAttrReq "Ascending" orderByColumn
        return { ColumnReference = columnReference; Ascending = ascending }
    }

and parseOrderByType (orderBy : Linq.XElement) : Result<OrderByType, _> =
    Result.builder {
        let! (orderByColumns, rest) = xElementMany (nameGuard ("OrderByColumn", ns) parseOrderByColumnType) (xElementsAll orderBy)
        do! ensureEmpty rest
        return { OrderByColumns = orderByColumns }
    }

and parseActivationInfoType (activationInfo : Linq.XElement) : Result<ActivationInfoType, _> =
    Result.builder {
        let! (object, rest) = xElement (nameGuard ("Object", ns) parseObjectType) (xElementsAll activationInfo)
        do! ensureEmpty rest

        let! type_ = xAttrReq "Type" activationInfo
        let! fragmentElimination = xAttr "FragmentElimination" activationInfo
        return { 
            Object = object 
            Type = type_
            FragmentElimination = fragmentElimination 
        }
    }

and parseBrickRoutingType (brickRouting : Linq.XElement) : Result<BrickRoutingType, _> =
    Result.builder {
        let! (object, rest) = xElement (nameGuard ("Object", ns) parseObjectType) (xElementsAll brickRouting)
        let! (fragmentIdColumn, rest) = xElement (nameGuard ("FragmentIdColumn", ns) parseSingleColumnReferenceType) rest
        do! ensureEmpty rest
        return { 
            Object = object
            FragmentIdColumn = fragmentIdColumn 
        }
    }

and parseParallelismType (parallelism : Linq.XElement) : Result<ParallelismType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase parallelism 
        let! (partitionColumns, rest) = xElement (nameGuard ("PartitionColumns", ns) parseColumnReferenceListType) rest
        let! (orderBy, rest) = xElement (nameGuard ("OrderBy", ns) parseOrderByType) rest
        let! (hashKeys, rest) = xElement (nameGuard ("HashKeys", ns) parseColumnReferenceListType) rest
        let! (probeColumn, rest) = xElement (nameGuard ("ProbeColumn", ns) parseSingleColumnReferenceType) rest
        let! (predicate, rest) = xElement (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (activation, rest) = xElement (nameGuard ("Activation", ns) parseActivationInfoType) rest
        let! (brickRouting, rest) = xElement (nameGuard ("BrickRouting", ns) parseBrickRoutingType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! partitioningType = xAttrP "PartitioningType" parsePartitioningType parallelism 
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

and parseProjectType (project : Linq.XElement) : Result<ProjectType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase project 
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! isNoOp = xAttr "IsNoOp" project
        return { 
            Base = relOpBase
            RelOps = relOps
            IsNoOp = isNoOp
        }
    }

and parseRemoteQueryBase (remoteQueryBase : Linq.XElement) : Result<RemoteQueryType*_, _> =
    Result.builder {
        let! (remoteBase, rest) = parseRemoteTypeBase remoteQueryBase
        
        let! remoteObject = xAttr "RemoteObject" remoteQueryBase
        return { RemoteBase = remoteBase; RemoteQuery = remoteObject }, rest
    }

and parseRemoteQuery (remoteQueryBase : Linq.XElement) : Result<RemoteQueryType, _> =
    Result.builder {
        let! (remoteBase, rest) = parseRemoteQueryBase remoteQueryBase
        do! ensureEmpty rest

        return remoteBase
    }

and parsePutType (put : Linq.XElement) : Result<PutType, _> =
    Result.builder {
        let! (remoteBase, rest) = parseRemoteQueryBase put 
        let! (relOp, rest) = xElement (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! isExternallyComputed = xAttr "IsExternallyComputed" put
        let! shuffleType = xAttr "ShuffleType" put
        let! shuffleColumn = xAttr "ShuffleColumn" put
        return { 
            RemoteBase = remoteBase
            RelOp = relOp
            
            IsExternallyComputed = isExternallyComputed
            ShuffleType = shuffleType
            ShuffleColumn = shuffleColumn
        }
    }

and parseRemoteFetchType (remoteFetch : Linq.XElement) : Result<RemoteFetchType, _> =
    Result.builder {
        let! (remoteBase, rest) = parseRemoteTypeBase remoteFetch 
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            RemoteBase = remoteBase
            RelOp = relOp
        }
    }

and parseRemoteModifyType (remoteModify : Linq.XElement) : Result<RemoteModifyType, _> =
    Result.builder {
        let! (remoteBase, rest) = parseRemoteTypeBase remoteModify 
        let! (setPredicate, rest) = xElement (nameGuard ("SetPredicate", ns) parseScalarExpressionType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        return { 
            RemoteBase = remoteBase
            SetPredicate = setPredicate 
            RelOp = relOp
        }
    }

and parseRemoteRangeType (remoteRange : Linq.XElement) : Result<RemoteRangeType, _> =
    Result.builder {
        let! (remoteBase, rest) = parseRemoteTypeBase remoteRange 
        let! (seekPredicates, rest) = xElement (nameGuard ("SeekPredicates", ns) parseSeekPredicatesType) rest
        do! ensureEmpty rest
        return { 
            RemoteBase = remoteBase
            SeekPredicates = seekPredicates
        }
    }

and parseRelOpSeekPredicate (elements : Linq.XElement list) =
    Result.builder {
        let! (seekPredicate', rest) = xElement (nameGuard ("SeekPredicate", ns) parseSeekPredicate) elements
        let! (seekPredicateNew', rest) = xElement (nameGuard ("SeekPredicateNew", ns) parseSeekPredicateNew) rest
        return!
            match seekPredicate', seekPredicateNew' with
            | Some sp, None -> Ok (Some (RelOpSeekPredicate.SeekPredicate sp), rest)
            | None, Some spNew -> Ok (Some (RelOpSeekPredicate.SeekPredicateNew spNew), rest)
            | None, None -> Ok (None, rest)
            | _ -> Errorf "Spool operator should have at most one SeekPredicate or SeekPredicateNew element"
    }

and parseSpoolType (spool : Linq.XElement) : Result<SpoolType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase spool 
        let! (seekPredicate, rest) = parseRelOpSeekPredicate rest
        let! (relOp, rest) = xElement (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! stack = xAttr "Stack" spool
        let! primaryNodeId = xAttr "PrimaryNodeId" spool
        return { 
            Base = relOpBase
            SeekPredicate = seekPredicate
            RelOp = relOp
            Stack = stack
            PrimaryNodeId = primaryNodeId
        }
    }

and parseScalarInsertType (scalarInsert : Linq.XElement) : Result<ScalarInsertType, _> =
    Result.builder {
        let! (rowsetBase, rest) = parseRowsetBase scalarInsert 
        let! (setPredicate, rest) = xElement (nameGuard ("SetPredicate", ns) parseScalarExpressionType) rest
        do! ensureEmpty rest
        
        let! dmlRequestSort = xAttr "DMLRequestSort" scalarInsert
        return { 
            RowsetBase = rowsetBase
            SetPredicate = setPredicate
            DMLRequestSort = dmlRequestSort
        }
    }

and parseSegmentType (segment : Linq.XElement) : Result<SegmentType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase segment 
        let! (groupBy, rest) = xElementReq (nameGuard ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (segmentColumn, rest) = xElementReq (nameGuard ("SegmentColumn", ns) parseSingleColumnReferenceType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        return { 
            Base = relOpBase
            GroupBy = groupBy
            SegmentColumn = segmentColumn 
            RelOp = relOp
        }
    }

and parseSequenceType (sequence : Linq.XElement) : Result<SequenceType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase sequence 
        let! (relOps, rest) = xElementMany2 (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest

        let! isGraphDBTransitiveClosure = xAttr "IsGraphDBTransitiveClosure" sequence
        let! graphSequenceIdentifier = xAttr "GraphSequenceIdentifier" sequence
        return { 
            Base = relOpBase
            RelOps = relOps // 2+
            IsGraphDBTransitiveClosure = isGraphDBTransitiveClosure
            GraphSequenceIdentifier = graphSequenceIdentifier
        }
    }

and parseSimpleUpdateType (simpleUpdate : Linq.XElement) : Result<SimpleUpdateType, _> =
    Result.builder {
        let! (rowsetBase, rest) = parseRowsetBase simpleUpdate 
        let! (seekPredicate, rest) = parseRelOpSeekPredicate rest
        let! (setPredicate, rest) = xElement (nameGuard ("SetPredicate", ns) parseScalarExpressionType) rest
        do! ensureEmpty rest
        
        let! dmlRequestSort = xAttr "DMLRequestSort" simpleUpdate
        return { 
            RowsetBase = rowsetBase
            SeekPredicate = seekPredicate
            SetPredicate = setPredicate 
            DMLRequestSort = dmlRequestSort
        }
    }

and parseSortType (sort : Linq.XElement) : Result<SortType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase sort 
        let! (orderBy, rest) = xElementReq (nameGuard ("OrderBy", ns) parseOrderByType) rest
        let! (partitionId, rest) = xElement (nameGuard ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! distinct = xAttrReq "Distinct" sort
        return { 
            Base = relOpBase
            OrderBy = orderBy
            PartitionId = partitionId 
            RelOp = relOp
            
            Distinct = distinct
        }
    }

and parseSplitType (split : Linq.XElement) : Result<SplitType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase split 
        let! (actionColumn, rest) = xElement (nameGuard ("ActionColumn", ns) parseSingleColumnReferenceType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            ActionColumn = actionColumn
            RelOp = relOp
        }
    }

and parseRollupLevelType (rollupLevel : Linq.XElement) : Result<RollupLevelType, _> =
    Result.builder {
        let! level = xAttrReq "Level" rollupLevel
        return { Level = level }
    }

and parseRollupInfoType (rollupInfo : Linq.XElement) : Result<RollupInfoType, _> =
    Result.builder {
        let! (rollupLevels, rest) = xElementMany2 (nameGuard ("RollupLevel", ns) parseRollupLevelType) (xElementsAll rollupInfo)
        do! ensureEmpty rest
        
        let! highestLevel = xAttrReq "HighestLevel" rollupInfo
        return { RollupLevels = rollupLevels; HighestLevel = highestLevel }
    }

and parseStreamAggregateType (streamAggregate : Linq.XElement) : Result<StreamAggregateType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase streamAggregate 
        let! (groupBy, rest) = xElement (nameGuard ("GroupBy", ns) parseColumnReferenceListType) rest
        let! (rollupInfo, rest) = xElement (nameGuard ("RollupInfo", ns) parseRollupInfoType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        return { 
            Base = relOpBase
            GroupBy = groupBy
            RollupInfo = rollupInfo
            RelOp = relOp
        }
    }

and parseConcatBase (concat : Linq.XElement) : Result<ConcatType*_, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase concat 
        let! (relOps, rest) = xElementMany (nameGuard ("RelOp", ns) parseRelOpType) rest
        return { Base = relOpBase; RelOps = relOps }, rest
    }

and parseSwitchType (switch : Linq.XElement) : Result<SwitchType, _> =
    Result.builder {
        let! (concatBase, rest) = parseConcatBase switch 
        let! (predicate, rest) = xElement (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        do! ensureEmpty rest
        
        return { 
            ConcatBase = concatBase
            Predicate = predicate
        }
    }

and parseTableScanType (tableScan : Linq.XElement) : Result<TableScanType, _> =
    Result.builder {
        let! (rowsetBase, rest) = parseRowsetBase tableScan 
        let! (predicate, rest) = xElement (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (partitionId, rest) = xElement (nameGuard ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (indexedViewInfo, rest) = xElement (nameGuard ("IndexedViewInfo", ns) parseIndexedViewInfoType) rest
        do! ensureEmpty rest
        
        let! ordered = xAttrReq "Ordered" tableScan
        let! forcedIndex = xAttr "ForcedIndex" tableScan
        let! forceScan = xAttr "ForceScan" tableScan
        let! noExpandHint = xAttr "NoExpandHint" tableScan
        let! storage = xAttrP "Storage" parseStorage tableScan
        return { 
            RowsetBase = rowsetBase
            Predicate = predicate
            PartitionId = partitionId
            IndexedViewInfo = indexedViewInfo
            
            Ordered = ordered
            ForcedIndex = forcedIndex
            ForceScan = forceScan
            NoExpandHint = noExpandHint
            Storage = storage 
        }
    }

and parseTableValuedFunctionType (tableValuedFunction : Linq.XElement) : Result<TableValuedFunctionType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase tableValuedFunction 
        let! (object, rest) = xElement (nameGuard ("Object", ns) parseObjectType) rest
        let! (predicate, rest) = xElement (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (relOp, rest) = xElement (nameGuard ("RelOp", ns) parseRelOpType) rest
        let! (parameterList, rest) = xElement (nameGuard ("ParameterList", ns) parseScalarExpressionListType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            Object = object
            Predicate = predicate
            RelOp = relOp
            ParameterList = parameterList
        }
    }

and parseTopType (top : Linq.XElement) : Result<TopType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase top 
        let! (tieColumns, rest) = xElement (nameGuard ("TieColumns", ns) parseColumnReferenceListType) rest
        let! (offsetExpression, rest) = xElement (nameGuard ("OffsetExpression", ns) parseScalarExpressionType) rest
        let! (topExpression, rest) = xElement (nameGuard ("TopExpression", ns) parseScalarExpressionType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! rowCount = xAttr "RowCount" top
        let! rows = xAttr "Rows" top
        let! isPercent = xAttr "IsPercent" top
        let! withTies = xAttr "WithTies" top
        let! topLocation = xAttr "TopLocation" top
        return { 
            Base = relOpBase
            TieColumns = tieColumns 
            OffsetExpression = offsetExpression
            TopExpression = topExpression
            RelOp = relOp
            
            RowCount = rowCount
            Rows = rows
            IsPercent = isPercent
            WithTies = withTies
            TopLocation = topLocation
        }
    }

and parseSortBase (sort : Linq.XElement) : Result<SortType*_, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase sort 
        let! (orderBy, rest) = xElementReq (nameGuard ("OrderBy", ns) parseOrderByType) rest
        let! (partitionId, rest) = xElement (nameGuard ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest

        let! distinct = xAttrReq "Distinct" sort
        
        return { 
            Base = relOpBase
            OrderBy = orderBy 
            PartitionId = partitionId
            RelOp = relOp

            Distinct = distinct
        }, rest
    }

and parseTopSortType (topSort : Linq.XElement) : Result<TopSortType, _> =
    Result.builder {
        let! (sortBase, rest) = parseSortBase topSort 
        do! ensureEmpty rest
        
        let! rows = xAttrReq "Rows" topSort
        let! withTies = xAttr "WithTies" topSort
        return { 
            SortBase = sortBase
            Rows = rows
            WithTies = withTies
        }
    }

and parseSetPredicateElementType (setPredicateElement : Linq.XElement) : Result<SetPredicateElementType, _> =
    Result.builder {
        let! (scalarOperator, rest) = xElementReq (nameGuard ("ScalarOperator", ns) parseScalarType) (xElementsAll setPredicateElement)
        do! ensureEmpty rest

        let! setPredicateType = xAttr "SetPredicateType" setPredicateElement
        return { 
            ScalarOperator = scalarOperator 
            SetPredicateType = setPredicateType 
        }
    }

and parseUpdateType (update : Linq.XElement) : Result<UpdateType, _> =
    Result.builder {
        let! (rowsetBase, rest) = parseRowsetBase update 
        let! (setPredicates, rest) = xElementMany (nameGuard ("SetPredicate", ns) parseSetPredicateElementType) rest
        let! (probeColumn, rest) = xElement (nameGuard ("ProbeColumn", ns) parseSingleColumnReferenceType) rest
        let! (actionColumn, rest) = xElement (nameGuard ("ActionColumn", ns) parseSingleColumnReferenceType) rest
        let! (originalActionColumn, rest) = xElement (nameGuard ("OriginalActionColumn", ns) parseSingleColumnReferenceType) rest
        let! (assignmentMap, rest) = xElement (nameGuard ("AssignmentMap", ns) parseAssignmentMapType) rest
        let! (sourceTable, rest) = xElement (nameGuard ("SourceTable", ns) parseParameterizationType) rest
        let! (targetTable, rest) = xElement (nameGuard ("TargetTable", ns) parseParameterizationType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! withOrderedPrefetch = xAttr "WithOrderedPrefetch" update
        let! withUnorderedPrefetch = xAttr "WithUnorderedPrefetch" update
        let! dmlRequestSort = xAttr "DMLRequestSort" update
        return { 
            RowsetBase = rowsetBase
            SetPredicates = setPredicates
            ProbeColumn = probeColumn
            ActionColumn = actionColumn
            OriginalActionColumn = originalActionColumn
            AssignmentMap = assignmentMap
            SourceTable = sourceTable
            TargetTable = targetTable
            RelOp = relOp
            
            WithOrderedPrefetch = withOrderedPrefetch 
            WithUnorderedPrefetch = withUnorderedPrefetch
            DMLRequestSort = dmlRequestSort
        }
    }

and parseWindowType (window : Linq.XElement) : Result<WindowType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase window 
        let! (relOp, rest) = xElement (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            RelOp = relOp
        }
    }

and parseWindowAggregateType (windowAggregate : Linq.XElement) : Result<WindowAggregateType, _> =
    Result.builder {
        let! (relOpBase, rest) = parseRelOpBase windowAggregate 
        let! (relOp, rest) = xElement (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        return { 
            Base = relOpBase
            RelOp = relOp
        }
    }

and parseXcsScanType (xcsScan : Linq.XElement) : Result<XcsScanType, _> =
    Result.builder {
        let! (rowsetBase, rest) = parseRowsetBase xcsScan 
        let! (predicate, rest) = xElement (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        let! (partitionId, rest) = xElement (nameGuard ("PartitionId", ns) parseSingleColumnReferenceType) rest
        let! (indexedViewInfo, rest) = xElement (nameGuard ("IndexedViewInfo", ns) parseIndexedViewInfoType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        do! ensureEmpty rest
        
        let! ordered = xAttrReq "Ordered" xcsScan
        let! forcedIndex = xAttr "ForcedIndex" xcsScan
        let! forceScan = xAttr "ForceScan" xcsScan
        let! noExpandHint = xAttr "NoExpandHint" xcsScan
        let! storage = xAttrP "Storage" parseStorage xcsScan
        return { 
            RowsetBase = rowsetBase
            Predicate = predicate
            PartitionId = partitionId
            IndexedViewInfo = indexedViewInfo
            RelOp = relOp

            Ordered = ordered
            ForcedIndex = forcedIndex
            ForceScan = forceScan
            NoExpandHint = noExpandHint
            Storage = storage
        }
    }

and parseRelOpDetails =
    createCaseParser
        [
            "AdaptiveJoin", parseAdaptiveJoinType >> Result.map RelOpDetails.AdaptiveJoin
            "Apply", parseJoinType >> Result.map RelOpDetails.Apply
            "Assert", parseFilterType >> Result.map RelOpDetails.Assert
            "BatchHashTableBuild", parseBatchHashTableBuildType >> Result.map RelOpDetails.BatchHashTableBuild
            "Bitmap", parseBitmapType >> Result.map RelOpDetails.Bitmap
            "Collapse", parseCollapseType >> Result.map RelOpDetails.Collapse
            "ComputeScalar", parseComputeScalarType >> Result.map RelOpDetails.ComputeScalar
            "Concat", parseConcatType >> Result.map RelOpDetails.Concat
            "ConstantScan", parseConstantScanType >> Result.map RelOpDetails.ConstantScan
            "ConstTableGet", parseGetType >> Result.map RelOpDetails.ConstTableGet
            "CreateIndex", parseCreateIndexType >> Result.map RelOpDetails.CreateIndex
            "Delete", parseDMLOpType >> Result.map RelOpDetails.Delete
            "DeletedScan", parseRowsetType >> Result.map RelOpDetails.DeletedScan
            "Extension", parseUDXType >> Result.map RelOpDetails.Extension
            "ExternalSelect", parseExternalSelectType >> Result.map RelOpDetails.ExternalSelect
            "ExtExtractScan", parseRemoteType >> Result.map RelOpDetails.ExtExtractScan
            "Filter", parseFilterType >> Result.map RelOpDetails.Filter
            "ForeignKeyReferencesCheck", parseForeignKeyReferencesCheckType >> Result.map RelOpDetails.ForeignKeyReferencesCheck
            "GbAgg", parseGbAggType >> Result.map RelOpDetails.GbAgg
            "GbApply", parseGbApplyType >> Result.map RelOpDetails.GbApply
            "Generic", parseGenericType >> Result.map RelOpDetails.Generic
            "Get", parseGetType >> Result.map RelOpDetails.Get
            "Hash", parseHashType >> Result.map RelOpDetails.Hash
            "IndexScan", parseIndexScanType >> Result.map RelOpDetails.IndexScan
            "InsertedScan", parseRowsetType >> Result.map RelOpDetails.InsertedScan
            "Insert", parseDMLOpType >> Result.map RelOpDetails.Insert
            "Join", parseJoinType >> Result.map RelOpDetails.Join
            "LocalCube", parseLocalCubeType >> Result.map RelOpDetails.LocalCube
            "LogRowScan", parseRelOpBase >> Result.bind (function r, [] -> RelOpDetails.LogRowScan r |> Ok | _ , rest -> Errorf "LogRowScan should not have child elements, but had %d" (List.length rest))
            "Merge", parseMergeType >> Result.map RelOpDetails.Merge
            "MergeInterval", parseSimpleIteratorOneChildType >> Result.map RelOpDetails.MergeInterval
            "Move", parseMoveType >> Result.map RelOpDetails.Move
            "NestedLoops", parseNestedLoopsType >> Result.map RelOpDetails.NestedLoops
            "OnlineIndex", parseCreateIndexType >> Result.map RelOpDetails.OnlineIndex
            "Parallelism", parseParallelismType >> Result.map RelOpDetails.Parallelism
            "ParameterTableScan", parseRelOpBase >>  Result.bind (function r, [] -> RelOpDetails.ParameterTableScan r |> Ok | _ , rest -> Errorf "ParameterTableScan should not have child elements, but had %d" (List.length rest)) 
            "PrintDataflow", parseRelOpBase >>  Result.bind (function r, [] -> RelOpDetails.PrintDataflow r |> Ok | _ , rest -> Errorf "PrintDataflow should not have child elements, but had %d" (List.length rest))
            "Project", parseProjectType >> Result.map RelOpDetails.Project
            "Put", parsePutType >> Result.map RelOpDetails.Put
            "RemoteFetch", parseRemoteFetchType >> Result.map RelOpDetails.RemoteFetch
            "RemoteModify", parseRemoteModifyType >> Result.map RelOpDetails.RemoteModify
            "RemoteQuery", parseRemoteQuery >> Result.map RelOpDetails.RemoteQuery
            "RemoteRange", parseRemoteRangeType >> Result.map RelOpDetails.RemoteRange
            "RemoteScan", parseRemoteType >> Result.map RelOpDetails.RemoteScan
            "RowCountSpool", parseSpoolType >> Result.map RelOpDetails.RowCountSpool
            "ScalarInsert", parseScalarInsertType >> Result.map RelOpDetails.ScalarInsert
            "Segment", parseSegmentType >> Result.map RelOpDetails.Segment
            "Sequence", parseSequenceType >> Result.map RelOpDetails.Sequence
            "SequenceProject", parseComputeScalarType >> Result.map RelOpDetails.SequenceProject
            "SimpleUpdate", parseSimpleUpdateType >> Result.map RelOpDetails.SimpleUpdate
            "Sort", parseSortType >> Result.map RelOpDetails.Sort
            "Split", parseSplitType >> Result.map RelOpDetails.Split
            "Spool", parseSpoolType >> Result.map RelOpDetails.Spool
            "StreamAggregate", parseStreamAggregateType >> Result.map RelOpDetails.StreamAggregate
            "Switch", parseSwitchType >> Result.map RelOpDetails.Switch
            "TableScan", parseTableScanType >> Result.map RelOpDetails.TableScan
            "TableValuedFunction", parseTableValuedFunctionType >> Result.map RelOpDetails.TableValuedFunction
            "Top", parseTopType >> Result.map RelOpDetails.Top
            "TopSort", parseTopSortType >> Result.map RelOpDetails.TopSort
            "Update", parseUpdateType >> Result.map RelOpDetails.Update
            "Union", parseConcatType >> Result.map RelOpDetails.Union
            "UnionAll", parseConcatType >> Result.map RelOpDetails.UnionAll
            "WindowSpool", parseWindowType >> Result.map RelOpDetails.WindowSpool
            "WindowAggregate", parseWindowAggregateType >> Result.map RelOpDetails.WindowAggregate
            "XcsScan", parseXcsScanType >> Result.map RelOpDetails.XcsScan
        ]

and parseRelOpType (relOp : Linq.XElement) : Result<RelOpType, _> =
    Result.builder {
        let! (columnsReferences, rest) = xElementReq (nameGuard ("OutputList", ns) parseColumnReferenceListType) (xElementsAll relOp)
        let! (warnings, rest) = xElement (nameGuard ("Warnings", ns) parseWarningsType) rest
        let! (memoryFractions, rest) = xElement (nameGuard ("MemoryFractions", ns) parseMemoryFractionsType) rest
        let! (runTimeInformation, rest) = xElement (nameGuard ("RunTimeInformation", ns) parseRunTimeInformation) rest
        let! (runTimePartitionSummary, rest) = xElement (nameGuard ("RunTimePartitionSummary", ns) parseRunTimePartitionSummary) rest
        let! (internalInfoType, rest) = xElement (nameGuard ("InternalInfo", ns) parseInternalInfoType) rest
        let! (operatorDetails, rest) = xElement parseRelOpDetails rest
        do! ensureEmpty rest
        
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

let parseOptimizationReplay (optimizationReplay : Linq.XElement) : Result<OptimizationReplayType, _> =
    Result.builder {
        let! script = xAttrReq "Script" optimizationReplay
        return { Script = script }
    }

let parseThreadReservations (threadReservation : Linq.XElement) : Result<ThreadReservationType, _> =
    Result.builder {
        let! nodeId = xAttr "NodeId" threadReservation
        let! reservedThreads = xAttrReq "ReservedThreads" threadReservation
        return { 
            NodeId = nodeId
            ReservedThreads = reservedThreads
        }
    }

let parseThreadStatType (threadStat : Linq.XElement) : Result<ThreadStatType, _> =
    Result.builder {
        let! branches = xAttrReq "ThreadId" threadStat
        let! usedThreads = xAttr "UsedThreads" threadStat
        let! (threadReservations, rest) = xElementMany (nameGuard ("ThreadReservations", ns) parseThreadReservations) (xElementsAll threadStat)
        do! ensureEmpty rest
        return { 
            Branches = branches
            UsedThreads = usedThreads
            ThreadReservations = threadReservations
        }
    }

let parseMissingColumnType (missingColumn : Linq.XElement) : Result<MissingColumnType, _> =
    Result.builder {
        let! name = xAttrReq "Name" missingColumn
        let! columnId = xAttrReq "ColumnId" missingColumn
        return { Name = name; ColumnId = columnId }
    }
let parseColumnGroup (columnGroup : Linq.XElement) : Result<ColumnGroupType, _> =
    Result.builder {
        let! usage = xAttrReq "Usage" columnGroup
        let! (columns, rest) = xElementMany (nameGuard ("Column", ns) parseMissingColumnType) (xElementsAll columnGroup)
        do! ensureEmpty rest
        return { 
            Usage = usage
            Columns = columns
        }
    }
let parseMissingIndexType (missingIndex : Linq.XElement) : Result<MissingIndexType, _> =
    Result.builder {
        let! database = xAttrReq "Database" missingIndex
        let! schema = xAttrReq "Schema" missingIndex
        let! table = xAttrReq "Table" missingIndex
        let! (columnGroups, rest) = xElementMany (nameGuard ("ColumnGroup", ns) parseColumnGroup) (xElementsAll missingIndex)
        do! ensureEmpty rest
        return { 
            Database = database
            Schema = schema
            Table = table
            ColumnGroups = columnGroups
        }
    }

let parseMissingIndexGroup (missingIndexGroup : Linq.XElement) : Result<MissingIndexGroupType, _> =
    Result.builder {
        let! impact = xAttrReq "Impact" missingIndexGroup
        let! (missingIndexes, rest) = xElementMany (nameGuard ("MissingIndex", ns) parseMissingIndexType) (xElementsAll missingIndexGroup)
        do! ensureEmpty rest
        return { 
            Impact = impact
            MissingIndexes = missingIndexes
        }
    }

let parseMissingIndexesType (missingIndexes : Linq.XElement) : Result<MissingIndexesType, _> =
    Result.builder {
        let! (missingIndexGroups, rest) = xElementMany (nameGuard ("MissingIndexGroup", ns) parseMissingIndexGroup) (xElementsAll missingIndexes)
        do! ensureEmpty rest
        return { MissingIndexGroups = missingIndexGroups }
    }

let parseGuessedSelectivityType (guessedSelectivity : Linq.XElement) : Result<GuessedSelectivityType, _> =
    Result.builder {
        let! (spatial, rest) = xElementReq (nameGuard ("Spatial", ns) parseObjectType) (xElementsAll guessedSelectivity)
        do! ensureEmpty rest
        return {  Spatial = spatial }
    }

let parseUnmatchedIndexesType (unmatchedIndexes : Linq.XElement) : Result<UnmatchedIndexesType, _> =
    ensureOnly (xElementReq (nameGuard ("Parameterization", ns) parseParameterizationType)) 
        (fun parameterization -> { Parameterization = parameterization })
        unmatchedIndexes

let parseTraceFlag (traceFlag : Linq.XElement) : Result<TraceFlag, _> =
    Result.builder {
        let! value = xAttrReq "Value" traceFlag
        let! scope = xAttrReq "Scope" traceFlag
        return { Value = value; Scope = scope }
    }
let parseTraceFlagsType (traceFlags : Linq.XElement) : Result<TraceFlagListType, _> =
    Result.builder {
        let! isCompileTime = xAttrReq "IsCompileTime" traceFlags
        let! (traceFlags, rest) = xElementMany (nameGuard ("TraceFlag", ns) parseTraceFlag) (xElementsAll traceFlags)
        do! ensureEmpty rest
        return { IsCompileTime = isCompileTime; TraceFlags = traceFlags }
    }

let parseWaitStatType (waitStat : Linq.XElement) : Result<WaitStatType, _> =
    Result.builder {
        let! waitType = xAttrReq "WaitType" waitStat
        let! waitTimeMs = xAttrReq "WaitTimeMs" waitStat
        let! waitCount = xAttrReq "WaitCount" waitStat
        return { WaitType = waitType; WaitTimeMs = waitTimeMs; WaitCount = waitCount }
    }

let parseWaitStatListType (waitStats : Linq.XElement) : Result<WaitStatType list, _> =
    ensureOnly (xElementMany (nameGuard ("Wait", ns) parseWaitStatType)) id waitStats

let parseQueryTimeStatsType (queryTimeStats : Linq.XElement) : Result<QueryExecTimeType, _> =
    Result.builder {
        let! cpuTime = xAttrReq "QueryExecutionTimeMs" queryTimeStats
        let! elapsedTime = xAttrReq "QueryCompilationTimeMs" queryTimeStats
        let! udfCpuTime = xAttr "UdfCpuTime" queryTimeStats
        let! udfElapsedTime = xAttr "UdfElapsedTime" queryTimeStats
        return { CpuTime = cpuTime; ElapsedTime = elapsedTime; UdfCpuTime = udfCpuTime; UdfElapsedTime = udfElapsedTime }
    }

let parseQueryPlanType (queryPlan : Linq.XElement) : Result<QueryPlanType, _> =
    Result.builder {
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

        let! (internalInfo, rest) = xElement (nameGuard ("InternalInfo", ns) parseInternalInfoType) (xElementsAll queryPlan)
        let! (optimizationReplay, rest) = xElement (nameGuard ("OptimizationReplay", ns) parseOptimizationReplay) rest
        let! (threadStat, rest) = xElement (nameGuard ("ThreadStat", ns) parseThreadStatType) rest
        let! (missingIndexes, rest) = xElement (nameGuard ("MissingIndexes", ns) parseMissingIndexesType) rest
        let! (guessedSelectivity, rest) = xElement (nameGuard ("GuessedSelectivity", ns) parseGuessedSelectivityType) rest
        let! (unmatchedIndexes, rest) = xElement (nameGuard ("UnmatchedIndexes", ns) parseUnmatchedIndexesType) rest
        let! (warnings, rest) = xElement (nameGuard ("Warnings", ns) parseWarningsType) rest
        let! (memoryGrantInfo, rest) = xElement (nameGuard ("MemoryGrantInfo", ns) parseMemoryGrantType) rest
        let! (optimizerHardwareDependentProperties, rest) = 
            xElement (nameGuard ("OptimizerHardwareDependentProperties", ns) parseOptimizerHardwareDependentProperties) rest
        let! (optimizerStatsUsage, rest) = xElement (nameGuard ("OptimizerStatsUsage", ns) parseOptimizerStatsUsage) rest
        // "CardinalityFeedback" is not documented by Microsoft and is not part of the official XSD
        let! (_cardinalityFeedback, rest) = xElement (nameGuard ("CardinalityFeedback", ns) parseUndocumentedElement) rest
        let! (traceFlags, rest) = xElementMany (nameGuard ("TraceFlags", ns) parseTraceFlagsType) rest
        let! (waitStats, rest) = xElement (nameGuard ("WaitStats", ns) parseWaitStatListType) rest
        let! (queryTimeStats, rest) = xElement (nameGuard ("QueryTimeStats", ns) parseQueryTimeStatsType) rest
        let! (relOp, rest) = xElementReq (nameGuard ("RelOp", ns) parseRelOpType) rest
        let! (parameterList, rest) = xElement (nameGuard ("ParameterList", ns) parseColumnReferenceListType) rest
        do! ensureEmpty rest
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

let parseReceivePlanType (receiveOperation : Linq.XElement) : Result<ReceivePlanDetailType, _> =
    Result.builder {
        let! operationType' = xAttrReq "OperationType" receiveOperation
        let! operationType =    
            // Todo
            match operationType' with
            | "ReceivePlanSelect" -> ReceivePlanOperationType.ReceivePlanSelect |> Ok
            | "ReceivePlanUpdate" -> ReceivePlanOperationType.ReceivePlanUpdate |> Ok
            | opType -> Errorf "Unknown ReceivePlan OperationType: '%s'" opType
        let xmlElements = xElementsAll receiveOperation
        let! (queryPlan,rest) = xElementReq (nameGuard ("QueryPlan", ns) parseQueryPlanType) xmlElements
        do! ensureEmpty rest
        return {
            OperationType = operationType
            QueryPlan = queryPlan
        }
    }

let parseParameterSensitivePredicateType (parameterSensitivePredicate : Linq.XElement) : Result<ParameterSensitivePredicateType, _> =
    Result.builder {
        let! lowBoundery = xAttrReq "LowBoundery" parameterSensitivePredicate
        let! highBoundery = xAttrReq "HighBoundery" parameterSensitivePredicate
        let! (statisticsInfo, rest) = xElementMany1 (nameGuard ("StatisticsInfo", ns) parseStatsInfo) (xElementsAll parameterSensitivePredicate)
        let! (predicate, rest) = xElementReq (nameGuard ("Predicate", ns) parseScalarExpressionType) rest
        do! ensureEmpty rest
        return { 
            StatisticsInfo = statisticsInfo
            Predicate = predicate
            LowBoundery = lowBoundery
            HighBoundary = highBoundery
        }
    }
let parseDispatcherType (dispatcher : Linq.XElement) : Result<DispatcherType, _> =
    Result.builder {
        let! (parameterSensitivePredicats, rest) = 
            xElementMany (nameGuard ("ParameterSensitivePredicate", ns) parseParameterSensitivePredicateType) (xElementsAll dispatcher)
        do! ensureEmpty rest
        return { 
            ParameterSensitivePredicate = parameterSensitivePredicats
        }
    }


let parseStmtCursor (stmtCursor : Linq.XElement) : Result<StmtCursorType, _> =
    Result.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtCursor
        return failwith "NYI: StmtCursor parsing not yet implemented"
    }

let parseStmtReceive (stmtReceive : Linq.XElement) : Result<StmtReceiveType, _> =
    Result.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtReceive
        let! (receivePlans,rest) = xElementMany (nameGuard ("ReceivePlan", ns) parseReceivePlanType) rest
        do! ensureEmpty rest
        return { 
            BaseInfo = baseInfo
            ReceivePlans = receivePlans
        }
    }

let parseStmtUseDb (stmtUseDb : Linq.XElement) : Result<StmtUseDbType, _> =
    Result.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtUseDb
        let! database = xAttrReq "Database" stmtUseDb
        do! ensureEmpty rest
        return { 
            BaseInfo = baseInfo
            Database = database
        }
    }

let parseExternalDistributedComputation (externalDistributedComputation : Linq.XElement) : Result<ExternalDistributedComputationType, _> =
    Result.builder {
        let! edcShowplanXml = xAttrReq "EdcShowplanXml" externalDistributedComputation
        return { 
            EdcShowplanXml = edcShowplanXml 
        }
    }

// ========================================
// Mutual recursion for statement block types
// ========================================

let rec parseStmtSimple (stmtSimple : Linq.XElement) : Result<StmtSimpleType, _> =
    Result.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtSimple
        let! (dispatcher, rest) = xElement (nameGuard ("Dispatcher", ns) parseDispatcherType) rest 
        let! (queryPlan, rest) = xElement (nameGuard ("QueryPlan", ns) parseQueryPlanType) rest 
        let! (udfs, rest) = xElementMany (nameGuard ("UDF", ns) parseFunctionType) rest 
        let! (storedProc, rest) = xElement (nameGuard ("StoredProc", ns) parseFunctionType) rest 
        do! ensureEmpty rest
        return { 
            BaseInfo = baseInfo; 
            Dispatcher = dispatcher
            QueryPlan = queryPlan 
            UDFs = udfs
            StoredProc = storedProc
        }
    }

and parseFunctionType (functionType : Linq.XElement) : Result<FunctionType, _> =
    Result.builder {
        let xmlElements = xElementsAll functionType
        let! (statements, rest) = xElementMany (nameGuard ("Statements", ns) parseStmtBlockType) xmlElements
        do! ensureEmpty rest

        let! procName = xAttrReq "ProcName" functionType
        let! isNativelyCompiled = xAttr "IsNativelyCompiled" functionType
        return {
            Statements = statements
            ProcName = procName
            IsNativelyCompiled = isNativelyCompiled 
        }
    }

and parseStmtCond (stmtCond: Linq.XElement) : Result<StmtCondType, _> =
    Result.builder {
        let! (baseInfo, rest) = parseBaseStmtInfoType stmtCond
        let! (condition, rest) =
            xElementReq 
                (nameGuard ("Condition", ns)
                    (fun (condition : Linq.XElement) ->
                        Result.builder {
                            let allElements = xElementsAll condition
                            let! (queryPlan, rest) = xElement (nameGuard ("QueryPlan", ns) parseQueryPlanType) allElements
                            let! (udfFunctions, rest) = xElementMany (nameGuard ("UDF", ns) parseFunctionType) rest
                            do! ensureEmpty rest
                            return { QueryPlan = queryPlan; UDFs = udfFunctions }
                        }))
                rest
        let! (thenStmt, rest) = 
            xElementReq 
                (nameGuard ("Then", ns) 
                    (fun stmts -> 
                        xElementsAll stmts 
                        |> xElementReq (nameGuard ("Statements", ns) parseStatements)
                        // Should use ensureEmpty
                        |> Result.map fst))
                rest
        let! (elseStmt, rest) = 
            xElement 
                (nameGuard ("Else", ns) 
                    (fun stmts -> 
                        xElementsAll stmts 
                        |> xElementReq (nameGuard ("Statements", ns) parseStatements)
                        // Should use ensureEmpty
                        |> Result.map fst))
                rest
        do! ensureEmpty rest
        return { 
            BaseInfo = baseInfo
            Condition = condition
            Then = thenStmt
            Else = elseStmt 
        }
    }

and parseStmtBlockType =
    createCaseParser [
         "StmtSimple", parseStmtSimple >> Result.map StmtBlockType.StmtSimple
         "StmtCond", parseStmtCond >> Result.map StmtBlockType.StmtCond
         "StmtCursor", parseStmtCursor >> Result.map StmtBlockType.StmtCursor
         "StmtReceive", parseStmtReceive >> Result.map StmtBlockType.StmtReceive
         "StmtUseDb", parseStmtUseDb >> Result.map StmtBlockType.StmtUseDb
         "ExternalDistributedComputation", parseExternalDistributedComputation >> Result.map StmtBlockType.ExternalDistributedComputation
    ]

and parseStatements (statements : Linq.XElement) : Result<StmtBlockType list, _> =
    ensureOnly (xElementMany parseStmtBlockType) id statements 

let parseBatch (batch : Linq.XElement) : Result<Batch, _> =
    ensureOnly 
        (xElementMany (nameGuard ("Statements", ns) parseStatements))
        (fun stmtBlockTypes -> { Statements = stmtBlockTypes |> List.collect id })
        batch

let parseBatchSequence (batchSequence : Linq.XElement) =
    ensureOnly (xElementMany1 (nameGuard ("Batch", ns) parseBatch)) id batchSequence


