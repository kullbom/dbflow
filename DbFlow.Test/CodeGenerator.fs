module DbFlow.Tests.CodeGenerator

open Xunit

type ``The code generator`` () = 
    let logger (s : string) = System.Console.Out.WriteLine s
    
    [<Fact>]
    member x.``Generate comparison code`` () =
        let fileContent = DbFlow.SqlServer.Schema.Generator.generate ()

        let targetFolder = 
            let targetFolder' = __SOURCE_DIRECTORY__ + "\\..\\DbFlow\\SqlServer\\Schema\\"
            System.IO.Path.GetFullPath (targetFolder')

        let file = System.IO.Path.Combine (targetFolder, "CompareGen.fs")
        
        if System.IO.File.Exists file
        then System.IO.File.Delete file

        System.IO.File.WriteAllText (file, fileContent)



