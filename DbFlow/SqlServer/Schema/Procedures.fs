namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

type Parameter = {
    Object : OBJECT
    Name : string
    ParameterId : int

    Datatype : Datatype

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
    MSDescription : string option
}

module Parameter = 
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
                    Object = RCMap.pick object_id objects
                    Name = readString "name" r
                    ParameterId = parameter_id

                    Datatype = Datatype.readType types None r

                    MSDescription = RCMap.tryPick (XPropertyClass.Parameter, object_id, parameter_id) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll objects types ms_descriptions connection =
        let parameters' = readAll' objects types ms_descriptions connection
        parameters'
        |> List.groupBy (fun c -> c.Object.ObjectId)
        |> List.fold 
            (fun m (object_id, cs) -> 
                Map.add object_id (cs |> List.sortBy (fun c -> c.ParameterId) |> List.toArray) m)
            Map.empty
        |> RCMap.ofMap
        
type Procedure = {
    Object : OBJECT
    Name : string

    Parameters : Parameter array
    Columns : Column array // Should only exist for SQL_TABLE_VALUED_FUNCTION
    
    OrigDefinition : string
    Definition : string

    Indexes : Index array

    MSDescription : string option
} 

module Procedure =
    let readAll (objects : RCMap<_,OBJECT>) parameters columns indexes (sql_modules : RCMap<int, SqlModule>) ms_descriptions _connection =
        objects
        |> RCMap.fold 
            (fun acc _ _ o ->
                let procedureDefiningToken =
                    match o.ObjectType with
                    | ObjectType.SqlScalarFunction 
                    | ObjectType.SqlInlineTableValuedFunction
                    | ObjectType.SqlTableValuedFunction  -> Some "FUNCTION"
                    | ObjectType.SqlStoredProcedure -> Some "PROCEDURE"
                    | _ -> None
                match procedureDefiningToken with
                | Some definingToken ->
                    let objectId = o.ObjectId
                    let object = RCMap.pick objectId objects // to increase the ref count
                    let origDefinition = (RCMap.pick objectId sql_modules).Definition.Trim()
                    {
                        Object = object
                        Name = o.Name

                        Parameters = match RCMap.tryPick objectId parameters with Some ps -> ps | None -> [||]
                        Columns = match RCMap.tryPick objectId columns with Some cs -> cs | None -> [||]
                        OrigDefinition = origDefinition
                        Definition = 
                            SqlParser.SqlDefinitions.updateProcedureDefinition 
                                $"[{object.Schema.Name}].[{object.Name}]" 
                                definingToken origDefinition

                        Indexes = match RCMap.tryPick objectId indexes with Some is -> is | None -> [||]

                        MSDescription = RCMap.tryPick (XPropertyClass.ObjectOrColumn, objectId, 0) ms_descriptions
                    } :: acc
                | None -> acc)
            []