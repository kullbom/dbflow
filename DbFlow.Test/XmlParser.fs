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
let inline xElementOptional path (c : #System.Xml.Linq.XContainer) = xElementOptional' c (XP path)

///<summary>xElements : name -> XContainer -> XElement list    
/// ('name' is string OR string * string)</summary>
let inline xElements path (c : #System.Xml.Linq.XContainer) = xElements' c (XN path)

///<summary>xAttr : name -> XElement -> XAttribute option    
/// ('name' is string OR string * string)</summary>
let inline xAttr name (e : #System.Xml.Linq.XElement) = 
    e.Attribute 
        (match XN name with
         | Name name -> System.Xml.Linq.XName.Get (name)
         | NSName (name, ns) -> System.Xml.Linq.XName.Get (name, ns))
    |> nullGuard

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


let parseBoolean (s : string) = 
    match s with
    | "true" | "1" -> Ok true
    | "false" | "0" -> Ok false
    | _ -> Result.Errorf "Invalid boolean value: %s" s

//let doc = System.Xml.Linq.XDocument.Parse "<foo><bar baz='br' bazn='bz' /><bar baz='br2' bazn='bz2' /></foo>"
//
//doc |> xElement "foo" >>= xElement "bar" 
//
//
//doc |> xPathElement "/foo/bar"
//doc |> xElement "foo" 
