module DbFlow.SqlServer.RawOut

open DbFlow
open DbFlow.SqlServer.Schema

let boolOut = function true -> "✓" | false -> "-"
let optionOut = function Some s -> s | None -> "-"
let optionOutT t = function Some s -> t s | None -> "-"

let filteredWriteL headerWriter exclude f xs =
    xs 
    |> List.fold 
        (fun i x -> 
            if not (exclude x)
            then
                if i = 0 then headerWriter ()
                f x
                i + 1
            else i)
        0
    |> ignore<int>

let filteredWriteA headerWriter exclude f xs =
    xs 
    |> Array.fold 
        (fun i x -> 
            if not (exclude x)
            then
                if i = 0 then headerWriter ()
                f x
                i + 1
            else i)
        0
    |> ignore<int>

let outputTableContent wl indent indent1 columns defaultConstraints checkConstraints triggers =
    columns
    |> filteredWriteA 
        (fun () -> wl $"{indent}COLUMNS")
        (fun _ -> false)
        (fun (c : COLUMN) -> 
            let indent = indent + indent1 
            let ansiPadded = if c.is_ansi_padded then " ANSI_PADDED" else ""
            let rowguidcol = if c.is_rowguidcol then " ROWGUIDCOL" else ""
            let maskedFunction = match c.masking_function with None -> "" | Some m -> $" MASKED_WITH ({m})"
            wl $"{indent}[{c.column_name}] {DATATYPE.typeStr false false c.data_type}{ansiPadded}{rowguidcol}{maskedFunction}")
    defaultConstraints
    |> filteredWriteA 
        (fun () -> wl $"{indent}DEFAULT CONSTRAINTS")
        (fun _ -> false)
        (fun (dc : DEFAULT_CONSTRAINT) -> 
            let indent = indent + indent1 
            let name = if dc.is_system_named then "SYSTEM NAMED" else dc.object.name
            wl $"{indent}{name}:{dc.column.column_name}:{dc.definition}")
    checkConstraints
    |> filteredWriteA 
        (fun () -> wl $"{indent}CHECK CONSTRAINTS")
        (fun _ -> false)
        (fun (cc : CHECK_CONSTRAINT) -> 
            let indent = indent + indent1 
            let name = if cc.is_system_named then "SYSTEM NAMED" else cc.object.name
            wl $"{indent}{name}:{cc.definition}")
    triggers
    |> filteredWriteA
        (fun () -> wl $"{indent}TRIGGERS")
        (fun _ -> false)
        (fun (tr : TRIGGER) ->
            let indent = indent + indent1 
            let disabled = if tr.is_disabled then " DISABLED" else ""
            let insteadOf = if tr.is_instead_of_trigger then " INSTEAD_OF" else ""
            wl $"{indent}{tr.trigger_name}{disabled}{insteadOf}"
        )


