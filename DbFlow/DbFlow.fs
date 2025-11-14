namespace DbFlow

// Should have a better name - and should probably be specific for db vendor
type Options = {
    BypassReferenceChecksOnLoad : bool
    SkipCompatibilityLevel : bool
}

module Options =
    let Default = { BypassReferenceChecksOnLoad = false; SkipCompatibilityLevel = false }

