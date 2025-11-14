namespace DbFlow

// Should have a better name - and should probably be specific for db vendor
type Options = {
    BypassReferenceChecksOnLoad : bool
    SkipCompatibilityLevel : bool
    TypenameFormatter : string -> string
}

module Options =
    let defaultTypenameFormatter (s : string) = s.ToUpperInvariant()
    let Default = { BypassReferenceChecksOnLoad = false; SkipCompatibilityLevel = false; TypenameFormatter = defaultTypenameFormatter }

