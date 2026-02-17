module DbFlow.FileSystem

open System.IO

let writeAllText path (fileContent : string) =
    IO.create
        (fun () -> File.WriteAllText(path, fileContent))
        (fun () -> File.WriteAllTextAsync(path, fileContent) |> Task.toUnit)

