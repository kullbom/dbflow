namespace DbFlow

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
    TypenameFormatter : string -> string
}
    with 
        static member defaultTypenameFormatter (s : string) = s.ToUpperInvariant()
        static member Default = 
                { 
                    SkipCompatibilityLevel = false; 
                    SkipUnsupportedExtendedProperties = false; 
                    TypenameFormatter = ScriptOptions.defaultTypenameFormatter 
                }

