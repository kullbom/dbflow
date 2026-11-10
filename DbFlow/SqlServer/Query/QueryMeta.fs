namespace DbFlow.SqlServer.Experimental

open DbFlow
open DbFlow.DbTr
open DbFlow.Readers
open DbFlow.SqlServer.Schema

type QueryParameter = {
    Name : string
    Ordinal : int
    Datatype : Datatype
}

type QueryResultColumn = {
    Name : string
    Ordinal : int
    Datatype : Datatype

    SourceServer : string option
    SourceDatabase : string option
    SourceSchema : string option
    SourceTable : string option
    SourceColumn : string option
}

type PlanExecutionStep = {
    // https://learn.microsoft.com/en-us/sql/t-sql/statements/set-showplan-all-transact-sql?view=sql-server-ver17

    // For rows that are not of type PLAN_ROW, this column contains the text of the 
    // Transact-SQL statement. For rows of type PLAN_ROW, this column contains a description 
    // of the operation. This column contains the physical operator and may optionally also contain 
    // the logical operator. This column may also be followed by a description that is determined by 
    // the physical operator. For more information, see Showplan Logical and Physical Operators 
    // Reference.
    StmtText : string
    // Number of the statement in the current batch.
    StmtId : int
    // ID of the node in the current query.
    NodeId : int
    // Node ID of the parent step.
    Parent : int
    // Physical implementation algorithm for the node. For rows of type PLAN_ROWS only.
    PhysicalOp : string
    // Relational algebraic operator this node represents. For rows of type PLAN_ROWS only.
    LogicalOp : string
    // Provides supplemental information about the operation being performed. The contents of 
    // this column depend on the physical operator.
    Argument : string
    // Contains a comma-separated list of values introduced by this operator. These values may 
    // be computed expressions which were present in the current query (for example, in the SELECT 
    // list or WHERE clause), or internal values introduced by the query processor in order to 
    // process this query. These defined values may then be referenced elsewhere within this query. 
    // For rows of type PLAN_ROWS only.
    DefinedValues : string list
    // Estimated number of rows of output produced by this operator. For rows of type PLAN_ROWS only.
    EstimateRows : single
    // Estimated I/O cost* for this operator. For rows of type PLAN_ROWS only.
    EstimateIO : single
    // Estimated CPU cost* for this operator. For rows of type PLAN_ROWS only.
    EstimateCPU : single 
    // Estimated average row size (in bytes) of the row being passed through this operator.
    AvgRowSize : int
    // Estimated (cumulative) cost* of this operation and all child operations.
    TotalSubtreeCost : single
    // Contains a comma-separated list of columns being projected by the current operation.
    OutputList : string list
    // Contains a comma-separated list of warning messages relating to the current operation. 
    // Warning messages may include the string "NO STATS:()" with a list of columns. This warning 
    // message means that the query optimizer attempted to make a decision based on the statistics 
    // for this column, but none were available. Consequently, the query optimizer had to make a 
    // guess, which may have resulted in the selection of an inefficient query plan. For more information 
    // about creating or updating column statistics (which help the query optimizer choose a more 
    // efficient query plan), see UPDATE STATISTICS. This column may optionally include the string 
    // "MISSING JOIN PREDICATE", which means that a join (involving tables) is taking place without a 
    // join predicate. Accidentally dropping a join predicate may result in a query which takes much 
    // longer to run than expected, and returns a huge result set. If this warning is present, verify 
    // that the absence of a join predicate is intentional.
    Warnings : string list
    // Node type. For the parent node of each query, this is the Transact-SQL statement type (for 
    // example, SELECT, INSERT, EXECUTE, and so on). For subnodes representing execution plans, the 
    // type is PLAN_ROW.
    // Type : string
    // 0 = Operator is not running in parallel.
    // 1 = Operator is running in parallel.
    Parallel : bool
    // Estimated number of times this operator will be executed while running the current query.
    EstimateExecutions : single
}

type QueryMeta = {
    UndeclaredParameters : QueryParameter array
    FirstResultSet : QueryResultColumn list

    ShowPlanXml : ShowPlanXml.Plan
    ShowPlanText : string list
    ShowPlanFull : PlanExecutionStep list
    PlanIssues : ShowPlanXml.QueryPlanIssue list
}

