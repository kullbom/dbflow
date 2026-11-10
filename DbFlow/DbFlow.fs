namespace DbFlow

// Should probably be specific for db vendor
type LocalDbOptions = {
    UseLocalSchemaCache : bool
    DbFolderPath : unit -> string 
    /// A function that given a database name, returns a connection string to connect to the local database server.
    /// Default is: "Server=(LocalDB)\\mssqllocaldb;Initial Catalog={dbName};Integrated Security=true"
    ConnectionString : string ->  string
}
    with
        static member Default = 
                { 
                    UseLocalSchemaCache = false
                    DbFolderPath = fun () -> System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Data")
                    ConnectionString = 
                        fun dbName ->  
                            $"Server=(LocalDB)\\mssqllocaldb;Initial Catalog={dbName};Integrated Security=true"
                }


// Should probably be specific for db vendor
type ReadOptions = {
    CheckReferencesOnLoad : bool
    RefreshViewMetadata : bool
}
    with
        static member Default = { CheckReferencesOnLoad = true; RefreshViewMetadata = false }

// Should probably be specific for db vendor
type ScriptOptions = {
    SkipCompatibilityLevel : bool
    SkipUnsupportedExtendedProperties : bool
    SkipSchemaAuthorization : bool
    TypenameFormatter : string -> string
}
    with 
        static member defaultTypenameFormatter (s : string) = s.ToUpperInvariant()
        static member Default = 
                { 
                    SkipCompatibilityLevel = false; 
                    SkipUnsupportedExtendedProperties = false;
                    SkipSchemaAuthorization = false;
                    TypenameFormatter = ScriptOptions.defaultTypenameFormatter 
                }

