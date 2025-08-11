namespace DbFlow.SqlServer.Scripts

type Script<'a when 'a: comparison> = { 
    directory_name : string; 
    filename : string; 
    content : string; 
    
    contains_objects : 'a Set  
    depends_on : 'a Set

    priority : int
}

type ScriptObjects =
    | SchemaDefinition
    | ObjectDefinitions of {| contains_objects : int list; depends_on : int list |}
    | UserDefinedTypeDefinition
    | XmlSchemaCollectionDefinition
