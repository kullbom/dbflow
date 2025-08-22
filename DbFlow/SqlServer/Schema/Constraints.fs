namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-default-constraints-transact-sql?view=sql-server-ver17

type DefaultConstraint = {
    Object : OBJECT
    Parent : OBJECT
    Column : Column

    IsSystemNamed : bool
    
    Definition : string

    MSDescription : string option
}

module DefaultConstraint =
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
                    Object = RCMap.pick object_id objects
                    Parent = RCMap.pick parent_object_id objects
                    Column = RCMap.pick (parent_object_id, parent_column_id) columns

                    IsSystemNamed = readBool "is_system_named" r
                    
                    Definition = readString "definition" r

                    MSDescription = RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll objects columns ms_descriptions connection =
        let defaultConstraints' = readAll' objects columns ms_descriptions connection
        defaultConstraints'
        |> List.groupBy (fun cc -> cc.Parent.ObjectId)
        |> List.fold 
            (fun m (object_id, trs) -> 
                Map.add object_id (trs |> List.toArray) m)
            Map.empty
        |> RCMap.ofMap
        

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-key-constraints-transact-sql?view=sql-server-ver17

type KeyConstraint = {
    Object : OBJECT
    
    IsSystemNamed : bool
    UniqueIndexId : int
    
    MSDescription : string option
}

module KeyConstraint =
    let readAll objects ms_descriptions connection =
        DbTr.reader
            "SELECT kc.object_id, kc.unique_index_id, kc.is_system_named FROM sys.key_constraints kc" 
            []
            (fun m r ->
                let object_id = readInt32 "object_id" r
                Map.add
                    object_id
                    {
                        Object = RCMap.pick object_id objects

                        IsSystemNamed = readBool "is_system_named" r
                        UniqueIndexId = readInt32 "unique_index_id" r
                        
                        MSDescription = RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, 0) ms_descriptions
                    }
                    m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-check-constraints-transact-sql?view=sql-server-ver17

type CheckConstraint = {
    object : OBJECT
    parent : OBJECT
    parent_column_id : int
    column : Column option
    
    is_disabled : bool
    is_not_for_replication : bool
    is_not_trusted : bool
    
    definition : string

    uses_database_collation : bool
    IsSystemNamed : bool

    ms_description : string option
}

module CheckConstraint =
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
                        object = RCMap.pick object_id objects
                        parent = RCMap.pick parent_id objects
                        parent_column_id = parent_column_id
                        column = 
                            match parent_column_id with
                            | 0 -> None
                            | column_id -> RCMap.pick (parent_id, column_id) columns |> Some
                        is_disabled = readBool "is_disabled" r
                        is_not_for_replication = readBool "is_not_for_replication" r
                        is_not_trusted = readBool "is_not_trusted" r

                        definition = readString "definition" r

                        uses_database_collation = readBool "uses_database_collation" r
                        IsSystemNamed = readBool "is_system_named" r

                        ms_description = RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects columns ms_descriptions connection =
        let checkConstraints' = readAll' objects columns ms_descriptions connection
        checkConstraints'
        |> List.groupBy (fun cc -> cc.parent.ObjectId)
        |> List.fold 
            (fun m (object_id, trs) -> 
                Map.add object_id (trs |> List.toArray) m)
            Map.empty
        |> RCMap.ofMap
        