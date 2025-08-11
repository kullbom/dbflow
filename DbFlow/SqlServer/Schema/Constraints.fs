namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-default-constraints-transact-sql?view=sql-server-ver17

type DEFAULT_CONSTRAINT = {
    object : OBJECT
    parent : OBJECT
    column : COLUMN

    is_system_named : bool
    
    definition : string

    ms_description : string option
}

module DEFAULT_CONSTRAINT =
    let readAll' objects columns ms_descriptions connection =
        DbTr.reader 
            "SELECT dc.object_id, dc.parent_object_id, dc.parent_column_id, dc.definition,dc.is_system_named 
             FROM sys.default_constraints dc"
            []
            (fun acc r ->
                let object_id = readInt32 "object_id" r
                let parent_object_id = readInt32 "parent_object_id" r
                let parent_column_id = readInt32 "parent_column_id" r
                {
                    object = PickMap.pick object_id objects
                    parent = PickMap.pick parent_object_id objects
                    column = PickMap.pick (parent_object_id, parent_column_id) columns

                    is_system_named = readBool "is_system_named" r
                    
                    definition = readString "definition" r

                    ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll objects columns ms_descriptions connection =
        let defaultConstraints' = readAll' objects columns ms_descriptions connection
        defaultConstraints'
        |> List.groupBy (fun cc -> cc.parent.object_id)
        |> List.fold 
            (fun m (object_id, trs) -> 
                Map.add object_id (trs |> List.toArray) m)
            Map.empty
        |> PickMap.ofMap
        

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-key-constraints-transact-sql?view=sql-server-ver17

type KEY_CONSTRAINT = {
    object : OBJECT
    
    is_system_named : bool
    unique_index_id : int
    
    ms_description : string option
}

module KEY_CONSTRAINT =
    let readAll objects ms_descriptions connection =
        DbTr.reader
            "SELECT kc.object_id, kc.unique_index_id, kc.is_system_named FROM sys.key_constraints kc" 
            []
            (fun m r ->
                let object_id = readInt32 "object_id" r
                Map.add
                    object_id
                    {
                        object = PickMap.pick object_id objects

                        is_system_named = readBool "is_system_named" r
                        unique_index_id = readInt32 "unique_index_id" r
                        
                        ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                    }
                    m)
            Map.empty
        |> DbTr.commit_ connection
        |> PickMap.ofMap


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-check-constraints-transact-sql?view=sql-server-ver17

type CHECK_CONSTRAINT = {
    object : OBJECT
    parent : OBJECT
    parent_column_id : int
    column : COLUMN option
    
    is_disabled : bool
    is_not_for_replication : bool
    is_not_trusted : bool
    
    definition : string

    uses_database_collation : bool
    is_system_named : bool

    ms_description : string option
}

module CHECK_CONSTRAINT =
    let readAll' objects columns ms_descriptions connection =
        DbTr.reader 
            "SELECT 
                 cc.object_id, cc.parent_object_id, cc.parent_column_id, cc.is_disabled, cc.is_not_for_replication, cc.is_not_trusted, cc.definition, cc.uses_database_collation, cc.is_system_named
             FROM sys.check_constraints cc"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                let parent_id = readInt32 "parent_object_id" r
                let parent_column_id = readInt32 "parent_column_id" r
                {
                        object = PickMap.pick object_id objects
                        parent = PickMap.pick parent_id objects
                        parent_column_id = parent_column_id
                        column = 
                            match parent_column_id with
                            | 0 -> None
                            | column_id -> PickMap.pick (parent_id, column_id) columns |> Some
                        is_disabled = readBool "is_disabled" r
                        is_not_for_replication = readBool "is_not_for_replication" r
                        is_not_trusted = readBool "is_not_trusted" r

                        definition = readString "definition" r

                        uses_database_collation = readBool "uses_database_collation" r
                        is_system_named = readBool "is_system_named" r

                        ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects columns ms_descriptions connection =
        let checkConstraints' = readAll' objects columns ms_descriptions connection
        checkConstraints'
        |> List.groupBy (fun cc -> cc.parent.object_id)
        |> List.fold 
            (fun m (object_id, trs) -> 
                Map.add object_id (trs |> List.toArray) m)
            Map.empty
        |> PickMap.ofMap
        