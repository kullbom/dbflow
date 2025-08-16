module DbFlow.Tests.CodeGenerator

open Xunit
open Xunit.Abstractions

open DbFlow
open DbFlow.SqlServer.Schema

[<RequireQualifiedAccess>]
type PropType =
    Gen
    | List
    | Array
    | Option
    | Special of (string -> string)

let listType = typeof<list<_>>.GetGenericTypeDefinition()
let optionType = typeof<option<_>>.GetGenericTypeDefinition()
let mapType = typeof<Map<_,_>>.GetGenericTypeDefinition()

let propertyType (parent : System.Type) (pName : string) (pType :System.Type) =
    match pName with
    | "dependencies" when parent = typeof<DATABASE> -> PropType.Special (fun pname -> "")
    | "schema_id" when parent = typeof<SCHEMA> -> PropType.Special (fun pname -> "")
    | "object_id" when parent = typeof<OBJECT> -> PropType.Special (fun pname -> "")
    | "parent_object_id" when parent = typeof<OBJECT> -> PropType.Special (fun pname -> "")
    | "create_date" -> PropType.Special (fun pname -> "")
    | "modify_date" -> PropType.Special (fun pname -> "")
    | "all_objects" -> PropType.Special (fun pname -> "")
    | "orig_definition" -> PropType.Special (fun pname -> "")
    
    // TODO: This should be part of the generated scripts
    | "ms_description" -> PropType.Special (fun pname -> "")

    | "name" when parent = typeof<OBJECT> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.object_name (x0, x1, path, diff')") 
    
    | "name" when parent = typeof<INDEX> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.index_name (x0, x1, path, diff')") 
    | "object" when parent = typeof<INDEX> -> PropType.Special (fun _ -> "")
    | "is_system_named" when parent = typeof<INDEX> -> PropType.Special (fun _ -> "")
    
    | "name" when parent = typeof<FOREIGN_KEY> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.foreign_key_name (x0, x1, path, diff')") 
    | "object" when parent = typeof<FOREIGN_KEY> -> PropType.Special (fun _ -> "")
    | "is_system_named" when parent = typeof<FOREIGN_KEY> -> PropType.Special (fun _ -> "")

    | "name" when parent = typeof<CHECK_CONSTRAINT> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.check_constraint_name (x0, x1, path, diff')") 
    | "object" when parent = typeof<CHECK_CONSTRAINT> -> PropType.Special (fun _ -> "")
    | "is_system_named" when parent = typeof<CHECK_CONSTRAINT> -> PropType.Special (fun _ -> "")

    | "name" when parent = typeof<DEFAULT_CONSTRAINT> -> PropType.Special (fun _pname -> $"|> fun diff' -> Compare.default_constraint_name (x0, x1, path, diff')") 
    | "object" when parent = typeof<DEFAULT_CONSTRAINT> -> PropType.Special (fun _ -> "")
    | "is_system_named" when parent = typeof<DEFAULT_CONSTRAINT> -> PropType.Special (fun _ -> "")

    | "sequence_definition" when parent = typeof<SEQUENCE> -> PropType.Special (fun pname -> $"|> fun diff' -> Compare.sequence_definition (x0.{pname}, x1.{pname}, \"{pname}\" :: path, diff')")
   
    | "identity_definition" when parent = typeof<COLUMN> -> PropType.Special (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.identity_definition (\"{pname}\" :: path)")
    | "sys_datatype"      when parent = typeof<DATATYPE> -> PropType.Special (fun pname -> $"|> Compare.collectOption x0.{pname} x1.{pname} Compare.equalCollector (\"{pname}\" :: path)")
    
    | "object_type" when pType = typeof<OBJECT_TYPE> -> PropType.Special (fun pname -> $"|> fun diff' -> Compare.equalCollector (x0.{pname}, x1.{pname}, (\"{pname}\" :: path), diff')")
    | "index_type" when pType = typeof<INDEX_TYPE> -> PropType.Special (fun pname -> $"|> fun diff' -> Compare.equalCollector (x0.{pname}, x1.{pname}, (\"{pname}\" :: path), diff')")
    | _ ->
        if pType.IsArray
        then PropType.Array
        else if pType.IsGenericType
        then 
            match pType.GetGenericTypeDefinition() with
            | t when t = listType -> PropType.List
            | t when t = optionType -> PropType.Option
        else PropType.Gen
    
type ``The code generator`` (outputHelper:ITestOutputHelper) = 
    let logger s = outputHelper.WriteLine s
    let targetFolder' = __SOURCE_DIRECTORY__ + "\\..\\DbFlow\\SqlServer\\Schema\\"
    let targetFolder = System.IO.Path.GetFullPath (targetFolder')
        
    let rec foldTypes (t : System.Type) f (visited, seed) =
        if Set.contains t.FullName visited
        then
            visited, seed
        else
            let visited' = Set.add t.FullName visited
            let name = t.Name 
            if t.Namespace = "DbFlow.SqlServer.Schema"
            then
                if t.IsArray
                then foldTypes (t.GetElementType ()) f (visited', seed)
                else 
                    let properties' = t.GetProperties ()
                    let properties =
                        properties'
                        |> Array.map (fun p -> propertyType t p.Name p.PropertyType, p.Name)
                    
                    let seed' = f seed name (Some properties)
                    
                    properties'
                    |> Array.fold (fun (visited'', acc) p ->
                        let pt = p.PropertyType 
                        if pt.IsGenericType
                        then 
                            match pt.GetGenericTypeDefinition(), pt.GenericTypeArguments with
                            | t, [| t' |] when t = listType -> foldTypes t' f (visited'', acc)
                            | t, [| t' |] when t = optionType -> foldTypes t' f (visited'', acc)
                            | t, [| kt; vt |] when t = mapType -> foldTypes kt f (visited'', acc) |> foldTypes vt f 
                        else foldTypes pt f (visited'', acc))
                        (visited', seed')
            else visited', f seed name None
         

    [<Fact>]
    member x.``Generate comparison`` () =
        let sb = System.Text.StringBuilder ()
        let append (s : string) = sb.AppendLine s |> ignore
        append """namespace DbFlow.SqlServer.Schema

open DbFlow

type CompareGen = CompareGenCase
    with"""

        
        foldTypes typeof<DATABASE>
            (fun () (ty : string) fields -> 
                if ty.StartsWith "<>" || ty.StartsWith "FSharpList" || ty = "Object" 
                    || ty = "OBJECT_TYPE" || ty = "SYS_DATATYPE" || ty = "INDEX_TYPE" || ty = "IDENTITY_DEFINITION" || ty = "SEQUENCE_DEFINITION"
                then ()
                else
                    append ""
                    match fields with
                    | None -> 
                        append $"        static member Collect (x0 : System.{ty}, x1 : System.{ty}, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)"
                    | Some fs ->
                        append $"        static member Collect (x0 : {ty}, x1 : {ty}, path, diffs) ="
                        append $"                    diffs"
                        fs
                        |> Array.choose 
                            (fun (pt, pname) -> 
                                match pt with
                                | PropType.Gen -> $"|> fun diffs' -> CompareGen.Collect (x0.{pname}, x1.{pname}, \"{pname}\" :: path, diffs')"
                                | PropType.Option -> $"|> Compare.collectOption x0.{pname} x1.{pname} CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.List -> $"|> Compare.collectList x0.{pname} x1.{pname} SortOrder.orderBy Sequence.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Array -> $"|> Compare.collectArray x0.{pname} x1.{pname} SortOrder.orderBy Sequence.elementId CompareGen.Collect (\"{pname}\" :: path)"
                                | PropType.Special special -> special pname
                                |> function "" -> None | s -> Some s 
                            )
                        |> Array.iter
                            (fun s -> append $"                    {s}"))
            (Set.empty, ())
        |> ignore

        let file = System.IO.Path.Combine (targetFolder, "CompareGen.fs")
        
        if System.IO.File.Exists file
        then System.IO.File.Delete file

        System.IO.File.WriteAllText (file, sb.ToString ())



