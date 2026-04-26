module DbFlow.SqlServer.ShowPlanXml.Plan

open DbFlow.XmlParser

let parse (s : string) =
    let xml = failwith ""
    match Parsers.parseShowPlanXML xml with
    | POk v -> Ok v
    | Failure e -> Error e