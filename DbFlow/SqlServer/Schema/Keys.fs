namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-foreign-key-columns-transact-sql?view=sql-server-ver17

type FOREIGN_KEY_COLUMN = {
    constraint_object : OBJECT
    constraint_column_id : int 

    parent_column : COLUMN
    referenced_column : COLUMN
}

module FOREIGN_KEY_COLUMN =
    let readAll' objects columns connection =
        DbTr.reader 
            "SELECT constraint_object_id, constraint_column_id, parent_object_id, parent_column_id, referenced_object_id, referenced_column_id
             FROM sys.foreign_key_columns"
            []
            (fun acc r -> 
                let constraint_object_id = readInt32 "constraint_object_id" r
                let constraint_column_id = readInt32 "constraint_column_id" r
                let parent_object_id = readInt32 "parent_object_id" r
                let parent_column_id = readInt32 "parent_column_id" r
                let referenced_object_id = readInt32 "referenced_object_id" r
                let referenced_column_id = readInt32 "referenced_column_id" r
                {
                    constraint_object = RCMap.pick constraint_object_id objects
                    constraint_column_id = constraint_column_id
                    
                    parent_column = RCMap.pick (parent_object_id, parent_column_id) columns
                    referenced_column = RCMap.pick (referenced_object_id, referenced_column_id) columns
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll foreignKeys columns connection =
        let fkColumns' = readAll' foreignKeys columns connection
        let fkColumnsByConstraint =
            fkColumns'
            |> List.groupBy (fun c -> c.constraint_object.ObjectId)
            |> List.map (fun (object_id, cs) -> object_id, cs |> List.sortBy (fun c -> c.constraint_column_id) |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        fkColumnsByConstraint
        

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-foreign-keys-transact-sql?view=sql-server-ver17

type REFERENTIAL_ACTION =
    | No_action   // 0
    | Cascade     // 1
    | Set_null    // 2
    | Set_default // 3

type FOREIGN_KEY = {
    name : string
    object : OBJECT
    parent : OBJECT
    referenced : OBJECT
    key_index_id : int
    is_disabled : bool
    IsSystemNamed : bool

    delete_referential_action : REFERENTIAL_ACTION
    update_referential_action : REFERENTIAL_ACTION

    columns : FOREIGN_KEY_COLUMN array

    ms_description : string option
}

module FOREIGN_KEY =
    let toREFERENTIAL_ACTION b =
        match b with
        | 0uy -> REFERENTIAL_ACTION.No_action
        | 1uy -> REFERENTIAL_ACTION.Cascade
        | 2uy -> REFERENTIAL_ACTION.Set_null
        | 3uy -> REFERENTIAL_ACTION.Set_default
        | _ -> failwithf "Unknown REFERENTIAL_ACTION: %i" b
    
    let readAll' objects fkColumns ms_descriptions connection =
        DbTr.reader 
            "SELECT 
                fk.name, fk.object_id, fk.parent_object_id, fk.referenced_object_id, fk.is_disabled, fk.is_system_named, fk.key_index_id, 
                fk.delete_referential_action, fk.update_referential_action
             FROM sys.foreign_keys fk"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                {
                    name = readString "name" r
                    object = RCMap.pick object_id objects
                    parent = RCMap.pick (readInt32 "parent_object_id" r) objects
                    referenced = RCMap.pick (readInt32 "referenced_object_id" r) objects
                    key_index_id = readInt32 "key_index_id" r
                    is_disabled = readBool "is_disabled" r
                    IsSystemNamed = readBool "is_system_named" r

                    delete_referential_action = toREFERENTIAL_ACTION (readByte "delete_referential_action" r)
                    update_referential_action = toREFERENTIAL_ACTION (readByte "update_referential_action" r)

                    columns = match RCMap.tryPick object_id fkColumns with Some cs -> cs | None -> [||]

                    ms_description = RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects fkColumns ms_descriptions connection =
        let foreignKeys' = readAll' objects fkColumns ms_descriptions connection
        let foreignKeysByParent =
            foreignKeys'
            |> List.groupBy (fun fk -> fk.parent.ObjectId)
            |> List.map (fun (parent_id, fks) -> parent_id, fks |> List.sortBy (fun fk -> fk.key_index_id) |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        let foreignKeysByReferenced =
            foreignKeys'
            |> List.groupBy (fun fk -> fk.referenced.ObjectId)
            |> List.map (fun (referenced_id, fks) -> referenced_id, fks |> List.sortBy (fun fk -> fk.name) |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        foreignKeysByParent, foreignKeysByReferenced
