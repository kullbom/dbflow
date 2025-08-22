namespace DbFlow

// Should have a better name
type Options = {
    SchemazenCompatibility : bool
    BypassReferenceChecksOnLoad : bool
}

module Options =
    let Default = { SchemazenCompatibility = false; BypassReferenceChecksOnLoad = false; }

