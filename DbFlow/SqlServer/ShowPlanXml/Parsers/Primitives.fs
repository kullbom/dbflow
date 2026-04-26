module DbFlow.SqlServer.Experimental.ShowPlanXml.Parsers.Primitives

open DbFlow.XmlParser
open DbFlow.SqlServer.Experimental.ShowPlanXml

let parseDateTime (dateTimeStr : string) : Result<System.DateTime, _> =
    match System.DateTime.TryParse dateTimeStr with
    | true, dt -> Ok dt
    | false, _ -> Errorf "Invalid DateTime format: '%s'" dateTimeStr

let createEnumParser (mapping : (string * 'a) list) : (string -> Result<'a, _>) =
    let d = dict mapping
    fun s ->
        match d.TryGetValue s with
        | true, value -> Ok value
        | false, _ -> Errorf "Unknown %s: '%s'" typeof<'a>.Name s

let createCaseParser (mapping : (string * (System.Xml.Linq.XElement -> Result<'r, _>)) list) : (System.Xml.Linq.XElement -> Result<'r, _>) =
    let d = dict mapping
    fun xml ->
        let name = xml.Name.LocalName
        match d.TryGetValue name with
        | true, parser -> parser xml
        | false, _ -> Errorf "Unknown %s case: '%s'" typeof<'r>.Name name


// ========================================
// Enum Parsers - no mutual dependencies
// ========================================

let parseIndexKind =
    createEnumParser
        [ 
            "Heap", IndexKindType.Heap
            "Clustered", IndexKindType.Clustered
            "FTSChangeTracking", IndexKindType.FTSChangeTracking
            "FTSMapping", IndexKindType.FTSMapping
            "NonClustered", IndexKindType.NonClustered
            "PrimaryXML", IndexKindType.PrimaryXML
            "SecondaryXML", IndexKindType.SecondaryXML
            "Spatial", IndexKindType.Spatial
            "ViewClustered", IndexKindType.ViewClustered
            "ViewNonClustered", IndexKindType.ViewNonClustered
            "NonClusteredHash", IndexKindType.NonClusteredHash
            "SelectiveXML", IndexKindType.SelectiveXML
            "SecondarySelectiveXML", IndexKindType.SecondarySelectiveXML 
        ]

let parseCloneAccessScope =
    createEnumParser
        [
            "Primary", CloneAccessScopeType.Primary
            "Secondary", CloneAccessScopeType.Secondary
            "Both", CloneAccessScopeType.Both
            "Either", CloneAccessScopeType.Either
            "ExactMatch", CloneAccessScopeType.ExactMatch
            "Local", CloneAccessScopeType.Local
        ]
    
let parseStorage =
    createEnumParser
        [
            "RowStore", StorageType.RowStore
            "ColumnStore", StorageType.ColumnStore
            "MemoryOptimized", StorageType.MemoryOptimized
        ]

let parseArithmeticOperation =
    createEnumParser
        [
            "ADD", ArithmeticOperationType.ADD
            "BIT_ADD", ArithmeticOperationType.BIT_ADD
            "BIT_AND", ArithmeticOperationType.BIT_AND
            "BIT_COMBINE", ArithmeticOperationType.BIT_COMBINE
            "BIT_NOT", ArithmeticOperationType.BIT_NOT
            "BIT_OR", ArithmeticOperationType.BIT_OR
            "BIT_XOR", ArithmeticOperationType.BIT_XOR
            "DIV", ArithmeticOperationType.DIV
            "HASH", ArithmeticOperationType.HASH
            "MINUS", ArithmeticOperationType.MINUS
            "MOD", ArithmeticOperationType.MOD
            "MULT", ArithmeticOperationType.MULT
            "SUB", ArithmeticOperationType.SUB
            "CONCAT", ArithmeticOperationType.CONCAT
        ]

let parseLogicalOperation =
    createEnumParser
        [
            "AND", LogicalOperationType.AND
            "IMPLIES", LogicalOperationType.IMPLIES
            "IS NOT NULL", LogicalOperationType.IS_NOT_NULL
            "IS NULL", LogicalOperationType.IS_NULL
            "IS", LogicalOperationType.IS
            "IsFalseOrNull", LogicalOperationType.IsFalseOrNull
            "NOT", LogicalOperationType.NOT
            "OR", LogicalOperationType.OR
            "XOR", LogicalOperationType.XOR
        ]

let parseSubqueryOperation =
    createEnumParser
        [
            "EQ ALL", SubqueryOperationType.EQ_ALL
            "EQ ANY", SubqueryOperationType.EQ_ANY
            "EXISTS", SubqueryOperationType.EXISTS
            "GE ALL", SubqueryOperationType.GE_ALL
            "GE ANY", SubqueryOperationType.GE_ANY
            "GT ALL", SubqueryOperationType.GT_ALL
            "GT ANY", SubqueryOperationType.GT_ANY
            "IN", SubqueryOperationType.IN
            "LE ALL", SubqueryOperationType.LE_ALL
            "LE ANY", SubqueryOperationType.LE_ANY
            "LT ALL", SubqueryOperationType.LT_ALL
            "LT ANY", SubqueryOperationType.LT_ANY
            "NE ALL", SubqueryOperationType.NE_ALL
            "NE ANY", SubqueryOperationType.NE_ANY
        ]

let parseCompareOp =
    createEnumParser
        [
            "BINARY IS", CompareOpType.BINARY_IS
            "BOTH NULL", CompareOpType.BOTH_NULL
            "EQ", CompareOpType.EQ
            "GE", CompareOpType.GE
            "GT", CompareOpType.GT
            "IS", CompareOpType.IS
            "IS NOT", CompareOpType.IS_NOT
            "IS NOT NULL", CompareOpType.IS_NOT_NULL
            "IS NULL", CompareOpType.IS_NULL
            "LE", CompareOpType.LE
            "LT", CompareOpType.LT
            "NE", CompareOpType.NE
            "ONE NULL", CompareOpType.ONE_NULL
        ]

let parseLogicalOp =
    createEnumParser
        [    
            "Aggregate", LogicalOpType.Aggregate
            "Anti Diff", LogicalOpType.AntiDiff
            "Assert", LogicalOpType.Assert
            "Async Concat", LogicalOpType.AsyncConcat
            "Batch Hash Table Build", LogicalOpType.BatchHashTableBuild
            "Bitmap Create", LogicalOpType.BitmapCreate
            "Clustered Index Scan", LogicalOpType.ClusteredIndexScan
            "Clustered Index Seek", LogicalOpType.ClusteredIndexSeek
            "Clustered Update", LogicalOpType.ClusteredUpdate
            "Collapse", LogicalOpType.Collapse
            "Compute Scalar", LogicalOpType.ComputeScalar
            "Concatenation", LogicalOpType.Concatenation
            "Constant Scan", LogicalOpType.ConstantScan
            "Constant Table Get", LogicalOpType.ConstantTableGet
            "Cross Join", LogicalOpType.CrossJoin
            "Delete", LogicalOpType.Delete
            "Deleted Scan", LogicalOpType.DeletedScan
            "Distinct Sort", LogicalOpType.DistinctSort
            "Distinct", LogicalOpType.Distinct
            "Distribute Streams", LogicalOpType.DistributeStreams
            "Eager Spool", LogicalOpType.EagerSpool
            "External Extraction Scan", LogicalOpType.ExternalExtractionScan
            "External Select", LogicalOpType.ExternalSelect
            "Filter", LogicalOpType.Filter
            "Flow Distinct", LogicalOpType.FlowDistinct
            "Foreign Key References Check", LogicalOpType.ForeignKeyReferencesCheck
            "Full Outer Join", LogicalOpType.FullOuterJoin
            "Gather Streams", LogicalOpType.GatherStreams
            "GbAgg", LogicalOpType.GbAgg
            "GbApply", LogicalOpType.GbApply
            "Get", LogicalOpType.Get
            "Generic", LogicalOpType.Generic
            "Inner Apply", LogicalOpType.InnerApply
            "Index Scan", LogicalOpType.IndexScan
            "Index Seek", LogicalOpType.IndexSeek
            "Inner Join", LogicalOpType.InnerJoin
            "Insert", LogicalOpType.Insert
            "Inserted Scan", LogicalOpType.InsertedScan
            "Intersect", LogicalOpType.Intersect
            "Intersect All", LogicalOpType.IntersectAll
            "Lazy Spool", LogicalOpType.LazySpool
            "Left Anti Semi Apply", LogicalOpType.LeftAntiSemiApply
            "Left Semi Apply", LogicalOpType.LeftSemiApply
            "Left Outer Apply", LogicalOpType.LeftOuterApply
            "Left Anti Semi Join", LogicalOpType.LeftAntiSemiJoin
            "Left Diff", LogicalOpType.LeftDiff
            "Left Diff All", LogicalOpType.LeftDiffAll
            "Left Outer Join", LogicalOpType.LeftOuterJoin
            "Left Semi Join", LogicalOpType.LeftSemiJoin
            "LocalCube", LogicalOpType.LocalCube
            "Log Row Scan", LogicalOpType.LogRowScan
            "Merge", LogicalOpType.Merge
            "Merge Interval", LogicalOpType.MergeInterval
            "Merge Stats", LogicalOpType.MergeStats
            "Move", LogicalOpType.Move
            "Parameter Table Scan", LogicalOpType.ParameterTableScan
            "Partial Aggregate", LogicalOpType.PartialAggregate
            "Print", LogicalOpType.Print
            "Project", LogicalOpType.Project
            "Put", LogicalOpType.Put
            "Rank", LogicalOpType.Rank
            "Remote Delete", LogicalOpType.RemoteDelete
            "Remote Index Scan", LogicalOpType.RemoteIndexScan
            "Remote Index Seek", LogicalOpType.RemoteIndexSeek
            "Remote Insert", LogicalOpType.RemoteInsert
            "Remote Query", LogicalOpType.RemoteQuery
            "Remote Scan", LogicalOpType.RemoteScan
            "Remote Update", LogicalOpType.RemoteUpdate
            "Repartition Streams", LogicalOpType.RepartitionStreams
            "RID Lookup", LogicalOpType.RIDLookup
            "Right Anti Semi Join", LogicalOpType.RightAntiSemiJoin
            "Right Diff", LogicalOpType.RightDiff
            "Right Diff All", LogicalOpType.RightDiffAll
            "Right Outer Join", LogicalOpType.RightOuterJoin
            "Right Semi Join", LogicalOpType.RightSemiJoin
            "Segment", LogicalOpType.Segment
            "Sequence", LogicalOpType.Sequence
            "Sort", LogicalOpType.Sort
            "Split", LogicalOpType.Split
            "Switch", LogicalOpType.Switch
            "Table-valued function", LogicalOpType.TableValuedFunction
            "Table Scan", LogicalOpType.TableScan
            "Top", LogicalOpType.Top
            "TopN Sort", LogicalOpType.TopNSort
            "UDX", LogicalOpType.UDX
            "Union", LogicalOpType.Union
            "Union All", LogicalOpType.UnionAll
            "Update", LogicalOpType.Update
            "Local Stats", LogicalOpType.LocalStats
            "Window Spool", LogicalOpType.WindowSpool
            "Window Aggregate", LogicalOpType.WindowAggregate
            "Key Lookup", LogicalOpType.KeyLookup
            "Extensible Column Store Scan", LogicalOpType.ExtensibleColumnStoreScan
        ]
    
let parsePhysicalOp =
    createEnumParser
        [
            "Adaptive Join", PhysicalOpType.AdaptiveJoin
            "Apply", PhysicalOpType.Apply
            "Assert", PhysicalOpType.Assert
            "Batch Hash Table Build", PhysicalOpType.BatchHashTableBuild
            "Bitmap", PhysicalOpType.Bitmap
            "Broadcast", PhysicalOpType.Broadcast
            "Clustered Index Delete", PhysicalOpType.ClusteredIndexDelete
            "Clustered Index Insert", PhysicalOpType.ClusteredIndexInsert
            "Clustered Index Scan", PhysicalOpType.ClusteredIndexScan
            "Clustered Index Seek", PhysicalOpType.ClusteredIndexSeek
            "Clustered Index Update", PhysicalOpType.ClusteredIndexUpdate
            "Clustered Index Merge", PhysicalOpType.ClusteredIndexMerge
            "Clustered Update", PhysicalOpType.ClusteredUpdate
            "Collapse", PhysicalOpType.Collapse
            "Columnstore Index Delete", PhysicalOpType.ColumnstoreIndexDelete
            "Columnstore Index Insert", PhysicalOpType.ColumnstoreIndexInsert
            "Columnstore Index Merge", PhysicalOpType.ColumnstoreIndexMerge
            "Columnstore Index Scan", PhysicalOpType.ColumnstoreIndexScan
            "Columnstore Index Update", PhysicalOpType.ColumnstoreIndexUpdate
            "Compute Scalar", PhysicalOpType.ComputeScalar
            "Compute To Control Node", PhysicalOpType.ComputeToControlNode
            "Concatenation", PhysicalOpType.Concatenation
            "Constant Scan", PhysicalOpType.ConstantScan
            "Constant Table Get", PhysicalOpType.ConstantTableGet
            "Control To Compute Nodes", PhysicalOpType.ControlToComputeNodes
            "Delete", PhysicalOpType.Delete
            "Deleted Scan", PhysicalOpType.DeletedScan
            "External Broadcast", PhysicalOpType.ExternalBroadcast
            "External Extraction Scan", PhysicalOpType.ExternalExtractionScan
            "External Local Streaming", PhysicalOpType.ExternalLocalStreaming
            "External Round Robin", PhysicalOpType.ExternalRoundRobin
            "External Select", PhysicalOpType.ExternalSelect
            "External Shuffle", PhysicalOpType.ExternalShuffle
            "Filter", PhysicalOpType.Filter
            "Foreign Key References Check", PhysicalOpType.ForeignKeyReferencesCheck
            "GbAgg", PhysicalOpType.GbAgg
            "GbApply", PhysicalOpType.GbApply
            "Get", PhysicalOpType.Get
            "Generic", PhysicalOpType.Generic
            "Hash Match", PhysicalOpType.HashMatch
            "Index Delete", PhysicalOpType.IndexDelete
            "Index Insert", PhysicalOpType.IndexInsert
            "Index Scan", PhysicalOpType.IndexScan
            "Insert", PhysicalOpType.Insert
            "Join", PhysicalOpType.Join
            "Index Seek", PhysicalOpType.IndexSeek
            "Index Spool", PhysicalOpType.IndexSpool
            "Index Update", PhysicalOpType.IndexUpdate
            "Inserted Scan", PhysicalOpType.InsertedScan
            "Local Cube", PhysicalOpType.LocalCube
            "Log Row Scan", PhysicalOpType.LogRowScan
            "Merge Interval", PhysicalOpType.MergeInterval
            "Merge Join", PhysicalOpType.MergeJoin
            "Nested Loops", PhysicalOpType.NestedLoops
            "Online Index Insert", PhysicalOpType.OnlineIndexInsert
            "Parallelism", PhysicalOpType.Parallelism
            "Parameter Table Scan", PhysicalOpType.ParameterTableScan
            "Print", PhysicalOpType.Print
            "Project", PhysicalOpType.Project
            "Put", PhysicalOpType.Put
            "Rank", PhysicalOpType.Rank
            "Remote Delete", PhysicalOpType.RemoteDelete
            "Remote Index Scan", PhysicalOpType.RemoteIndexScan
            "Remote Index Seek", PhysicalOpType.RemoteIndexSeek
            "Remote Insert", PhysicalOpType.RemoteInsert
            "Remote Query", PhysicalOpType.RemoteQuery
            "Remote Scan", PhysicalOpType.RemoteScan
            "Remote Update", PhysicalOpType.RemoteUpdate
            "RID Lookup", PhysicalOpType.RIDLookup
            "Row Count Spool", PhysicalOpType.RowCountSpool
            "Segment", PhysicalOpType.Segment
            "Sequence", PhysicalOpType.Sequence
            "Sequence Project", PhysicalOpType.SequenceProject
            "Shuffle", PhysicalOpType.Shuffle
            "Single Source Round Robin Move", PhysicalOpType.SingleSourceRoundRobinMove
            "Sort", PhysicalOpType.Sort
            "Split", PhysicalOpType.Split
            "Stream Aggregate", PhysicalOpType.StreamAggregate
            "Switch", PhysicalOpType.Switch
            "Table Delete", PhysicalOpType.TableDelete
            "Table Insert", PhysicalOpType.TableInsert
            "Table Merge", PhysicalOpType.TableMerge
            "Table Scan", PhysicalOpType.TableScan
            "Table Spool", PhysicalOpType.TableSpool
            "Table Update", PhysicalOpType.TableUpdate
            "Table-valued function", PhysicalOpType.TableValuedFunction
            "Top", PhysicalOpType.Top
            "Trim", PhysicalOpType.Trim
            "UDX", PhysicalOpType.UDX
            "Union", PhysicalOpType.Union
            "Union All", PhysicalOpType.UnionAll
            "Window Aggregate", PhysicalOpType.WindowAggregate
            "Window Spool", PhysicalOpType.WindowSpool
            "Key Lookup", PhysicalOpType.KeyLookup
            "Extensible Column Store Scan", PhysicalOpType.ExtensibleColumnStoreScan
        ]

let parseExecutionMode =
    createEnumParser
        [
            "Row", ExecutionModeType.Row
            "Batch", ExecutionModeType.Batch
        ]

let parsePartitioningType =
    createEnumParser
        [
            "Broadcast", PartitionType.Broadcast
            "Demand", PartitionType.Demand
            "Hash", PartitionType.Hash
            "NoPartitioning", PartitionType.NoPartitioning
            "Range", PartitionType.Range
            "RoundRobin", PartitionType.RoundRobin
            "CloneLocation", PartitionType.CloneLocation
        ]