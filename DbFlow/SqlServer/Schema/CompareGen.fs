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
                       |> Compare.collectList x0.TableTypes x1.TableTypes SortOrder.orderBy ElementId.elementId CompareGen.Collect ("TableTypes" :: path)
                       |> Compare.collectList x0.Procedures x1.Procedures SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Procedures" :: path)
                       |> Compare.collectList x0.XmlSchemaCollections x1.XmlSchemaCollections SortOrder.orderBy ElementId.elementId CompareGen.Collect ("XmlSchemaCollections" :: path)
                       |> Compare.collectList x0.Triggers x1.Triggers SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Triggers" :: path)
                       |> Compare.collectList x0.Synonyms x1.Synonyms SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Synonyms" :: path)
                       |> Compare.collectList x0.Sequences x1.Sequences SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Sequences" :: path)
                       |> CompareGen.Collect (x0.Properties, x1.Properties) ("Properties" :: path)

        static member Collect (x0 : Schema, x1 : Schema) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.PrincipalName, x1.PrincipalName) ("PrincipalName" :: path)
                       |> CompareGen.Collect (x0.IsSystemSchema, x1.IsSystemSchema) ("IsSystemSchema" :: path)

        static member Collect (x0 : System.String, x1 : System.String) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : System.Int32, x1 : System.Int32) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : System.Boolean, x1 : System.Boolean) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : TABLE, x1 : TABLE) =
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
                       |> CompareGen.Collect (x0.IsAnsiPadded, x1.IsAnsiPadded) ("IsAnsiPadded" :: path)
                       |> Compare.collectOption x0.ComputedDefinition x1.ComputedDefinition CompareGen.Collect ("ComputedDefinition" :: path)
                       |> Compare.collectOption x0.IdentityDefinition x1.IdentityDefinition Compare.identityDefinition ("IdentityDefinition" :: path)
                       |> Compare.collectOption x0.MaskingFunction x1.MaskingFunction CompareGen.Collect ("MaskingFunction" :: path)
                       |> CompareGen.Collect (x0.IsRowguidcol, x1.IsRowguidcol) ("IsRowguidcol" :: path)

        static member Collect (x0 : Datatype, x1 : Datatype) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.SystemTypeId, x1.SystemTypeId) ("SystemTypeId" :: path)
                       |> CompareGen.Collect (x0.Parameter, x1.Parameter) ("Parameter" :: path)
                       |> CompareGen.Collect (x0.IsUserDefined, x1.IsUserDefined) ("IsUserDefined" :: path)
                       |> Compare.collectOption x0.SystemDatatype x1.SystemDatatype Compare.equalCollector ("SystemDatatype" :: path)
                       |> Compare.collectOption x0.TableDatatype x1.TableDatatype CompareGen.Collect ("TableDatatype" :: path)

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

        static member Collect (x0 : TableDatatype, x1 : TableDatatype) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)

        static member Collect (x0 : ComputedDefinition, x1 : ComputedDefinition) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.ComputedDefinition, x1.ComputedDefinition) ("ComputedDefinition" :: path)
                       |> CompareGen.Collect (x0.IsPersisted, x1.IsPersisted) ("IsPersisted" :: path)

        static member Collect (x0 : INDEX, x1 : INDEX) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.parent, x1.parent) ("parent" :: path)
                       |> Compare.indexName (x0, x1) path
                       |> Compare.equalCollector (x0.index_type, x1.index_type) ("index_type" :: path)
                       |> CompareGen.Collect (x0.is_unique, x1.is_unique) ("is_unique" :: path)
                       |> CompareGen.Collect (x0.data_space_id, x1.data_space_id) ("data_space_id" :: path)
                       |> CompareGen.Collect (x0.ignore_dup_key, x1.ignore_dup_key) ("ignore_dup_key" :: path)
                       |> CompareGen.Collect (x0.is_primary_key, x1.is_primary_key) ("is_primary_key" :: path)
                       |> CompareGen.Collect (x0.is_unique_constraint, x1.is_unique_constraint) ("is_unique_constraint" :: path)
                       |> CompareGen.Collect (x0.fill_factor, x1.fill_factor) ("fill_factor" :: path)
                       |> CompareGen.Collect (x0.is_padded, x1.is_padded) ("is_padded" :: path)
                       |> CompareGen.Collect (x0.is_disabled, x1.is_disabled) ("is_disabled" :: path)
                       |> CompareGen.Collect (x0.is_hypothetical, x1.is_hypothetical) ("is_hypothetical" :: path)
                       |> CompareGen.Collect (x0.allow_row_locks, x1.allow_row_locks) ("allow_row_locks" :: path)
                       |> CompareGen.Collect (x0.allow_page_locks, x1.allow_page_locks) ("allow_page_locks" :: path)
                       |> Compare.collectOption x0.filter x1.filter CompareGen.Collect ("filter" :: path)
                       |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("columns" :: path)

        static member Collect (x0 : INDEX_COLUMN, x1 : INDEX_COLUMN) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)
                       |> CompareGen.Collect (x0.index_column_id, x1.index_column_id) ("index_column_id" :: path)
                       |> CompareGen.Collect (x0.column, x1.column) ("column" :: path)
                       |> Compare.collectOption x0.key_ordinal x1.key_ordinal CompareGen.Collect ("key_ordinal" :: path)
                       |> Compare.collectOption x0.partition_ordinal x1.partition_ordinal CompareGen.Collect ("partition_ordinal" :: path)
                       |> CompareGen.Collect (x0.is_descending_key, x1.is_descending_key) ("is_descending_key" :: path)
                       |> CompareGen.Collect (x0.is_included_column, x1.is_included_column) ("is_included_column" :: path)

        static member Collect (x0 : Trigger, x1 : Trigger) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.Parent, x1.Parent) ("Parent" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> CompareGen.Collect (x0.IsDisabled, x1.IsDisabled) ("IsDisabled" :: path)
                       |> CompareGen.Collect (x0.IsInsteadOfTrigger, x1.IsInsteadOfTrigger) ("IsInsteadOfTrigger" :: path)

        static member Collect (x0 : FOREIGN_KEY, x1 : FOREIGN_KEY) =
                    fun path diffs ->
                       diffs
                       |> Compare.foreignKeyName (x0, x1) path
                       |> CompareGen.Collect (x0.parent, x1.parent) ("parent" :: path)
                       |> CompareGen.Collect (x0.referenced, x1.referenced) ("referenced" :: path)
                       |> CompareGen.Collect (x0.is_disabled, x1.is_disabled) ("is_disabled" :: path)
                       |> Compare.equalCollector (x0.DeleteReferentialAction, x1.DeleteReferentialAction) ("DeleteReferentialAction" :: path)
                       |> Compare.equalCollector (x0.UpdateReferentialAction, x1.UpdateReferentialAction) ("UpdateReferentialAction" :: path)
                       |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("columns" :: path)

        static member Collect (x0 : FOREIGN_KEY_COLUMN, x1 : FOREIGN_KEY_COLUMN) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.constraint_object, x1.constraint_object) ("constraint_object" :: path)
                       |> CompareGen.Collect (x0.constraint_column_id, x1.constraint_column_id) ("constraint_column_id" :: path)
                       |> CompareGen.Collect (x0.parent_column, x1.parent_column) ("parent_column" :: path)
                       |> CompareGen.Collect (x0.referenced_column, x1.referenced_column) ("referenced_column" :: path)

        static member Collect (x0 : CheckConstraint, x1 : CheckConstraint) =
                    fun path diffs ->
                       diffs
                       |> Compare.checkConstraintName (x0, x1) path
                       |> CompareGen.Collect (x0.parent, x1.parent) ("parent" :: path)
                       |> Compare.collectOption x0.column x1.column CompareGen.Collect ("column" :: path)
                       |> CompareGen.Collect (x0.is_disabled, x1.is_disabled) ("is_disabled" :: path)
                       |> CompareGen.Collect (x0.is_not_for_replication, x1.is_not_for_replication) ("is_not_for_replication" :: path)
                       |> CompareGen.Collect (x0.is_not_trusted, x1.is_not_trusted) ("is_not_trusted" :: path)
                       |> CompareGen.Collect (x0.definition, x1.definition) ("definition" :: path)
                       |> CompareGen.Collect (x0.uses_database_collation, x1.uses_database_collation) ("uses_database_collation" :: path)

        static member Collect (x0 : DefaultConstraint, x1 : DefaultConstraint) =
                    fun path diffs ->
                       diffs
                       |> Compare.defaultConstraintName (x0, x1) path
                       |> CompareGen.Collect (x0.Parent, x1.Parent) ("Parent" :: path)
                       |> CompareGen.Collect (x0.Column, x1.Column) ("Column" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)

        static member Collect (x0 : VIEW, x1 : VIEW) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.collectArray x0.Triggers x1.Triggers SortOrder.orderBy ElementId.elementId CompareGen.Collect ("Triggers" :: path)

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

        static member Collect (x0 : PARAMETER, x1 : PARAMETER) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)
                       |> CompareGen.Collect (x0.name, x1.name) ("name" :: path)
                       |> CompareGen.Collect (x0.parameter_id, x1.parameter_id) ("parameter_id" :: path)
                       |> CompareGen.Collect (x0.data_type, x1.data_type) ("data_type" :: path)

        static member Collect (x0 : XmlSchemaCollection, x1 : XmlSchemaCollection) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.XmlCollectionId, x1.XmlCollectionId) ("XmlCollectionId" :: path)
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)

        static member Collect (x0 : DatabaseTrigger, x1 : DatabaseTrigger) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Definition, x1.Definition) ("Definition" :: path)
                       |> CompareGen.Collect (x0.IsDisabled, x1.IsDisabled) ("IsDisabled" :: path)
                       |> CompareGen.Collect (x0.IsInsteadOfTrigger, x1.IsInsteadOfTrigger) ("IsInsteadOfTrigger" :: path)

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
