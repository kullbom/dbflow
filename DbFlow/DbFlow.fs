namespace DbFlow

type Options = {
    SchemazenCompatibility : bool
    BypassReferenceChecksOnLoad : bool
}

module Options =
    let Default = { SchemazenCompatibility = false; BypassReferenceChecksOnLoad = false; }

