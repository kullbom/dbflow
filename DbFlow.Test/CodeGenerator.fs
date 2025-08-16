module DbFlow.Tests.CodeGenerator

open Xunit
open Xunit.Abstractions

open DbFlow
open DbFlow.SqlServer.Schema

type ``The code generator`` (outputHelper:ITestOutputHelper) = 
    let logger s = outputHelper.WriteLine s
    let targetFolder' = __SOURCE_DIRECTORY__ + "\\..\\DbFlow\\SqlServer\\Schema\\"
    let targetFolder = System.IO.Path.GetFullPath (targetFolder')
        
    let listType = typeof<list<_>>.GetGenericTypeDefinition()
    let optionType = typeof<option<_>>.GetGenericTypeDefinition()
    let mapType = typeof<Map<_,_>>.GetGenericTypeDefinition()

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
                    let properties = t.GetProperties ()
                    let propertyNames = properties |> Array.map (fun p -> p.Name)
                    
                    let seed' = f seed name (Some propertyNames)
                    
                    properties
                    |> Array.fold (fun (visited'', acc) p ->
                        let pt = p.PropertyType 
                        if pt.IsGenericType
                        then 
                            match pt.GetGenericTypeDefinition(), pt.GenericTypeArguments with
                            | t, [| t' |] when t = listType -> foldTypes t' f (visited'', acc)
                            | t, [| t' |] when t = optionType -> foldTypes t' f (visited'', acc)
                            | t, [| kt; vt |] when t = mapType -> foldTypes kt f (visited'', acc) |> foldTypes vt f 
                            | t, ts' -> 
                                logger (sprintf "UNKNOWN TYPE: %A %A" t ts')
                                visited'', acc
                        else foldTypes pt f (visited'', acc))
                        (visited', seed')
            else visited', f seed name None
         

    [<Fact>]
    member x.``Generate comparison`` () =
        let sb = System.Text.StringBuilder ()
        let append (s : string) = sb.Append s |> ignore
        append """namespace DbFlow.SqlServer.Schema

open DbFlow

type Compare = Compare  
    with """

        
        foldTypes typeof<DATABASE>
            (fun () (ty : string) fields -> 
                match fields with
                | None -> 
                    append $"        static member IsSame (x0 : {ty}, x1 : {ty}) = (x0 = x1)"
                | Some fs ->
                    append $"        static member IsSame (x0 : {ty}, x1 : {ty}) =" 
                    
                    fs
                    |> Array.map (fun f -> $"IsSame (x0.{f}, x1.{f})")
                    |> Array.joinBy 
                            "
                && "        
                        id
                    |> append)
            (Set.empty, ())
        |> ignore

        let file = System.IO.Path.Combine (targetFolder, "Compare.fs")
        
        if System.IO.File.Exists file
        then System.IO.File.Delete file

        System.IO.File.WriteAllText (file, sb.ToString ())