module QueryMeta =
    
    module Sys =
        let readCommaSeparated' (columnName : string) (r : System.Data.IDataReader) : string list =
            match nullable columnName readString r with
            | Some s -> s.Split(',') |> Array.map (fun s -> s.Trim()) |> Array.toList
            | None -> []

        let readExecutionStep' acc (r : System.Data.IDataReader) : PlanExecutionStep list =
            if readString "Type" r = "PLAN_ROW"
            then 
                {
                    StmtText = readString "StmtText" r
                    StmtId = readInt32 "StmtId" r
                    NodeId = readInt32 "NodeId" r
                    Parent = readInt32 "Parent" r
                    PhysicalOp = readString "PhysicalOp" r
                    LogicalOp = readString "LogicalOp" r
                    Argument = readString "Argument" r
                    DefinedValues = readCommaSeparated' "DefinedValues" r
                    EstimateRows = readFloat32 "EstimateRows" r
                    EstimateIO = readFloat32 "EstimateIO" r
                    EstimateCPU = readFloat32 "EstimateCPU" r
                    AvgRowSize = readInt32 "AvgRowSize" r
                    TotalSubtreeCost = readFloat32 "TotalSubtreeCost" r
                    OutputList = readCommaSeparated' "OutputList" r
                    Warnings = readCommaSeparated' "Warnings" r
                    Parallel = readBool "Parallel" r
                    EstimateExecutions = readFloat32 "EstimateExecutions" r
                } :: acc
            else
                acc
        
        let estimatedPlan (sqlQuery : string) : (PlanExecutionStep list) DbTr =
            DbTr.nonQuery "SET SHOWPLAN_ALL ON" []
            |> DbTr.bind
                (fun () -> DbTr.reader sqlQuery [] readExecutionStep' [])
            |> DbTr.bind
                (fun r -> DbTr.nonQuery "SET SHOWPLAN_ALL OFF" [] |> DbTr.map (fun () -> List.rev r))

        let estimatedTextPlan (sqlQuery : string) : (string list) DbTr =
            DbTr.nonQuery "SET SHOWPLAN_TEXT ON" []
            |> DbTr.bind
                (fun () -> 
                    DbTr.readerMulti sqlQuery [] (fun i acc r -> if i = 1 then r.GetString(0) :: acc else acc) [])
            |> DbTr.bind
                (fun r -> DbTr.nonQuery "SET SHOWPLAN_TEXT OFF" [] |> DbTr.map (fun () -> List.rev r))
                
        let estimatedXmlPlan (sqlQuery : string) : string DbTr =
            DbTr.nonQuery "SET SHOWPLAN_XML ON" []
            |> DbTr.bind
                (fun () -> 
                    DbTr.reader sqlQuery [] (fun acc r -> r.GetString(0) :: acc) []
                    |> DbTr.map (function [qPlan] -> qPlan | _ -> failwithf "Unexpected result from SHOWPLAN_XML"))
            |> DbTr.bind
                (fun r -> DbTr.nonQuery "SET SHOWPLAN_XML OFF" [] |> DbTr.map (fun () -> r))

        
        
// Documentation:
// Design: https://dataedo.com/samples/html2/AdventureWorks/#/
// https://medium.com/ai-dev-tips/mermaid-markdown-to-create-er-diagrams-from-a-db-schema-ca109d4db140
// https://kroki.io/
// https://mermaid.js.org/config/schema-docs/config.html#defaultrenderer

        let describe_first_result_set dbTypes (sqlQuery : string) : QueryResultColumn list DbTr =
            readerSP 
                "sys.sp_describe_first_result_set" 
                [
                    "tsql", sqlQuery //(NCHAR,forSql)
                    "params", null // (NCHAR, null); 
                    "browse_information_mode", 1uy 
                ]
                (fun acc r -> 
                    if Readers.readBool "is_hidden" r
                    then acc
                    else 
                        let dataType =
                            let userTypeId = 
                                match nullable "user_type_id" readInt32 r with
                                | Some id -> id
                                | None -> Readers.readInt32 "system_type_id" r
                            { Map.find userTypeId dbTypes with
                                Parameter = {
                                    MaxLength = readInt16 "max_length" r
                                    Precision = readByte "precision" r
                                    Scale = readByte "scale" r
                                    CollationName = nullable "collation_name" readString r
                                    IsNullable = readBool "is_nullable" r
                                }
                            }
                        {
                            Name = readString "name" r    
                            Ordinal = readInt32 "column_ordinal" r         
                            
                            Datatype = dataType
                            
                            SourceServer = nullable "source_server" readString r
                            SourceDatabase = nullable "source_database" readString r
                            SourceSchema = nullable "source_schema" readString r
                            SourceTable = nullable "source_table" readString r
                            SourceColumn = nullable "source_column" readString r
                        } :: acc    
                )
                []

        let describe_undeclared_parameters dbTypes (sqlQuery : string) : QueryParameter list DbTr =
            readerSP 
                "sys.sp_describe_undeclared_parameters" 
                [
                    ("tsql", sqlQuery) //(NCHAR,forSql)
                ]
                (fun acc r -> 
                    let dataType =
                        let userTypeId = readInt32 "suggested_user_type_id" r
                        { Map.find userTypeId dbTypes with
                            Parameter = {
                                MaxLength = readInt16 "suggested_max_length" r
                                Precision = readByte "suggested_precision" r
                                Scale = readByte "suggested_scale" r
                                CollationName = None //nullable "collation_name" readString r // ???
                                IsNullable = true //readBool "is_nullable" r
                            }
                        }
                    {
                        Name = readString "name" r    
                        Ordinal = readInt32 "parameter_ordinal" r          
                        
                        Datatype = dataType
                        // HasDefaultValue = false
                    } :: acc
                )
                []