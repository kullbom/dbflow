namespace DbFlow

// Should have a better name
type Options = {
    BypassReferenceChecksOnLoad : bool
    SkipCompatibilityLevel : bool
}

module Options =
    let Default = { BypassReferenceChecksOnLoad = false; SkipCompatibilityLevel = false }

