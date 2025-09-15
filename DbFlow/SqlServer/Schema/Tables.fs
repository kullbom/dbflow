namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-table-types-transact-sql?view=sql-server-ver17

type TableType = {
    Schema : Schema
    Name : string
    Object : OBJECT
  
    Columns : Column array
    Indexes : Index array
    
    ForeignKeys : ForeignKey array
    ReferencedForeignKeys : ForeignKey array

    CheckConstraints : CheckConstraint array
    DefaultConstraints : DefaultConstraint array
}

module TableType = 
    let readAll schemas objects xProperties columns indexes foreignKeys referencedForeignKeys checkConstraints defaultConstraints connection =
        DbTr.readMap
            "SELECT tt.schema_id, tt.name type_name, tt.type_table_object_id, tt.is_memory_optimized  
             FROM sys.table_types tt"
            []
            (fun r -> 
                let object_id = readInt32 "type_table_object_id" r
                object_id,
                {
                    Schema = RCMap.pick (readInt32 "schema_id" r) schemas
                    Name = readString "type_name" r
                    Object = RCMap.pick object_id objects
                  
                    Columns = match RCMap.tryPick object_id columns with Some cs -> cs | None -> [||]
                    Indexes = match RCMap.tryPick object_id indexes with Some ixs -> ixs | None -> [||]
                    ForeignKeys = match RCMap.tryPick object_id foreignKeys with Some trs -> trs | None -> [||]
                    ReferencedForeignKeys = match RCMap.tryPick object_id referencedForeignKeys with Some trs -> trs | None -> [||]
                    CheckConstraints = match RCMap.tryPick object_id checkConstraints with Some ccs -> ccs | None -> [||] 
                    DefaultConstraints = match RCMap.tryPick object_id defaultConstraints with Some dcs -> dcs | None -> [||]
                })
        |> DbTr.commit_ connection
        

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-tables-transact-sql?view=sql-server-ver17

type Table = {
    Schema : Schema
    Name : string
    Object : OBJECT
    
    Columns : Column array
    Indexes : Index array
    Triggers : Trigger array

    ForeignKeys : ForeignKey array
    ReferencedForeignKeys : ForeignKey array

    CheckConstraints : CheckConstraint array
    DefaultConstraints : DefaultConstraint array

    XProperties : Map<string, string>
}

module Table = 
    let fullName (t : Table) = $"[{t.Schema.Name}].[{t.Name}]"

    let readAll schemas objects columns indexes triggers foreignKeys referencedForeignKeys checkConstraints defaultConstraints xProperties connection =
        objects
        |> RCMap.fold
            (fun acc _ _ (o : OBJECT) -> 
                match o.ObjectType with
                | ObjectType.UserTable -> 
                    let objectId = o.ObjectId
                    {
                        Schema = o.Schema
                        Name = o.Name
                        Object = o
                        
                        Columns = RCMap.tryPick objectId columns |> Option.escape [||]
                        Indexes = RCMap.tryPick objectId indexes |> Option.escape [||]
                        Triggers = RCMap.tryPick objectId triggers |> Option.escape [||]

                        ForeignKeys = RCMap.tryPick objectId foreignKeys |> Option.escape [||]
                        ReferencedForeignKeys = RCMap.tryPick objectId referencedForeignKeys |> Option.escape [||]

                        CheckConstraints = RCMap.tryPick objectId checkConstraints |> Option.escape [||] 
                        DefaultConstraints = RCMap.tryPick objectId defaultConstraints |> Option.escape [||]

                        XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, objectId, 0) xProperties
                    } :: acc
                | _ -> acc)
            []
        |> List.sortBy (fun t -> t.Schema.Name, t.Name)


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-views-transact-sql?view=sql-server-ver17

type View = {
    Schema : Schema
    Name : string
    Object : OBJECT
    
    Definition : string

    Columns : Column array
    Indexes : Index array
    Triggers : Trigger array

    XProperties : Map<string, string>
}

module View = 
    let fullName (v : View) = $"[{v.Schema.Name}].[{v.Name}]"
    
    let readAll schemas objects columns indexes triggers (sql_modules : RCMap<int, SqlModule>) xProperties connection =
        objects
        |> RCMap.fold
            (fun acc _ _ (o : OBJECT) -> 
                match o.ObjectType with
                | ObjectType.View -> 
                    let objectId = o.ObjectId
                    {
                        Schema = o.Schema
                        Name = o.Name
                        Object = o
                        
                        Definition = 
                            let sqlModule = RCMap.pick objectId sql_modules
                            sqlModule.Definition |> String.trim

                        Columns = RCMap.tryPick objectId columns |> Option.escape [||]
                        Indexes = RCMap.tryPick objectId indexes |> Option.escape [||]
                        Triggers = RCMap.tryPick objectId triggers |> Option.escape [||]

                        XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, objectId, 0) xProperties
                    } :: acc
                |_ -> acc)
            []
        |> List.sortBy (fun v -> v.Schema.Name, v.Name)

    
        