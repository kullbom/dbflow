namespace DbFlow.SqlServer.ShowPlanXml

open System.Xml
open DbFlow.XmlParser

// ========================================
// Root Element
// ========================================

type Plan = {
    Version: string
    Build: string
    ClusteredMode: bool option
    BatchSequence: BatchSequence
}

module Plan =
    let internal parseShowPlanXML (root : Linq.XElement) : PResult<Plan, _> =
        let ns = Parsers.Internal.ns
        PResult.builder {
            let! version = xAttrReq "Version" root  
            let! build = xAttrReq "Build" root
            let! clusteredMode = xAttr "ClusteredMode" root
            let! batches = xElementReqP ("BatchSequence", ns) (xElementsP ("Batch", ns) Parsers.Internal.parseBatch) root
            return {
                Version = version
                Build = build
                ClusteredMode = clusteredMode
                BatchSequence = { Batches = batches }
            }
        }

    let parseXElement (xml : Linq.XElement) =
        match parseShowPlanXML xml with
        | POk v -> Ok v
        | Failure e -> Error e
    
    let parseXDocument (doc : Linq.XDocument) = parseXElement doc.Root
    
    let parseString s = Linq.XDocument.Parse s |> parseXDocument
    
    