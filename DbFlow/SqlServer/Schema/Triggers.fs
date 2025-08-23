namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-triggers-transact-sql?view=sql-server-ver17

type DatabaseTrigger = {
    //object : OBJECT
    Name : string
    Definition : string
    IsDisabled : bool
    IsInsteadOfTrigger : bool

    MSDescription : string option
}

type Trigger = {
    Object : OBJECT
    Parent : OBJECT
    Name : string
    OrigDefinition : string
    Definition : string
    IsDisabled : bool
    IsInsteadOfTrigger : bool

    MSDescription : string option
}

module Trigger =
    let readAllDatabaseTriggers objects (sql_modules : RCMap<int, SqlModule>) ms_descriptions connection =
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
                        //object = RCMap.pick object_id objects 
                        Name = name
                        Definition = 
                            let sqlModule = RCMap.pick object_id sql_modules
                            sqlModule.Definition |> String.trim 
                        IsDisabled = readBool "is_disabled" r
                        IsInsteadOfTrigger = readBool "is_instead_of_trigger" r

                        MSDescription = RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        
    let readAll' (objects : RCMap<int, OBJECT>) (sql_modules : RCMap<int, SqlModule>) ms_descriptions connection =
        DbTr.reader 
            "SELECT
                 tr.object_id, tr.parent_id, tr.name, tr.is_disabled, tr.is_instead_of_trigger 
             FROM sys.triggers tr
             WHERE parent_class = 1"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                let parent_id = readInt32 "parent_id" r
                let object = RCMap.pick object_id objects
                let parent = RCMap.pick parent_id objects
                let name = readString "name" r
                let trigger_definition = 
                    let sqlModule = RCMap.pick object_id sql_modules
                    sqlModule.Definition |> String.trim
                let updated_trigger_definition = 
                    SqlParser.SqlDefinitions.updateTriggerDefinition 
                        $"[{object.Schema.Name}].[{name}]" $"[{parent.Schema.Name}].[{parent.Name}]"
                        trigger_definition
                {
                        Object = object
                        Parent = parent
                        Name = name
                        OrigDefinition = trigger_definition
                        Definition = updated_trigger_definition
                        IsDisabled = readBool "is_disabled" r
                        IsInsteadOfTrigger = readBool "is_instead_of_trigger" r

                        MSDescription = RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects sql_modules ms_descriptions connection =
        let triggers' = readAll' objects sql_modules ms_descriptions connection
        triggers' 
        |> List.groupBy (fun tr -> tr.Parent.ObjectId)
        |> List.fold 
            (fun m (object_id, trs) -> 
                Map.add object_id (trs |> List.toArray) m)
            Map.empty
        |> RCMap.ofMap
    
