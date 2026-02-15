namespace DbFlow

type ReadOptions = {
    CheckReferencesOnLoad : bool
    RefreshViewMetadata : bool
}
    with
        static member Default = { CheckReferencesOnLoad = true; RefreshViewMetadata = false }

// Should have a better name - and should probably be specific for db vendor
type ScriptOptions = {
    SkipCompatibilityLevel : bool
    TypenameFormatter : string -> string
}
    with 
        static member defaultTypenameFormatter (s : string) = s.ToUpperInvariant()
        static member Default = { SkipCompatibilityLevel = false; TypenameFormatter = ScriptOptions.defaultTypenameFormatter }

