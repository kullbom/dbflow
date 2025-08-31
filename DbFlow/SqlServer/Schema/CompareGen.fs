namespace DbFlow.SqlServer.Schema

open DbFlow

type CompareGen = CompareGenCase
    with

        static member Collect (x0 : DatabaseSchema, x1 : DatabaseSchema) =
                    fun path diffs ->
                       diffs
                       |> Compare.collectList x0.Schemas x1.Schemas SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Schemas" :: path)
                       |> Compare.collectList x0.Tables x1.Tables SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Tables" :: path)
                       |> Compare.collectList x0.Views x1.Views SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Views" :: path)
                       |> Compare.collectList x0.Types x1.Types SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Types" :: path)
                       |> Compare.collectAsList x0.TableTypes x1.TableTypes SortOrder.orderBy ElementId.elementId CompareGen.Collect ("TableTypes" :: path)
                       |> Compare.collectList x0.Procedures x1.Procedures SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Procedures" :: path)
                       |> Compare.collectList x0.XmlSchemaCollections x1.XmlSchemaCollections SortOrder.orderBy ElementId.elementId CompareGen.Collect ("XmlSchemaCollections" :: path)
                       |> Compare.collectList x0.Triggers x1.Triggers SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Triggers" :: path)
                       |> Compare.collectList x0.Synonyms x1.Synonyms SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Synonyms" :: path)
                       |> Compare.collectList x0.Sequences x1.Sequences SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Sequences" :: path)
                       |> CompareGen.Collect (x0.Properties, x1.Properties) ("Properties" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : Schema, x1 : Schema) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.PrincipalName, x1.PrincipalName) ("PrincipalName" :: path)
                       |> CompareGen.Collect (x0.IsSystemSchema, x1.IsSystemSchema) ("IsSystemSchema" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : System.String, x1 : System.String) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : System.Int32, x1 : System.Int32) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : System.Boolean, x1 : System.Boolean) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : Table, x1 : Table) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.collectArray x0.Triggers x1.Triggers SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Triggers" :: path)
                       |> Compare.collectArray x0.ForeignKeys x1.ForeignKeys SortOrder.orderBy ElementId.elementId CompareGen.Collect ("ForeignKeys" :: path)
                       |> Compare.collectArray x0.ReferencedForeignKeys x1.ReferencedForeignKeys SortOrder.orderBy ElementId.elementId CompareGen.Collect ("ReferencedForeignKeys" :: path)
                       |> Compare.collectArray x0.CheckConstraints x1.CheckConstraints SortOrder.orderBy ElementId.elementId CompareGen.Collect ("CheckConstraints" :: path)
                       |> Compare.collectArray x0.DefaultConstraints x1.DefaultConstraints SortOrder.orderBy ElementId.elementId CompareGen.Collect ("DefaultConstraints" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : OBJECT, x1 : OBJECT) =
                    fun path diffs ->
                       diffs
                       |> Compare.objectName (x0, x1) path
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> Compare.equalCollector (x0.ObjectType, x1.ObjectType) ("ObjectType" :: path)

        static member Collect (x0 : System.DateTime, x1 : System.DateTime) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : Column, x1 : Column) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.Datatype, x1.Datatype) ("Datatype" :: path)
                       |> Compare.collectOption x0.ComputedDefinition x1.ComputedDefinition CompareGen.Collect ("ComputedDefinition" :: path)
                       |> Compare.collectOption x0.IdentityDefinition x1.IdentityDefinition Compare.identityDefinition ("IdentityDefinition" :: path)
                       |> Compare.collectOption x0.MaskingFunction x1.MaskingFunction CompareGen.Collect ("MaskingFunction" :: path)
                       |> CompareGen.Collect (x0.IsRowguidcol, x1.IsRowguidcol) ("IsRowguidcol" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : Datatype, x1 : Datatype) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.SystemTypeId, x1.SystemTypeId) ("SystemTypeId" :: path)
                       |> CompareGen.Collect (x0.Parameter, x1.Parameter) ("Parameter" :: path)
                       |> Compare.datatypeSpec (x0.DatatypeSpec, x1.DatatypeSpec) CompareGen.Collect ("DatatypeSpec" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : System.Byte, x1 : System.Byte) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : DatatypeParameter, x1 : DatatypeParameter) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.MaxLength, x1.MaxLength) ("MaxLength" :: path)
                       |> CompareGen.Collect (x0.Precision, x1.Precision) ("Precision" :: path)
                       |> CompareGen.Collect (x0.Scale, x1.Scale) ("Scale" :: path)
                       |> Compare.collectOption x0.CollationName x1.CollationName CompareGen.Collect ("CollationName" :: path)
                       |> CompareGen.Collect (x0.IsNullable, x1.IsNullable) ("IsNullable" :: path)

        static member Collect (x0 : System.Int16, x1 : System.Int16) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : ComputedDefinition, x1 : ComputedDefinition) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.ComputedDefinition, x1.ComputedDefinition) ("ComputedDefinition" :: path)
                       |> CompareGen.Collect (x0.IsPersisted, x1.IsPersisted) ("IsPersisted" :: path)

        static member Collect (x0 : Index, x1 : Index) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Parent, x1.Parent) ("Parent" :: path)
                       |> Compare.indexName (x0, x1) path
                       |> Compare.equalCollector (x0.IndexType, x1.IndexType) ("IndexType" :: path)
                       |> CompareGen.Collect (x0.IsUnique, x1.IsUnique) ("IsUnique" :: path)
                       |> CompareGen.Collect (x0.DataSpaceId, x1.DataSpaceId) ("DataSpaceId" :: path)
                       |> CompareGen.Collect (x0.IgnoreDupKey, x1.IgnoreDupKey) ("IgnoreDupKey" :: path)
                       |> CompareGen.Collect (x0.IsPrimaryKey, x1.IsPrimaryKey) ("IsPrimaryKey" :: path)
                       |> CompareGen.Collect (x0.IsUniqueConstraint, x1.IsUniqueConstraint) ("IsUniqueConstraint" :: path)
                       |> CompareGen.Collect (x0.FillFactor, x1.FillFactor) ("FillFactor" :: path)
                       |> CompareGen.Collect (x0.IsPadded, x1.IsPadded) ("IsPadded" :: path)
                       |> CompareGen.Collect (x0.IsDisabled, x1.IsDisabled) ("IsDisabled" :: path)
                       |> CompareGen.Collect (x0.IsHypothetical, x1.IsHypothetical) ("IsHypothetical" :: path)
                       |> CompareGen.Collect (x0.AllowRowLocks, x1.AllowRowLocks) ("AllowRowLocks" :: path)
                       |> CompareGen.Collect (x0.AllowPageLocks, x1.AllowPageLocks) ("AllowPageLocks" :: path)
                       |> Compare.collectOption x0.Filter x1.Filter CompareGen.Collect ("Filter" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : IndexColumn, x1 : IndexColumn) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.IndexColumnId, x1.IndexColumnId) ("IndexColumnId" :: path)
                       |> CompareGen.Collect (x0.Column, x1.Column) ("Column" :: path)
                       |> Compare.collectOption x0.KeyOrdinal x1.KeyOrdinal CompareGen.Collect ("KeyOrdinal" :: path)
                       |> Compare.collectOption x0.PartitionOrdinal x1.PartitionOrdinal CompareGen.Collect ("PartitionOrdinal" :: path)
                       |> CompareGen.Collect (x0.IsDescendingKey, x1.IsDescendingKey) ("IsDescendingKey" :: path)
                       |> CompareGen.Collect (x0.IsIncludedColumn, x1.IsIncludedColumn) ("IsIncludedColumn" :: path)

        static member Collect (x0 : Trigger, x1 : Trigger) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.Parent, x1.Parent) ("Parent" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> CompareGen.Collect (x0.IsDisabled, x1.IsDisabled) ("IsDisabled" :: path)
                       |> CompareGen.Collect (x0.IsInsteadOfTrigger, x1.IsInsteadOfTrigger) ("IsInsteadOfTrigger" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : ForeignKey, x1 : ForeignKey) =
                    fun path diffs ->
                       diffs
                       |> Compare.foreignKeyName (x0, x1) path
                       |> CompareGen.Collect (x0.Parent, x1.Parent) ("Parent" :: path)
                       |> CompareGen.Collect (x0.Referenced, x1.Referenced) ("Referenced" :: path)
                       |> CompareGen.Collect (x0.IsDisabled, x1.IsDisabled) ("IsDisabled" :: path)
                       |> Compare.equalCollector (x0.DeleteReferentialAction, x1.DeleteReferentialAction) ("DeleteReferentialAction" :: path)
                       |> Compare.equalCollector (x0.UpdateReferentialAction, x1.UpdateReferentialAction) ("UpdateReferentialAction" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : ForeignKeycolumn, x1 : ForeignKeycolumn) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.ConstraintObject, x1.ConstraintObject) ("ConstraintObject" :: path)
                       |> CompareGen.Collect (x0.ConstraintColumnId, x1.ConstraintColumnId) ("ConstraintColumnId" :: path)
                       |> CompareGen.Collect (x0.ParentColumn, x1.ParentColumn) ("ParentColumn" :: path)
                       |> CompareGen.Collect (x0.ReferencedColumn, x1.ReferencedColumn) ("ReferencedColumn" :: path)

        static member Collect (x0 : CheckConstraint, x1 : CheckConstraint) =
                    fun path diffs ->
                       diffs
                       |> Compare.checkConstraintName (x0, x1) path
                       |> CompareGen.Collect (x0.Parent, x1.Parent) ("Parent" :: path)
                       |> Compare.collectOption x0.Column x1.Column CompareGen.Collect ("Column" :: path)
                       |> CompareGen.Collect (x0.IsDisabled, x1.IsDisabled) ("IsDisabled" :: path)
                       |> CompareGen.Collect (x0.IsNotForReplication, x1.IsNotForReplication) ("IsNotForReplication" :: path)
                       |> CompareGen.Collect (x0.IsNotTrusted, x1.IsNotTrusted) ("IsNotTrusted" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> CompareGen.Collect (x0.UsesDatabaseCollation, x1.UsesDatabaseCollation) ("UsesDatabaseCollation" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : DefaultConstraint, x1 : DefaultConstraint) =
                    fun path diffs ->
                       diffs
                       |> Compare.defaultConstraintName (x0, x1) path
                       |> CompareGen.Collect (x0.Parent, x1.Parent) ("Parent" :: path)
                       |> CompareGen.Collect (x0.Column, x1.Column) ("Column" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : View, x1 : View) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.collectArray x0.Triggers x1.Triggers SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Triggers" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : TableType, x1 : TableType) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.collectArray x0.ForeignKeys x1.ForeignKeys SortOrder.orderBy ElementId.elementId CompareGen.Collect ("ForeignKeys" :: path)
                       |> Compare.collectArray x0.ReferencedForeignKeys x1.ReferencedForeignKeys SortOrder.orderBy ElementId.elementId CompareGen.Collect ("ReferencedForeignKeys" :: path)
                       |> Compare.collectArray x0.CheckConstraints x1.CheckConstraints SortOrder.orderBy ElementId.elementId CompareGen.Collect ("CheckConstraints" :: path)
                       |> Compare.collectArray x0.DefaultConstraints x1.DefaultConstraints SortOrder.orderBy ElementId.elementId CompareGen.Collect ("DefaultConstraints" :: path)

        static member Collect (x0 : Procedure, x1 : Procedure) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> Compare.collectArray x0.Parameters x1.Parameters SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Parameters" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Columns" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : Parameter, x1 : Parameter) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.ParameterId, x1.ParameterId) ("ParameterId" :: path)
                       |> CompareGen.Collect (x0.Datatype, x1.Datatype) ("Datatype" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : XmlSchemaCollection, x1 : XmlSchemaCollection) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.XmlCollectionId, x1.XmlCollectionId) ("XmlCollectionId" :: path)
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : DatabaseTrigger, x1 : DatabaseTrigger) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> CompareGen.Collect (x0.IsDisabled, x1.IsDisabled) ("IsDisabled" :: path)
                       |> CompareGen.Collect (x0.IsInsteadOfTrigger, x1.IsInsteadOfTrigger) ("IsInsteadOfTrigger" :: path)
                       |> Compare.xProperties (x0.XProperties, x1.XProperties) ("XProperties" :: path)

        static member Collect (x0 : Synonym, x1 : Synonym) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.BaseObjectName, x1.BaseObjectName) ("BaseObjectName" :: path)

        static member Collect (x0 : Sequence, x1 : Sequence) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> Compare.sequenceDefinition (x0.SequenceDefinition, x1.SequenceDefinition) ("SequenceDefinition" :: path)
                       |> CompareGen.Collect (x0.IsCycling, x1.IsCycling) ("IsCycling" :: path)
                       |> CompareGen.Collect (x0.IsCached, x1.IsCached) ("IsCached" :: path)
                       |> Compare.collectOption x0.CacheSize x1.CacheSize CompareGen.Collect ("CacheSize" :: path)
                       |> CompareGen.Collect (x0.Datatype, x1.Datatype) ("Datatype" :: path)
                       |> CompareGen.Collect (x0.IsExhausted, x1.IsExhausted) ("IsExhausted" :: path)

        static member Collect (x0 : DatabaseProperties, x1 : DatabaseProperties) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.compatibility_level, x1.compatibility_level) ("compatibility_level" :: path)
                       |> CompareGen.Collect (x0.collation_name, x1.collation_name) ("collation_name" :: path)
                       |> CompareGen.Collect (x0.is_auto_close_on, x1.is_auto_close_on) ("is_auto_close_on" :: path)
                       |> CompareGen.Collect (x0.is_auto_shrink_on, x1.is_auto_shrink_on) ("is_auto_shrink_on" :: path)
                       |> CompareGen.Collect (x0.snapshot_isolation_state, x1.snapshot_isolation_state) ("snapshot_isolation_state" :: path)
                       |> CompareGen.Collect (x0.is_read_committed_snapshot_on, x1.is_read_committed_snapshot_on) ("is_read_committed_snapshot_on" :: path)
                       |> CompareGen.Collect (x0.recovery_model, x1.recovery_model) ("recovery_model" :: path)
                       |> CompareGen.Collect (x0.page_verify_option, x1.page_verify_option) ("page_verify_option" :: path)
                       |> CompareGen.Collect (x0.is_auto_create_stats_on, x1.is_auto_create_stats_on) ("is_auto_create_stats_on" :: path)
                       |> CompareGen.Collect (x0.is_auto_update_stats_on, x1.is_auto_update_stats_on) ("is_auto_update_stats_on" :: path)
                       |> CompareGen.Collect (x0.is_auto_update_stats_async_on, x1.is_auto_update_stats_async_on) ("is_auto_update_stats_async_on" :: path)
                       |> CompareGen.Collect (x0.is_ansi_null_default_on, x1.is_ansi_null_default_on) ("is_ansi_null_default_on" :: path)
                       |> CompareGen.Collect (x0.is_ansi_nulls_on, x1.is_ansi_nulls_on) ("is_ansi_nulls_on" :: path)
                       |> CompareGen.Collect (x0.is_ansi_padding_on, x1.is_ansi_padding_on) ("is_ansi_padding_on" :: path)
                       |> CompareGen.Collect (x0.is_ansi_warnings_on, x1.is_ansi_warnings_on) ("is_ansi_warnings_on" :: path)
                       |> CompareGen.Collect (x0.is_arithabort_on, x1.is_arithabort_on) ("is_arithabort_on" :: path)
                       |> CompareGen.Collect (x0.is_concat_null_yields_null_on, x1.is_concat_null_yields_null_on) ("is_concat_null_yields_null_on" :: path)
                       |> CompareGen.Collect (x0.is_numeric_roundabort_on, x1.is_numeric_roundabort_on) ("is_numeric_roundabort_on" :: path)
                       |> CompareGen.Collect (x0.is_quoted_identifier_on, x1.is_quoted_identifier_on) ("is_quoted_identifier_on" :: path)
                       |> CompareGen.Collect (x0.is_recursive_triggers_on, x1.is_recursive_triggers_on) ("is_recursive_triggers_on" :: path)
                       |> CompareGen.Collect (x0.is_cursor_close_on_commit_on, x1.is_cursor_close_on_commit_on) ("is_cursor_close_on_commit_on" :: path)
                       |> CompareGen.Collect (x0.is_local_cursor_default, x1.is_local_cursor_default) ("is_local_cursor_default" :: path)
                       |> CompareGen.Collect (x0.is_trustworthy_on, x1.is_trustworthy_on) ("is_trustworthy_on" :: path)
                       |> CompareGen.Collect (x0.is_db_chaining_on, x1.is_db_chaining_on) ("is_db_chaining_on" :: path)
                       |> CompareGen.Collect (x0.is_parameterization_forced, x1.is_parameterization_forced) ("is_parameterization_forced" :: path)
                       |> CompareGen.Collect (x0.is_date_correlation_on, x1.is_date_correlation_on) ("is_date_correlation_on" :: path)
