namespace DbFlow.SqlServer.Schema

open DbFlow

type CompareGen = CompareGenCase
    with

        static member Collect (x0 : DatabaseSchema, x1 : DatabaseSchema) =
                    fun path diffs ->
                       diffs