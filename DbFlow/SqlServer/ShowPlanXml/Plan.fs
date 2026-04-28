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
            // elements
            let! (batches, rest) = 
                xElementsAll root
                |> xElementReq (ensureName ("BatchSequence", ns) Parsers.Internal.parseBatchSequence)
            do! xElementEnsureEmpty rest
            return {
                Version = version
                Build = build
                ClusteredMode = clusteredMode
                BatchSequence = { Batches = batches }
            }
        } 

    let parseXElement (xml : Linq.XElement) =
        let ns = Parsers.Internal.ns
        match ensureName ("ShowPlanXML", ns) parseShowPlanXML xml with
        | POk v -> Ok v
        | Failure e -> Error e
    
    let parseXDocument (doc : Linq.XDocument) = parseXElement doc.Root
    
    let parseString s = Linq.XDocument.Parse s |> parseXDocument
    
    