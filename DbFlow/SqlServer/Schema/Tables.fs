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
    //mutable triggers : TRIGGER array
    
    ForeignKeys : ForeignKey array
    ReferencedForeignKeys : ForeignKey array

    CheckConstraints : CheckConstraint array
    DefaultConstraints : DefaultConstraint array

    //XProperties : Map<string, string>
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
        DbTr.reader 
            "SELECT t.name table_name, t.object_id, t.schema_id FROM sys.tables t" 
            []
            (fun acc r -> 
                let objectId = readInt32 "object_id" r
                {
                    Schema = RCMap.pick (readInt32 "schema_id" r) schemas
                    Name = readString "table_name" r
                    Object = RCMap.pick objectId objects
                    
                    Columns = match RCMap.tryPick objectId columns with Some cs -> cs | None -> [||]
                    Indexes = match RCMap.tryPick objectId indexes with Some ixs -> ixs | None -> [||]
                    Triggers = match RCMap.tryPick objectId triggers with Some trs -> trs | None -> [||]

                    ForeignKeys = match RCMap.tryPick objectId foreignKeys with Some trs -> trs | None -> [||]
                    ReferencedForeignKeys = match RCMap.tryPick objectId referencedForeignKeys with Some trs -> trs | None -> [||]

                    CheckConstraints = match RCMap.tryPick objectId checkConstraints with Some ccs -> ccs | None -> [||] 
                    DefaultConstraints = match RCMap.tryPick objectId defaultConstraints with Some dcs -> dcs | None -> [||]

                    XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, objectId, 0) xProperties
                } :: acc)
            []
        |> DbTr.commit_ connection
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
    let readAll schemas objects columns indexes triggers (sql_modules : RCMap<int, SqlModule>) xProperties connection =
        DbTr.reader
            "SELECT v.name view_name, v.object_id, v.schema_id
             FROM sys.views v"
            []
            (fun acc r -> 
                let objectId = readInt32 "object_id" r
                {
                    Schema = RCMap.pick (readInt32 "schema_id" r) schemas
                    Name = readString "view_name" r
                    Object = RCMap.pick objectId objects
                    
                    Definition = 
                        let sqlModule = RCMap.pick objectId sql_modules
                        sqlModule.Definition |> String.trim

                    Columns = RCMap.tryPick objectId columns |> Option.escape [||]
                    Indexes = RCMap.tryPick objectId indexes |> Option.escape [||]
                    Triggers = RCMap.tryPick objectId triggers |> Option.escape [||]

                    XProperties = XProperty.getXProperties (XPropertyClass.ObjectOrColumn, objectId, 0) xProperties
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> List.sortBy (fun v -> v.Schema.Name, v.Name)

    
        