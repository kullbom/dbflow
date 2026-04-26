module DbFlow.XmlParser

[<Struct>]
type PResult<'T, 'TError> =
    | POk of ok: 'T
    | Failure of error: 'TError
    
module PResult =
    let map f = function
        | POk x -> POk (f x)
        | Failure e -> Failure e

    let bind f = function
        | POk x -> f x
        | Failure e -> Failure e

    type CEBuilder() =
        member inline b.Return (x)        = POk x
        member inline b.Bind (p, rest)    = p |> bind rest
        member inline b.Let (p, rest)     = rest p
        member inline b.ReturnFrom (expr) = expr

    let builder = new CEBuilder()

let Failf fmt = 
    //failwithf fmt
    Printf.ksprintf Failure fmt

let forAll (f : 'a -> PResult<'b, _>) (xs : 'a list) : PResult<'b list, _> =
    let rec forAll' acc xs =
        match xs with
        | [] -> POk (List.rev acc)
        | x :: xs ->
            match f x with
            | POk y -> forAll' (y :: acc) xs
            | Failure e -> Failure e
    forAll' [] xs

type XName =
      Name of string
    | NSName of string * string 

type XNameResolver = XNameResolver  with
    static member inline ($) (XNameResolver , (name : string)) = XName.Name name
    
    static member inline ($) (XNameResolver , (name : string, ns : string)) = XName.NSName (name, ns)
    
let inline XN selector : XName = XNameResolver $ selector


let nullGuard = function null -> None | x -> Some x

let xElementRequire' (c : #System.Xml.Linq.XContainer) n =
    match n with
    | Name name -> 
        match c.Element (System.Xml.Linq.XName.Get (name)) with 
        | null -> Failf "Expected %s not found" name 
        | e -> POk e 
        
    | NSName (name, ns) -> 
        match c.Element (System.Xml.Linq.XName.Get (name,ns)) with 
        | null -> Failf "Expected %s (ns: %s) not found" name ns
        | e -> POk e 
        

let xElementOptional' (c : #System.Xml.Linq.XContainer) n =
    match n with 
    | Name name -> 
        c.Element (System.Xml.Linq.XName.Get (name)) |> nullGuard |> POk
    | NSName (name, ns) -> 
        c.Element (System.Xml.Linq.XName.Get (name,ns)) |> nullGuard |> POk
           

let xElements' (c : #System.Xml.Linq.XContainer) =
    function Name name -> c.Elements (System.Xml.Linq.XName.Get (name)) |> Seq.toList
           | NSName (name, ns) -> c.Elements (System.Xml.Linq.XName.Get (name,ns)) |> Seq.toList

// Simple navigation

let inline xElementReqP path parser (c : #System.Xml.Linq.XContainer) = 
    match xElementRequire' c (XN path) with
    | POk v -> parser v 
    | Failure e -> Failure e

let inline xElementP path parser (c : #System.Xml.Linq.XContainer) = 
    match xElementOptional' c (XN path) with
    | POk (Some e) -> parser e |> PResult.map Some
    | POk None -> POk None
    | Failure e -> Failure e

let inline xElementsP name parser (c : #System.Xml.Linq.XContainer) = 
    xElements' c (XN name) |> forAll parser

let xElementsAllP parser (c : #System.Xml.Linq.XContainer) = 
    c.Elements () |> Seq.toList |> forAll parser


module XmlReaders =
    let readBool (s : string) = 
        match s with
        | "true" | "1" -> POk true
        | "false" | "0" -> POk false
        | _ -> Failf "Invalid boolean value: %s" s

    let readString  (s : string) : PResult<string, string> = s |> POk
    let readInt32   (s : string) = 
        match System.Int32.TryParse s with
        | true, v -> POk v
        | false, _ -> Failf "Invalid int32 value: %s" s

    let readUInt64  (s : string) = 
        match System.UInt64.TryParse s with
        | true, v -> POk v
        | false, _ -> Failf "Invalid uint64 value: %s" s

    let readFloat   (s : string) = 
        match System.Double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with
        | true, v -> POk v
        | false, _ -> Failf "Invalid float value: %s" s

type public XmlTypeMapping = 
    static member inline ($) (_:XmlTypeMapping,_:bool)   = XmlReaders.readBool     
    static member inline ($) (_:XmlTypeMapping,_:string) = XmlReaders.readString
    static member inline ($) (_:XmlTypeMapping,_:int32)  = XmlReaders.readInt32
    static member inline ($) (_:XmlTypeMapping,_:uint64) = XmlReaders.readUInt64
    static member inline ($) (_:XmlTypeMapping,_:float)  = XmlReaders.readFloat

let inline resolveAttr (a : string) : PResult< ^R,string> = (Unchecked.defaultof<XmlTypeMapping> $ (Unchecked.defaultof< ^R>)) a

let inline xAttr' name (e : #System.Xml.Linq.XElement) = 
    e.Attribute 
        (match XN name with
         | Name name -> System.Xml.Linq.XName.Get (name)
         | NSName (name, ns) -> System.Xml.Linq.XName.Get (name, ns))
    |> nullGuard
    |> Option.map (fun a -> a.Value)
    
/// Reads and resolves an attribute, returning None if the attribute is not present, or an error if the value cannot be resolved to the expected type
let inline xAttr name (e : #System.Xml.Linq.XElement) = 
    match xAttr' name e with
    | Some v -> resolveAttr v |> PResult.map Some 
    | None -> POk None

/// Reads and resolves an attribute, returning None if the attribute is not present, or an error if the value cannot be resolved/parsed to the expected type
let inline xAttrP name attrParser (e : #System.Xml.Linq.XElement) = 
    match xAttr' name e with
    | Some v -> resolveAttr v |> PResult.bind attrParser |> PResult.map Some 
    | None -> POk None

/// Reads and resolves an attribute, returning an error if the attribute is not present or if the value cannot be resolved to the expected type
let inline xAttrReq name (e : #System.Xml.Linq.XElement) = 
    match xAttr' name e with
    | Some v -> resolveAttr v
    | None -> 
        Failf "Expected attribute %A not found" name

/// Reads and resolves an attribute, returning an error if the attribute is not present or if the value cannot be resolved/parsed to the expected type
let inline xAttrReqP name parser (e : #System.Xml.Linq.XElement) = 
    match xAttr' name e with
    | Some v -> resolveAttr v |> PResult.bind parser
    | None -> 
        Failf "Expected attribute %A not found" name

