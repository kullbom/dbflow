module DbFlow.Test.ShowPlanXml

open System.Xml

open DbFlow
open DbFlow.Test.XmlParser

let ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan"

type StmtSimpleType () = class end 
type StmtCondType () = class end
type StmtCursorType () = class end
type StmtReceiveType () = class end
type StmtUseDbType () = class end

/// The statement block that contains many statements
type StmtBlockType = 
    | StmtSimple of StmtSimpleType
    | StmtCond of StmtCondType
    | StmtCursor of StmtCursorType 
    | StmtReceive of StmtReceiveType
    | StmtUseDb

type Batch = {
    // minOccurs="1" maxOccurs="unbounded"
    // The statement block that contains many statements
    Statements : StmtBlockType list
}

type ShowPlanXML = {
    // minOccurs="1" maxOccurs="unbounded"
    Batches : Batch list

    // Attributes
    Version : string 
    Build : string
    ClusteredMode : bool option
}

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
            Batches = []
        }
    }


