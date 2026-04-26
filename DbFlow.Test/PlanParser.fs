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

let parseRelOpType (relOp : Linq.XElement) : Result<RelOpType, _> =
    failwith "Not implemented yet"

let parseColumnReferenceType (columnReference : Linq.XElement) : Result<ColumnReferenceType, _> =
    Result.builder {
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

let parseParameterList (parameterList : Linq.XElement) : Result<ColumnReferenceType list, _> =
    Result.builder {
        let! columnReferenceTypes = xElementsAll parameterList |> forAll parseColumnReferenceType
        return columnReferenceTypes
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
        let! columnReferenceType = xElementOptional parseParameterList ("ParameterList", ns) queryPlan
        
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
