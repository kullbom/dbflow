namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

(*
object_id
name
parameter_id
system_type_id
user_type_id
max_length
precision
scale

*)
type PARAMETER = {
    object : OBJECT
    name : string
    parameter_id : int

    data_type : DATATYPE

    // is_output
    // is_cursor_ref
    // has_default_value
    // is_xml_document
    // default_value
    // xml_collection_id
    // is_readonly
    // is_nullable
    // encryption_type
    // encryption_type_desc
    // encryption_algorithm_name
    // column_encryption_key_id
    // column_encryption_key_database_name
    ms_description : string option
}

module PARAMETER = 
    let readAll' objects types ms_descriptions connection =
        DbTr.reader
            "SELECT 
                p.object_id, p.name, p.parameter_id,
                p.system_type_id, p.user_type_id, p.max_length, p.precision, p.scale, p.is_nullable
             FROM sys.parameters p"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                let parameter_id = readInt32 "parameter_id" r
                {
                    object = PickMap.pick object_id objects
                    name = readString "name" r
                    parameter_id = parameter_id

                    data_type = DATATYPE.readType types None r

                    ms_description = PickMap.tryPick (XPROPERTY_CLASS.PARAMETER, object_id, parameter_id) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll objects types ms_descriptions connection =
        let parameters' = readAll' objects types ms_descriptions connection
        parameters'
        |> List.groupBy (fun c -> c.object.object_id)
        |> List.fold 
            (fun m (object_id, cs) -> 
                Map.add object_id (cs |> List.sortBy (fun c -> c.parameter_id) |> List.toArray) m)
            Map.empty
        |> PickMap.ofMap
        
type PROCEDURE = {
    object : OBJECT
    name : string

    parameters : PARAMETER array
    columns : COLUMN array // Should only exist for SQL_TABLE_VALUED_FUNCTION
    
    orig_definition : string
    definition : string

    indexes : INDEX array

    ms_description : string option
} 

module PROCEDURE =
    let readAll (objects : PickMap<_,OBJECT>) parameters columns indexes (sql_modules : PickMap<int, SQL_MODULE>) ms_descriptions _connection =
        objects
        |> PickMap.fold 
            (fun acc _ _ o ->
                let procedureDefiningToken =
                    match o.object_type with
                    | OBJECT_TYPE.SQL_SCALAR_FUNCTION 
                    | OBJECT_TYPE.SQL_INLINE_TABLE_VALUED_FUNCTION
                    | OBJECT_TYPE.SQL_TABLE_VALUED_FUNCTION  -> Some "FUNCTION"
                    | OBJECT_TYPE.SQL_STORED_PROCEDURE -> Some "PROCEDURE"
                    | _ -> None
                match procedureDefiningToken with
                | Some definingToken ->
                    let object_id = o.object_id
                    let object = PickMap.pick object_id objects // to increase the ref count
                    let orig_definition = (PickMap.pick object_id sql_modules).definition.Trim()
                    {
                        object = object
                        name = o.name

                        parameters = match PickMap.tryPick object_id parameters with Some ps -> ps | None -> [||]
                        columns = match PickMap.tryPick object_id columns with Some cs -> cs | None -> [||]
                        orig_definition = orig_definition
                        definition = 
                            SqlParser.SqlDefinitions.updateProcedureDefinition 
                                $"[{object.schema.name}].[{object.name}]" 
                                definingToken orig_definition

                        indexes = match PickMap.tryPick object_id indexes with Some is -> is | None -> [||]

                        ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                    } :: acc
                | None -> acc)
            []