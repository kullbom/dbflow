namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-table-types-transact-sql?view=sql-server-ver17

type TABLE_TYPE = {
    schema : SCHEMA
    type_name : string
    object : OBJECT
  
    columns : COLUMN array
    indexes : INDEX array
    //mutable triggers : TRIGGER array
    
    foreignKeys : FOREIGN_KEY array
    referencedForeignKeys : FOREIGN_KEY array

    checkConstraints : CHECK_CONSTRAINT array
    defaultConstraints : DEFAULT_CONSTRAINT array

    ms_description : string option
}

module TABLE_TYPE = 
    let readAll schemas objects ms_descriptions columns indexes foreignKeys referencedForeignKeys checkConstraints defaultConstraints connection =
        DbTr.reader
            "SELECT tt.schema_id, tt.name type_name, tt.type_table_object_id, tt.is_memory_optimized  
             FROM sys.table_types tt"
            []
            (fun acc r -> 
                let object_id = readInt32 "type_table_object_id" r
                {
                    schema = PickMap.pick (readInt32 "schema_id" r) schemas
                    type_name = readString "type_name" r
                    object = PickMap.pick object_id objects
                  
                    columns = match PickMap.tryPick object_id columns with Some cs -> cs | None -> [||]
                    indexes = match PickMap.tryPick object_id indexes with Some ixs -> ixs | None -> [||]
                    foreignKeys = match PickMap.tryPick object_id foreignKeys with Some trs -> trs | None -> [||]
                    referencedForeignKeys = match PickMap.tryPick object_id referencedForeignKeys with Some trs -> trs | None -> [||]
                    checkConstraints = match PickMap.tryPick object_id checkConstraints with Some ccs -> ccs | None -> [||] 
                    defaultConstraints = match PickMap.tryPick object_id defaultConstraints with Some dcs -> dcs | None -> [||]

                    ms_description = 
                        //PickMap.tryPick (XPROPERTY_CLASS.TYPE, object_id, 0) ms_descriptions
                        PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> List.sortBy (fun t -> t.schema.name, t.type_name)


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-tables-transact-sql?view=sql-server-ver17

type TABLE = {
    schema : SCHEMA
    table_name : string
    object : OBJECT
    
    columns : COLUMN array
    indexes : INDEX array
    triggers : TRIGGER array

    foreignKeys : FOREIGN_KEY array
    referencedForeignKeys : FOREIGN_KEY array

    checkConstraints : CHECK_CONSTRAINT array
    defaultConstraints : DEFAULT_CONSTRAINT array

    ms_description : string option
}

module TABLE = 
    let readAll schemas objects columns indexes triggers foreignKeys referencedForeignKeys checkConstraints defaultConstraints ms_descriptions connection =
        DbTr.reader 
            "SELECT t.name table_name, t.object_id, t.schema_id FROM sys.tables t" 
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                {
                    schema = PickMap.pick (readInt32 "schema_id" r) schemas
                    table_name = readString "table_name" r
                    object = PickMap.pick object_id objects
                    
                    columns = match PickMap.tryPick object_id columns with Some cs -> cs | None -> [||]
                    indexes = match PickMap.tryPick object_id indexes with Some ixs -> ixs | None -> [||]
                    triggers = match PickMap.tryPick object_id triggers with Some trs -> trs | None -> [||]

                    foreignKeys = match PickMap.tryPick object_id foreignKeys with Some trs -> trs | None -> [||]
                    referencedForeignKeys = match PickMap.tryPick object_id referencedForeignKeys with Some trs -> trs | None -> [||]

                    checkConstraints = match PickMap.tryPick object_id checkConstraints with Some ccs -> ccs | None -> [||] 
                    defaultConstraints = match PickMap.tryPick object_id defaultConstraints with Some dcs -> dcs | None -> [||]

                    ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> List.sortBy (fun t -> t.schema.name, t.table_name)


// https://learn.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-views-transact-sql?view=sql-server-ver17

type VIEW = {
    schema : SCHEMA
    view_name : string
    object : OBJECT
    
    definition : string

    columns : COLUMN array
    indexes : INDEX array
    triggers : TRIGGER array

    ms_description : string option
}

module VIEW = 
    let readAll schemas objects columns indexes triggers (sql_modules : PickMap<int, SQL_MODULE>) ms_descriptions connection =
        DbTr.reader
            "SELECT v.name view_name, v.object_id, v.schema_id
             FROM sys.views v"
            []
            (fun acc r -> 
                let object_id = readInt32 "object_id" r
                {
                    schema = PickMap.pick (readInt32 "schema_id" r) schemas
                    view_name = readString "view_name" r
                    object = PickMap.pick object_id objects
                    
                    definition = (PickMap.pick object_id sql_modules).definition.Trim()

                    columns = PickMap.tryPick object_id columns |> Option.escape [||]
                    indexes = PickMap.tryPick object_id indexes |> Option.escape [||]
                    triggers = PickMap.tryPick object_id triggers |> Option.escape [||]

                    ms_description = PickMap.tryPick (XPROPERTY_CLASS.OBJECT_OR_COLUMN, object_id, 0) ms_descriptions
                } :: acc)
            []
        |> DbTr.commit_ connection
        |> List.sortBy (fun v -> v.schema.name, v.view_name)

    
        