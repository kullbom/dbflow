namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-foreign-key-columns-transact-sql?view=sql-server-ver17

type ForeignKeycolumn = {
    ConstraintObject : OBJECT
    ConstraintColumnId : int 

    ParentColumn : Column
    ReferencedColumn : Column
}

module ForeignKeycolumn =
    let readAll' objects columns connection =
        DbTr.reader 
            "SELECT constraint_object_id, constraint_column_id, parent_object_id, parent_column_id, referenced_object_id, referenced_column_id
             FROM sys.foreign_key_columns"
            []
            (fun acc r -> 
                let constraintObjectId = readInt32 "constraint_object_id" r
                let constraintColumnId = readInt32 "constraint_column_id" r
                let parentObjectId = readInt32 "parent_object_id" r
                let parentColumnId = readInt32 "parent_column_id" r
                let referencedObjectId = readInt32 "referenced_object_id" r
                let referencedColumnId = readInt32 "referenced_column_id" r
                {
                    ConstraintObject = RCMap.pick constraintObjectId objects
                    ConstraintColumnId = constraintColumnId
                    
                    ParentColumn = RCMap.pick (parentObjectId, parentColumnId) columns
                    ReferencedColumn = RCMap.pick (referencedObjectId, referencedColumnId) columns
                } :: acc)
            []
        |> DbTr.commit_ connection

    let readAll foreignKeys columns connection =
        let fkColumns' = readAll' foreignKeys columns connection
        let fkColumnsByConstraint =
            fkColumns'
            |> List.groupBy (fun c -> c.ConstraintObject.ObjectId)
            |> List.map (fun (object_id, cs) -> object_id, cs |> List.sortBy (fun c -> c.ConstraintColumnId) |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        fkColumnsByConstraint
        

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-foreign-keys-transact-sql?view=sql-server-ver17

type ReferentialAction =
    | NoAction   // 0
    | Cascade     // 1
    | SetNull    // 2
    | SetDefault // 3

type ForeignKey = {
    Name : string
    Object : OBJECT
    Parent : OBJECT
    Referenced : OBJECT
    KeyIndexId : int
    IsDisabled : bool
    IsSystemNamed : bool

    DeleteReferentialAction : ReferentialAction
    UpdateReferentialAction : ReferentialAction

    Columns : ForeignKeycolumn array

    XProperties : Map<string, string>
}

module ForeignKey =
    let toReferentialAction b =
        match b with
        | 0uy -> ReferentialAction.NoAction
        | 1uy -> ReferentialAction.Cascade
        | 2uy -> ReferentialAction.SetNull
        | 3uy -> ReferentialAction.SetDefault
        | _ -> failwithf "Unknown REFERENTIAL_ACTION: %i" b
    
    let readAll' objects fkColumns xProperties connection =
        DbTr.reader 
            "SELECT 
                fk.name, fk.object_id, fk.parent_object_id, fk.referenced_object_id, fk.is_disabled, fk.is_system_named, fk.key_index_id, 
                fk.delete_referential_action, fk.update_referential_action
             FROM sys.foreign_keys fk"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                {
                    Name = readString "name" r
                    Object = RCMap.pick object_id objects
                    Parent = RCMap.pick (readInt32 "parent_object_id" r) objects
                    Referenced = RCMap.pick (readInt32 "referenced_object_id" r) objects
                    KeyIndexId = readInt32 "key_index_id" r
                    IsDisabled = readBool "is_disabled" r
                    IsSystemNamed = readBool "is_system_named" r

                    DeleteReferentialAction = r |> readByte "delete_referential_action" |> toReferentialAction
                    UpdateReferentialAction = r |> readByte "update_referential_action" |> toReferentialAction

                    Columns = match RCMap.tryPick object_id fkColumns with Some cs -> cs | None -> [||]

                    XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, object_id, 0) xProperties
                } :: acc)
            []
        |> DbTr.commit_ connection
            
    let stableOrder (fk : ForeignKey) =
        if fk.IsSystemNamed 
        then 
            let name' = fk.Name.[0..fk.Name.LastIndexOf '_']
            let parentColumns = fk.Columns |> Array.joinBy "," (fun c -> c.ParentColumn.Name) 
            let refColumns = fk.Columns |> Array.joinBy "," (fun c -> c.ReferencedColumn.Name)
            sprintf "%s%s%s%s" name' fk.Referenced.Name parentColumns refColumns
        else fk.Name

    let readAll objects fkColumns xProperties connection =
        let foreignKeys' = readAll' objects fkColumns xProperties connection
        let foreignKeysByParent =
            foreignKeys'
            |> List.groupBy (fun fk -> fk.Parent.ObjectId)
            |> List.map (fun (parent_id, fks) -> parent_id, fks |> List.sortBy stableOrder |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        let foreignKeysByReferenced =
            foreignKeys'
            |> List.groupBy (fun fk -> fk.Referenced.ObjectId)
            |> List.map (fun (referenced_id, fks) -> referenced_id, fks |> List.sortBy (fun fk -> fk.Name) |> List.toArray)
            |> Map.ofList
            |> RCMap.ofMap
        foreignKeysByParent, foreignKeysByReferenced