let generateRawOutput filename (db : DATABASE) =
    if System.IO.File.Exists filename
    then System.IO.File.Delete(filename)

    let allTypes = 
        db.TYPES
        |> List.map (fun t -> t.user_type_id, t)
        |> Map.ofList
    
    IO.writeToFile
        filename
        (fun writer ->
            let wl : string -> unit = writer.WriteLine
            let indent1 = "   "
            let indent = indent1
            db.SCHEMAS
            |> filteredWriteL 
                (fun () -> wl "SCHEMAS")
                (fun s -> s.is_system_schema) 
                (fun s -> wl $"{indent}{s.name}:{s.principal_name}:{boolOut s.is_system_schema}")
            db.TYPES
            |> filteredWriteL
                (fun () -> wl "USER_DEFINED_TYPES")
                (fun t -> not t.is_user_defined || t.table_datatype.IsSome)
                (fun t -> 
                    let tDef =
                        let base_type = allTypes |> Map.find (int t.system_type_id)
                        DATATYPE.typeStr' false false base_type.name base_type.sys_datatype t.parameter
                    let nullStr = if t.parameter.is_nullable then "NULL" else "NOT NULL"
                    wl $"{indent}{t.name}:{t.schema.name}:{tDef} {nullStr}")
            db.TABLE_TYPES
            |> filteredWriteL
                (fun () -> wl "TABLE_TYPES")
                (fun t -> false)
                (fun t -> 
                    wl $"{indent}{t.type_name}:{t.schema.name}"
                    let indent = indent + indent1 
                    outputTableContent wl indent indent1 t.columns t.defaultConstraints t.checkConstraints [||])
                
            db.TABLES
            |> filteredWriteL 
                (fun () -> wl "TABLES")
                (fun _ -> false)
                (fun t -> 
                    wl $"{indent}{t.table_name}:{t.schema.name}"
                    let indent = indent + indent1 
                    outputTableContent wl indent indent1 t.columns t.defaultConstraints t.checkConstraints t.triggers)
            db.VIEWS
            |> filteredWriteL
                (fun () -> wl "VIEWS")
                (fun _ -> false)
                (fun v ->
                    wl $"{indent}{v.view_name}:{v.schema.name}"
                    let indent = indent + indent1 
                    v.columns
                    |> filteredWriteA 
                        (fun () -> wl $"{indent}COLUMNS")
                        (fun _ -> false)
                        (fun c -> 
                            let indent = indent + indent1 
                            let ansiPadded = if c.is_ansi_padded then " ANSIPADDED" else ""
                            let rowguidcol = if c.is_rowguidcol then " ROWGUIDCOL" else ""
                            wl $"{indent}[{c.column_name}] {DATATYPE.typeStr false false c.data_type}{ansiPadded}{rowguidcol}")
                    v.triggers
                    |> filteredWriteA
                        (fun () -> wl $"{indent}TRIGGERS")
                        (fun _ -> false)
                        (fun tr ->
                            let indent = indent + indent1 
                            let disabled = if tr.is_disabled then " DISABLED" else ""
                            let insteadOf = if tr.is_instead_of_trigger then " INSTEAD_OF" else ""
                            wl $"{indent}{tr.trigger_name}{disabled}{insteadOf}"
                        )
                )
            db.PROCEDURES
            |> filteredWriteL
                (fun () -> wl "PROCEDURES")
                (fun p -> p.object.object_type <> OBJECT_TYPE.SQL_STORED_PROCEDURE)
                (fun p ->
                    let indent = indent + indent1
                    wl $"{indent}{p.name}"
                    )
            db.PROCEDURES
            |> filteredWriteL
                (fun () -> wl "FUNCTIONS")
                (fun p -> p.object.object_type = OBJECT_TYPE.SQL_STORED_PROCEDURE)
                (fun p ->
                    let indent = indent + indent1
                    wl $"{indent}{p.name}"
                    )
            db.SYNONYMS
            |> filteredWriteL 
                (fun () -> wl "SYNONYMS")
                (fun s -> false) 
                (fun s -> 
                    let indent = indent + indent1
                    wl $"{indent}{s.object.name}:{s.object.schema.name}:{s.base_object_name}")

            db.SEQUENCES
            |> filteredWriteL 
                (fun () -> wl "SEQUENCES")
                (fun s -> false) 
                (fun s -> 
                    let indent = indent + indent1
                    let typeStr = DATATYPE.typeStr false false s.data_type
                    let options = $"{optionOutT string s.minimum_value}:{optionOutT string  s.maximum_value}:{boolOut s.is_cycling}:{optionOutT string  s.cache_size}"
                    
                    wl $"{indent}{s.object.name}:{s.object.schema.name}:{typeStr}:{s.start_value}:{s.increment}:{options}")
            db.XML_SCHEMA_COLLECTIONS
            |> filteredWriteL 
                (fun () -> wl "XML_SCHEMA_COLLECTIONS")
                (fun c -> false) 
                (fun c -> 
                    let indent = indent + indent1
                    
                    wl $"{indent}{c.name}")
        )

