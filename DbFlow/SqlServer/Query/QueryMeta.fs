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

type QueryMeta = {
    UndeclaredParameters : QueryParameter array
    FirstResultSet : QueryResultColumn list

    ShowPlanXml : ShowPlanXml.Plan
    PlanIssues : ShowPlanXml.QueryPlanIssue list
}

module QueryMeta =
    
    module Sys =
        let estimatedPlan (sqlQuery : string) : string DbTr =
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
                    "tsql", sqlQuery //(NCHAR,forSql)
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