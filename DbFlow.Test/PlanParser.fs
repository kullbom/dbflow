module DbFlow.Test.PlanParser


open System.Xml

open DbFlow
open DbFlow.Test.XmlParser
open DbFlow.Test.ShowPlanXml    

let ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan"

let parseShowPlanXML (root : Linq.XElement) : Result<ShowPlanXML, _> =
    Result.builder {
        let! version = xAttr "Version" root |> function Some v -> v |> xAttrVal |> Ok | None -> Error "Version attribute not found" 
        let! build = root |> xAttr "Build" |> function Some v -> v |> xAttrVal |> Ok | None -> Error "Build attribute not found" 
        let! clusteredMode = 
            root 
            |> xAttr "ClusteredMode" 
            |> function Some v -> parseBoolean (xAttrVal v) |> Result.map Some | None -> Ok None
             
        //let batchSequences = 
        //    root |> xElements ("BatchSequence", ns)
        
        return {
            Version = version
            Build = build
            ClusteredMode = clusteredMode
            BatchSequence = { Batches = [] }
        }
    }
