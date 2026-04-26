module DbFlow.XmlParser
    
module Result =
    type CEBuilder() =
        member inline b.Return (x)        = Ok x
        member inline b.Bind (p, body)    = match p with Ok x -> body x | Error e -> Error e
        member inline b.Let (p, body)     = body p
        member inline b.ReturnFrom (expr) = expr

    let builder = new CEBuilder()

let Errorf fmt = 
    Printf.ksprintf Error fmt

let inline (|>>) x f = Result.map f x 
let inline (>>=) x f = Result.bind f x 


[<Struct>]
type XName =
      Name of string
    | NSName of string * string 

type XNameResolver = XNameResolver  with
    static member inline ($) (XNameResolver , (name : string)) = XName.Name name
    
    static member inline ($) (XNameResolver , (name : string, ns : string)) = XName.NSName (name, ns)
    
let inline XN selector : XName = XNameResolver $ selector

let inline getXName name =
    match XN name with
    | Name name -> System.Xml.Linq.XName.Get (name)
    | NSName (name, ns) -> System.Xml.Linq.XName.Get (name, ns)

let inline nullGuard x = match x with null -> None | x -> Some x


// ======================================================
// Element sequences
// ======================================================

let xElementsAll (c : #System.Xml.Linq.XContainer) = 
    c.Elements () |> Seq.toList

let nameGuard' xname parser (e : System.Xml.Linq.XElement) =
    if e.Name = xname
    then 
        match parser e with 
        | Ok x -> Ok x
        | Error e -> 
            failwithf "Error parsing element %A: %s" xname e   
            Error e
    else Errorf "Expected element %A but found %A" xname e.Name

let inline nameGuard name parser (e : System.Xml.Linq.XElement) =
    nameGuard' (getXName name) parser e

let ensureEmpty (es : System.Xml.Linq.XElement list) =
    match es with
    | [] -> Ok ()
    | e :: _ -> 
        failwithf "Expected no more elements, but found %A" e.Name
        Errorf "Expected no more elements, but found %A" e.Name

let ensureOnly parser transformer (xml : System.Xml.Linq.XElement)  =
    match parser (xElementsAll xml) with
    | Ok (res, rest) -> 
        match ensureEmpty rest with
        | Ok () -> Ok (transformer res)
        | Error e -> Error e
    | Error e -> Error e

/// Parse the first element if the name matches 
let xElement parser (es : System.Xml.Linq.XElement list) =
    match es with
    | e :: es' ->
        match parser e with
        | Ok x -> Ok (Some x, es')
        | Error _ -> Ok (None, es)
    | _ -> 
        Ok (None, es)

/// Parse the first element if the name matches - else fail
let xElementReq parser (es : System.Xml.Linq.XElement list) =
    match es with
    | e :: es' ->
        parser e |> Result.map (fun x -> x, es')
    | [] -> 
        Errorf "Required element not found" 

let rec xElementMany' parser acc (es : System.Xml.Linq.XElement list) =
    match es with
    | e :: es' ->
        match parser e with
        | Ok x -> xElementMany' parser (x :: acc) es'
        | Error _ -> Ok (List.rev acc, es)
    | _ -> 
        Ok (List.rev acc, es)

/// Parse elements as long as the name matches
let xElementMany parser (es : System.Xml.Linq.XElement list) =
    xElementMany' parser [] es

/// Parse elements as long as the name matches - require at least one match
let xElementMany1 parser (es : System.Xml.Linq.XElement list) =
    match es with
    | e :: es' ->
        match parser e with
        | Ok x -> xElementMany' parser [x] es'
        | Error e -> Error e
    | _ -> 
        Errorf "Expected at least one"
    
/// Parse elements as long as the name matches - require at least two matches
let xElementMany2 parser (es : System.Xml.Linq.XElement list) =
    match es with
    | e0 :: e1 :: es' ->
        match parser e0, parser e1 with
        | Ok x0, Ok x1 -> 
            xElementMany' parser [x1; x0] es'
        | Error e, _ -> Error e
        | _, Error e -> Error e
    | _ -> 
        Errorf "Expected at least one"

// ======================================================
// Attributes
// ======================================================

module XmlReaders =
    let readBool (s : string) = 
        match s with
        | "true" | "1" -> Ok true
        | "false" | "0" -> Ok false
        | _ -> Errorf "Invalid boolean value: %s" s

    let inline readString (s : string) : Result<string, string> = s |> Ok
    let readInt32 (s : string) = 
        match System.Int32.TryParse s with
        | true, v -> Ok v
        | false, _ -> Errorf "Invalid int32 value: %s" s

    let readUInt64 (s : string) = 
        match System.UInt64.TryParse s with
        | true, v -> Ok v
        | false, _ -> Errorf "Invalid uint64 value: %s" s

    let readFloat (s : string) = 
        match System.Double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with
        | true, v -> Ok v
        | false, _ -> Errorf "Invalid float value: %s" s

type public XmlTypeMapping = 
    static member inline ($) (_:XmlTypeMapping,_:bool)   = XmlReaders.readBool     
    static member inline ($) (_:XmlTypeMapping,_:string) = XmlReaders.readString
    static member inline ($) (_:XmlTypeMapping,_:int32)  = XmlReaders.readInt32
    static member inline ($) (_:XmlTypeMapping,_:uint64) = XmlReaders.readUInt64
    static member inline ($) (_:XmlTypeMapping,_:float)  = XmlReaders.readFloat

let inline resolveAttr (a : string) : Result< ^R,string> = (Unchecked.defaultof<XmlTypeMapping> $ (Unchecked.defaultof< ^R>)) a

module Internal =
    let xAttrInternal' name resolver (e : #System.Xml.Linq.XElement) = 
        match e.Attribute name with
        | null -> None
        | a -> Some (resolver a.Value)
        
    let xAttr' name resolver (e : #System.Xml.Linq.XElement) = 
        match xAttrInternal' name resolver e with
        | Some v -> v |> Result.map Some 
        | None -> Ok None
    
    let xAttrP' name resolver attrParser (e : #System.Xml.Linq.XElement) = 
        match xAttrInternal' name resolver e with
        | Some v -> v |> Result.bind attrParser |> Result.map Some 
        | None -> Ok None
    
    let xAttrReq' name resolver (e : #System.Xml.Linq.XElement) = 
        match xAttrInternal' name resolver e with
        | Some v -> v
        | None -> 
            Errorf "Expected attribute %A not found" name
    
    let xAttrReqP' name resolver parser (e : #System.Xml.Linq.XElement) = 
        match xAttrInternal' name resolver e with
        | Some v -> v |> Result.bind parser
        | None -> 
            Errorf "Expected attribute %A not found" name

/// Reads and resolves an attribute, returning None if the attribute is not present, or an error if the value cannot be resolved to the expected type
let inline xAttr name (e : #System.Xml.Linq.XElement) = 
    Internal.xAttr' (getXName name) resolveAttr e

/// Reads and resolves an attribute, returning None if the attribute is not present, or an error if the value cannot be resolved/parsed to the expected type
let inline xAttrP name attrParser (e : #System.Xml.Linq.XElement) = 
    Internal.xAttrP' (getXName name) resolveAttr attrParser e

/// Reads and resolves an attribute, returning an error if the attribute is not present or if the value cannot be resolved to the expected type
let inline xAttrReq name (e : #System.Xml.Linq.XElement) = 
    Internal.xAttrReq' (getXName name) resolveAttr e

/// Reads and resolves an attribute, returning an error if the attribute is not present or if the value cannot be resolved/parsed to the expected type
let inline xAttrReqP name parser (e : #System.Xml.Linq.XElement) = 
    Internal.xAttrReqP' (getXName name) resolveAttr parser e

