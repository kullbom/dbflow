namespace DbFlow

// Should have a better name
type Options = {
    BypassReferenceChecksOnLoad : bool
}

module Options =
    let Default = { BypassReferenceChecksOnLoad = false; }

