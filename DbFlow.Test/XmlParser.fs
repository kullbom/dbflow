module DbFlow.Test.XmlParser

open DbFlow

type XName =
      Name of string
    | NSName of string * string 

type XNameResolver = XNameResolver  with
    static member inline ($) (XNameResolver , (name : string)) = XName.Name name
    
    static member inline ($) (XNameResolver , (name : string, ns : string)) = XName.NSName (name, ns)
    
let inline XN selector : XName = XNameResolver $ selector

type XPath =
      XName of XName
    | NSPath of (string * string) list

type XPathResolver = XPathResolver with
    static member inline ($) (XPathResolver, (name : string)) = XPath.XName (XName.Name name)
    
    static member inline ($) (XPathResolver, (name : string, ns : string)) = XPath.XName (XName.NSName (name, ns))
    
    static member inline ($) (XPathResolver, path : (string * string) list) = XPath.NSPath path

let inline XP selector = XPathResolver $ selector


type XValResolver = XValResolver with
    static member xValue (x : #System.Xml.Linq.XElement) = x.Value
    static member xValue (x : System.Xml.Linq.XAttribute) = x.Value

let nullGuard = function null -> None | x -> Some x

let xElementRequire' (c : #System.Xml.Linq.XContainer) =
    let name2element (c' : #System.Xml.Linq.XContainer) name =
        c'.Element (System.Xml.Linq.XName.Get (name)) |> nullGuard
    let nsname2element (c' : #System.Xml.Linq.XContainer) (name, ns) =
        c'.Element (System.Xml.Linq.XName.Get (name,ns)) |> nullGuard
        
    function XName (Name name) -> match name2element c name with Some e -> Ok e | None -> Result.Errorf "Expected %s not found" name
           | XName (NSName (name, ns)) -> match nsname2element c (name, ns) with Some e -> Ok e | None -> Result.Errorf "Expected %s (ns: %s) not found" name ns
           | NSPath path -> let rec followPath (c' : System.Xml.Linq.XContainer) =
                                function []      -> Error "Nej, jättedålig input" 
                                       | [last]  -> 
                                            match nsname2element c' last with
                                            | Some e -> Ok e
                                            | None -> let (name, ns) = last in Result.Errorf "Expected %s (ns: %s) not found" name ns
                                       | p :: ps -> match nsname2element c' p with
                                                    | Some c'' -> followPath (c'' :> System.Xml.Linq.XContainer) ps
                                                    | None     -> let (name, ns) = p in Result.Errorf "Expected %s (ns: %s) not found" name ns
                            followPath c path


let xElementOptional' (c : #System.Xml.Linq.XContainer) =
    let name2element (c' : #System.Xml.Linq.XContainer) name =
        c'.Element (System.Xml.Linq.XName.Get (name)) |> nullGuard 
    let nsname2element (c' : #System.Xml.Linq.XContainer) (name, ns) =
        c'.Element (System.Xml.Linq.XName.Get (name,ns)) |> nullGuard 
        
    function XName (Name name) -> name2element c name |> Ok
           | XName (NSName (name, ns)) -> nsname2element c (name, ns) |> Ok
           | NSPath path -> let rec followPath (c' : System.Xml.Linq.XContainer) =
                                function []      -> Ok None 
                                       | [last]  -> nsname2element c' last |> Ok
                                       | p :: ps -> match nsname2element c' p with
                                                    | Some c'' -> followPath (c'' :> System.Xml.Linq.XContainer) ps
                                                    | None     -> Ok None
                            followPath c path

let xElements' (c : #System.Xml.Linq.XContainer) =
    function Name name -> c.Elements (System.Xml.Linq.XName.Get (name)) |> Seq.toList
           | NSName (name, ns) -> c.Elements (System.Xml.Linq.XName.Get (name,ns)) |> Seq.toList

// Simple navigation

///<summary>xElement : name/names -> XContainer -> Result<xElement, string>
/// ('name' is string OR string * string)</summary>
let inline xElementRequire path (c : #System.Xml.Linq.XContainer) = xElementRequire' c (XP path)

///<summary>xElement : name/names -> XContainer -> Result<xElement option, string>
/// ('name' is string OR string * string)</summary>
let inline xElementOptional parser path (c : #System.Xml.Linq.XContainer) = 
    match xElementOptional' c (XP path) with
    | Ok (Some e) -> parser e |> Result.map Some
    | Ok None -> Ok None
    | Error e -> Error e

///<summary>xElements : name -> XContainer -> XElement list    
/// ('name' is string OR string * string)</summary>
let inline xElements name (c : #System.Xml.Linq.XContainer) = xElements' c (XN name)

let xElementsAll (c : #System.Xml.Linq.XContainer) = c.Elements () |> Seq.toList


module XmlReaders =
    let readBool (s : string) = 
        match s with
        | "true" | "1" -> Ok true
        | "false" | "0" -> Ok false
        | _ -> Result.Errorf "Invalid boolean value: %s" s

    let readString  (s : string) : Result<string, string> = s |> Ok

    let readInt32   (s : string) = 
        match System.Int32.TryParse s with
        | true, v -> Ok v
        | false, _ -> Result.Errorf "Invalid int32 value: %s" s

    let readUInt64  (s : string) = 
        match System.UInt64.TryParse s with
        | true, v -> Ok v
        | false, _ -> Result.Errorf "Invalid uint64 value: %s" s

    let readFloat   (s : string) = 
        match System.Double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with
        | true, v -> Ok v
        | false, _ -> Result.Errorf "Invalid float value: %s" s
        
    //let optionReader (reader : System.Xml.Linq.XAttribute -> Result<'a, string>) (a : System.Xml.Linq.XAttribute) =
    //    match a..ValueKind with
    //    | Json.JsonValueKind.Null -> Ok None
    //    | _ -> reader j |> Result.map Some

type public XmlTypeMapping = 
    static member inline ($) (_:XmlTypeMapping,_:bool)   = XmlReaders.readBool     
    static member inline ($) (_:XmlTypeMapping,_:string) = XmlReaders.readString
    static member inline ($) (_:XmlTypeMapping,_:int32)  = XmlReaders.readInt32
    static member inline ($) (_:XmlTypeMapping,_:uint64) = XmlReaders.readUInt64
    static member inline ($) (_:XmlTypeMapping,_:float)  = XmlReaders.readFloat
    
    //static member inline ($) (_:XmlTypeMapping,_:^K option) =
    //    Readers.optionReader (Unchecked.defaultof<XmlTypeMapping> $ Unchecked.defaultof< ^K>)

/// Convert a JsonElement to the expected output type (or Error)
let inline resolveAttr (a : string) : Result< ^R,string> = (Unchecked.defaultof<XmlTypeMapping> $ (Unchecked.defaultof< ^R>)) a
///<summary>xAttr : name -> XElement -> XAttribute option    
/// ('name' is string OR string * string)</summary>
let inline xAttr' name (e : #System.Xml.Linq.XElement) = 
    e.Attribute 
        (match XN name with
         | Name name -> System.Xml.Linq.XName.Get (name)
         | NSName (name, ns) -> System.Xml.Linq.XName.Get (name, ns))
    |> nullGuard
    |> Option.map (fun a -> a.Value)
    
let inline xAttr name (e : #System.Xml.Linq.XElement) = 
    match xAttr' name e with
    | Some v -> resolveAttr v |> Result.map Some 
    | None -> Ok None

let inline xAttrTr t name (e : #System.Xml.Linq.XElement) = 
    match xAttr' name e with
    | Some v -> resolveAttr v |> Result.bind t |> Result.map Some 
    | None -> Ok None

let inline xAttrRequire name (e : #System.Xml.Linq.XElement) = 
    match xAttr' name e with
    | Some v -> resolveAttr v
    | None -> Result.Errorf "Expected attribute %A not found" name

///<summary>xVal : XElement -> string</summary>
let inline xVal (e : #System.Xml.Linq.XElement) = e.Value
///<summary>xAttrVal : XAttribute -> string</summary>
let inline xAttrVal (a : #System.Xml.Linq.XAttribute) = a.Value

// XPath

///<summary>xPathElement : expression -> XNode -> XElement option</summary>
let xPathElement expression (n : #System.Xml.Linq.XNode) = 
    System.Xml.XPath.Extensions.XPathSelectElement (n, expression) |> nullGuard

///<summary>xPathElements : expression -> XNode -> XElement list</summary>
let xPathElements expression (n : #System.Xml.Linq.XNode) = 
    System.Xml.XPath.Extensions.XPathSelectElements (n, expression) |> Seq.toList


//let doc = System.Xml.Linq.XDocument.Parse "<foo><bar baz='br' bazn='bz' /><bar baz='br2' bazn='bz2' /></foo>"
//
//doc |> xElement "foo" >>= xElement "bar" 
//
//
//doc |> xPathElement "/foo/bar"
//doc |> xElement "foo" 
