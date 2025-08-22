namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-table-types-transact-sql?view=sql-server-ver17

type TableType = {
    Schema : Schema
    Name : string
    Object : OBJECT
  
    Columns : COLUMN array
    Indexes : INDEX array
    //mutable triggers : TRIGGER array
    
    ForeignKeys : FOREIGN_KEY array
    ReferencedForeignKeys : FOREIGN_KEY array

    CheckConstraints : CheckConstraint array
    DefaultConstraints : DefaultConstraint array

    MSDescription : string option
}

module TableType = 
    let readAll schemas objects ms_descriptions columns indexes foreignKeys referencedForeignKeys checkConstraints defaultConstraints connection =
        DbTr.reader
            "SELECT tt.schema_id, tt.name type_name, tt.type_table_object_id, tt.is_memory_optimized  
             FROM sys.table_types tt"
            []
            (fun acc r -> 
                let object_id = readInt32 "type_table_object_id" r
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

                    MSDescription = 
                        //RCMap.tryPick (XPROPERTY_CLASS.TYPE, object_id, 0) ms_descriptions
                        RCMap.tryPick (XPropertyClass.ObjectOrColumn, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> List.sortBy (fun t -> t.Schema.Name, t.Name)


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-tables-transact-sql?view=sql-server-ver17

type TABLE = {
    Schema : Schema
    Name : string
    Object : OBJECT
    
    Columns : COLUMN array
    Indexes : INDEX array
    Triggers : TRIGGER array

    ForeignKeys : FOREIGN_KEY array
    ReferencedForeignKeys : FOREIGN_KEY array

    CheckConstraints : CheckConstraint array
    DefaultConstraints : DefaultConstraint array

    MSDescription : string option
}

module TABLE = 
    let readAll schemas objects columns indexes triggers foreignKeys referencedForeignKeys checkConstraints defaultConstraints ms_descriptions connection =
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

                    MSDescription = RCMap.tryPick (XPropertyClass.ObjectOrColumn, objectId, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> List.sortBy (fun t -> t.Schema.Name, t.Name)


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-views-transact-sql?view=sql-server-ver17

type VIEW = {
    Schema : Schema
    Name : string
    Object : OBJECT
    
    Definition : string

    Columns : COLUMN array
    Indexes : INDEX array
    Triggers : TRIGGER array

    MSDescription : string option
}

module VIEW = 
    let readAll schemas objects columns indexes triggers (sql_modules : RCMap<int, SqlModule>) ms_descriptions connection =
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
                    
                    Definition = (RCMap.pick objectId sql_modules).Definition.Trim()

                    Columns = RCMap.tryPick objectId columns |> Option.escape [||]
                    Indexes = RCMap.tryPick objectId indexes |> Option.escape [||]
                    Triggers = RCMap.tryPick objectId triggers |> Option.escape [||]

                    MSDescription = RCMap.tryPick (XPropertyClass.ObjectOrColumn, objectId, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> List.sortBy (fun v -> v.Schema.Name, v.Name)

    
        