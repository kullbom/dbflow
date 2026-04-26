namespace DbFlow.SqlServer.Experimental.ShowPlanXml

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
    let internal parseShowPlanXML (root : Linq.XElement) : Result<Plan, _> =
        let ns = Parsers.Internal.ns
        Result.builder {
            let! version = xAttrReq "Version" root  
            let! build = xAttrReq "Build" root
            let! clusteredMode = xAttr "ClusteredMode" root
            // elements
            let! (batches, rest) = 
                xElementsAll root
                |> xElementReq (nameGuard ("BatchSequence", ns) Parsers.Internal.parseBatchSequence)
            do! ensureEmpty rest
            return {
                Version = version
                Build = build
                ClusteredMode = clusteredMode
                BatchSequence = { Batches = batches }
            }
        } 

    let parseXElement (xml : Linq.XElement) =
        let ns = Parsers.Internal.ns
        nameGuard ("ShowPlanXML", ns) parseShowPlanXML xml 
    
    let parseXDocument (doc : Linq.XDocument) = parseXElement doc.Root
    
    let parseString s = Linq.XDocument.Parse s |> parseXDocument

    
    