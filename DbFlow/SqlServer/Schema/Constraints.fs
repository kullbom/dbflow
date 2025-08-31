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

    XProperties : Map<string, string>
}

module DefaultConstraint =
    let readAll' objects columns xProperties connection =
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

                    XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, object_id, 0) xProperties
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
        

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-check-constraints-transact-sql?view=sql-server-ver17

type CheckConstraint = {
    Object : OBJECT
    Parent : OBJECT
    ParentColumnId : int
    Column : Column option
    
    IsDisabled : bool
    IsNotForReplication : bool
    IsNotTrusted : bool
    
    Definition : string

    UsesDatabaseCollation : bool
    IsSystemNamed : bool

    XProperties : Map<string, string>
}

module CheckConstraint =
    let readAll' objects columns xProperties connection =
        DbTr.reader 
            "SELECT 
                 cc.object_id, cc.parent_object_id, cc.parent_column_id, cc.is_disabled, cc.is_not_for_replication, cc.is_not_trusted, cc.definition, cc.uses_database_collation, cc.is_system_named
             FROM sys.check_constraints cc"
            []
            (fun acc r -> 
                let objectId = readInt32 "object_id" r
                let parentId = readInt32 "parent_object_id" r
                let parentColumnId = readInt32 "parent_column_id" r
                {
                        Object = RCMap.pick objectId objects
                        Parent = RCMap.pick parentId objects
                        ParentColumnId = parentColumnId
                        Column = 
                            match parentColumnId with
                            | 0 -> None
                            | _ -> RCMap.pick (parentId, parentColumnId) columns |> Some
                        IsDisabled = readBool "is_disabled" r
                        IsNotForReplication = readBool "is_not_for_replication" r
                        IsNotTrusted = readBool "is_not_trusted" r

                        Definition = readString "definition" r

                        UsesDatabaseCollation = readBool "uses_database_collation" r
                        IsSystemNamed = readBool "is_system_named" r

                        XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, objectId, 0) xProperties
                } :: acc)
            []
        |> DbTr.commit_ connection
            

    let readAll objects columns xProperties connection =
        let checkConstraints' = readAll' objects columns xProperties connection
        checkConstraints'
        |> List.groupBy (fun cc -> cc.Parent.ObjectId)
        |> List.fold 
            (fun m (object_id, trs) -> 
                Map.add object_id (trs |> List.toArray) m)
            Map.empty
        |> RCMap.ofMap
        