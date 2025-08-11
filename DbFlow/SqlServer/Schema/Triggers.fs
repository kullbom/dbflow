namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-triggers-transact-sql?view=sql-server-ver17

type DATABASE_TRIGGER = {
    //object : OBJECT
    trigger_name : string
    definition : string
    is_disabled : bool
    is_instead_of_trigger : bool

    ms_description : string option
}

type TRIGGER = {
    object : OBJECT
    parent : OBJECT
    trigger_name : string
    orig_definition : string
    definition : string
    is_disabled : bool
    is_instead_of_trigger : bool

    ms_description : string option
}

module TRIGGER =
    let readAllDatabaseTriggers objects (sql_modules : PickMap<int, SQL_MODULE>) ms_descriptions connection =
        DbTr.reader 
            "SELECT
                 tr.object_id, tr.name, tr.is_disabled, tr.is_instead_of_trigger 
             FROM sys.triggers tr
             WHERE parent_class = 0"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                let name = readString "name" r
                {
                        //object = PickMap.pick object_id objects 
                        trigger_name = name
                        definition = (PickMap.pick object_id sql_modules).definition.Trim()
                        is_disabled = readBool "is_disabled" r
                        is_instead_of_trigger = readBool "is_instead_of_trigger" r

                        ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        
    let readAll' (objects : PickMap<int, OBJECT>) (sql_modules : PickMap<int, SQL_MODULE>) ms_descriptions connection =
        DbTr.reader 
            "SELECT
                 tr.object_id, tr.parent_id, tr.name, tr.is_disabled, tr.is_instead_of_trigger 
             FROM sys.triggers tr
             WHERE parent_class = 1"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                let parent_id = readInt32 "parent_id" r
                let object = PickMap.pick object_id objects
                let parent = PickMap.pick parent_id objects
                let name = readString "name" r
                let trigger_definition = (PickMap.pick object_id sql_modules).definition.Trim()
                let updated_trigger_definition = 
                    SqlParser.SqlDefinitions.updateTriggerDefinition 
                        $"[{object.schema.name}].[{name}]" $"[{parent.schema.name}].[{parent.name}]"
                        trigger_definition
                let foo = 0
                {
                        object = object
                        parent = parent
                        trigger_name = name
                        orig_definition = trigger_definition
                        definition = updated_trigger_definition
                        is_disabled = readBool "is_disabled" r
                        is_instead_of_trigger = readBool "is_instead_of_trigger" r

                        ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects sql_modules ms_descriptions connection =
        let triggers' = readAll' objects sql_modules ms_descriptions connection
        triggers' 
        |> List.groupBy (fun tr -> tr.parent.object_id)
        |> List.fold 
            (fun m (object_id, trs) -> 
                Map.add object_id (trs |> List.toArray) m)
            Map.empty
        |> PickMap.ofMap
    
