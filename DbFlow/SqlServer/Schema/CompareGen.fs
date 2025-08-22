namespace DbFlow.SqlServer.Schema

open DbFlow

type CompareGen = CompareGenCase
    with

        static member Collect (x0 : DatabaseSchema, x1 : DatabaseSchema) =
                    fun path diffs ->
                       diffs
                       |> Compare.collectList x0.Schemas x1.Schemas SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Schemas" :: path)
                       |> Compare.collectList x0.Tables x1.Tables SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Tables" :: path)
                       |> Compare.collectList x0.Views x1.Views SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Views" :: path)
                       |> Compare.collectList x0.Types x1.Types SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Types" :: path)
                       |> Compare.collectList x0.TableTypes x1.TableTypes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("TableTypes" :: path)
                       |> Compare.collectList x0.Procedures x1.Procedures SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Procedures" :: path)
                       |> Compare.collectList x0.XmlSchemaCollections x1.XmlSchemaCollections SortOrder.orderBy Sequence.elementId CompareGen.Collect ("XmlSchemaCollections" :: path)
                       |> Compare.collectList x0.Triggers x1.Triggers SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Triggers" :: path)
                       |> Compare.collectList x0.Synonyms x1.Synonyms SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Synonyms" :: path)
                       |> Compare.collectList x0.Sequences x1.Sequences SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Sequences" :: path)
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
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.collectArray x0.Triggers x1.Triggers SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Triggers" :: path)
                       |> Compare.collectArray x0.ForeignKeys x1.ForeignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("ForeignKeys" :: path)
                       |> Compare.collectArray x0.ReferencedForeignKeys x1.ReferencedForeignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("ReferencedForeignKeys" :: path)
                       |> Compare.collectArray x0.CheckConstraints x1.CheckConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("CheckConstraints" :: path)
                       |> Compare.collectArray x0.DefaultConstraints x1.DefaultConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("DefaultConstraints" :: path)

        static member Collect (x0 : OBJECT, x1 : OBJECT) =
                    fun path diffs ->
                       diffs
                       |> Compare.object_name (x0, x1) path
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> Compare.equalCollector (x0.ObjectType, x1.ObjectType) ("ObjectType" :: path)

        static member Collect (x0 : System.DateTime, x1 : System.DateTime) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : COLUMN, x1 : COLUMN) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.column_name, x1.column_name) ("column_name" :: path)
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)
                       |> CompareGen.Collect (x0.data_type, x1.data_type) ("data_type" :: path)
                       |> CompareGen.Collect (x0.is_ansi_padded, x1.is_ansi_padded) ("is_ansi_padded" :: path)
                       |> Compare.collectOption x0.computed_definition x1.computed_definition CompareGen.Collect ("computed_definition" :: path)
                       |> Compare.collectOption x0.identity_definition x1.identity_definition Compare.identity_definition ("identity_definition" :: path)
                       |> Compare.collectOption x0.masking_function x1.masking_function CompareGen.Collect ("masking_function" :: path)
                       |> CompareGen.Collect (x0.is_rowguidcol, x1.is_rowguidcol) ("is_rowguidcol" :: path)

        static member Collect (x0 : Datatype, x1 : Datatype) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.name, x1.name) ("name" :: path)
                       |> CompareGen.Collect (x0.schema, x1.schema) ("schema" :: path)
                       |> CompareGen.Collect (x0.system_type_id, x1.system_type_id) ("system_type_id" :: path)
                       |> CompareGen.Collect (x0.parameter, x1.parameter) ("parameter" :: path)
                       |> CompareGen.Collect (x0.is_user_defined, x1.is_user_defined) ("is_user_defined" :: path)
                       |> Compare.collectOption x0.sys_datatype x1.sys_datatype Compare.equalCollector ("sys_datatype" :: path)
                       |> Compare.collectOption x0.table_datatype x1.table_datatype CompareGen.Collect ("table_datatype" :: path)

        static member Collect (x0 : System.Byte, x1 : System.Byte) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : DatatypeParameter, x1 : DatatypeParameter) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.max_length, x1.max_length) ("max_length" :: path)
                       |> CompareGen.Collect (x0.precision, x1.precision) ("precision" :: path)
                       |> CompareGen.Collect (x0.scale, x1.scale) ("scale" :: path)
                       |> Compare.collectOption x0.collation_name x1.collation_name CompareGen.Collect ("collation_name" :: path)
                       |> CompareGen.Collect (x0.is_nullable, x1.is_nullable) ("is_nullable" :: path)

        static member Collect (x0 : System.Int16, x1 : System.Int16) = Compare.equalCollector (x0, x1)

        static member Collect (x0 : TableDatatype, x1 : TableDatatype) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)

        static member Collect (x0 : COMPUTED_DEFINITION, x1 : COMPUTED_DEFINITION) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.computed_definition, x1.computed_definition) ("computed_definition" :: path)
                       |> CompareGen.Collect (x0.is_persisted, x1.is_persisted) ("is_persisted" :: path)

        static member Collect (x0 : INDEX, x1 : INDEX) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.parent, x1.parent) ("parent" :: path)
                       |> Compare.index_name (x0, x1) path
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
                       |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)

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

        static member Collect (x0 : TRIGGER, x1 : TRIGGER) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)
                       |> CompareGen.Collect (x0.parent, x1.parent) ("parent" :: path)
                       |> CompareGen.Collect (x0.trigger_name, x1.trigger_name) ("trigger_name" :: path)
                       |> CompareGen.Collect (x0.orig_definition, x1.orig_definition) ("orig_definition" :: path)
                       |> CompareGen.Collect (x0.definition, x1.definition) ("definition" :: path)
                       |> CompareGen.Collect (x0.is_disabled, x1.is_disabled) ("is_disabled" :: path)
                       |> CompareGen.Collect (x0.is_instead_of_trigger, x1.is_instead_of_trigger) ("is_instead_of_trigger" :: path)

        static member Collect (x0 : FOREIGN_KEY, x1 : FOREIGN_KEY) =
                    fun path diffs ->
                       diffs
                       |> Compare.foreign_key_name (x0, x1) path
                       |> CompareGen.Collect (x0.parent, x1.parent) ("parent" :: path)
                       |> CompareGen.Collect (x0.referenced, x1.referenced) ("referenced" :: path)
                       |> CompareGen.Collect (x0.is_disabled, x1.is_disabled) ("is_disabled" :: path)
                       |> Compare.equalCollector (x0.delete_referential_action, x1.delete_referential_action) ("delete_referential_action" :: path)
                       |> Compare.equalCollector (x0.update_referential_action, x1.update_referential_action) ("update_referential_action" :: path)
                       |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)

        static member Collect (x0 : FOREIGN_KEY_COLUMN, x1 : FOREIGN_KEY_COLUMN) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.constraint_object, x1.constraint_object) ("constraint_object" :: path)
                       |> CompareGen.Collect (x0.constraint_column_id, x1.constraint_column_id) ("constraint_column_id" :: path)
                       |> CompareGen.Collect (x0.parent_column, x1.parent_column) ("parent_column" :: path)
                       |> CompareGen.Collect (x0.referenced_column, x1.referenced_column) ("referenced_column" :: path)

        static member Collect (x0 : CHECK_CONSTRAINT, x1 : CHECK_CONSTRAINT) =
                    fun path diffs ->
                       diffs
                       |> Compare.check_constraint_name (x0, x1) path
                       |> CompareGen.Collect (x0.parent, x1.parent) ("parent" :: path)
                       |> Compare.collectOption x0.column x1.column CompareGen.Collect ("column" :: path)
                       |> CompareGen.Collect (x0.is_disabled, x1.is_disabled) ("is_disabled" :: path)
                       |> CompareGen.Collect (x0.is_not_for_replication, x1.is_not_for_replication) ("is_not_for_replication" :: path)
                       |> CompareGen.Collect (x0.is_not_trusted, x1.is_not_trusted) ("is_not_trusted" :: path)
                       |> CompareGen.Collect (x0.definition, x1.definition) ("definition" :: path)
                       |> CompareGen.Collect (x0.uses_database_collation, x1.uses_database_collation) ("uses_database_collation" :: path)

        static member Collect (x0 : DEFAULT_CONSTRAINT, x1 : DEFAULT_CONSTRAINT) =
                    fun path diffs ->
                       diffs
                       |> Compare.default_constraint_name (x0, x1) path
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
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.collectArray x0.Triggers x1.Triggers SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Triggers" :: path)

        static member Collect (x0 : TableType, x1 : TableType) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.Schema, x1.Schema) ("Schema" :: path)
                       |> CompareGen.Collect (x0.Name, x1.Name) ("Name" :: path)
                       |> CompareGen.Collect (x0.Object, x1.Object) ("Object" :: path)
                       |> Compare.collectArray x0.Columns x1.Columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Columns" :: path)
                       |> Compare.collectArray x0.Indexes x1.Indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("Indexes" :: path)
                       |> Compare.collectArray x0.ForeignKeys x1.ForeignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("ForeignKeys" :: path)
                       |> Compare.collectArray x0.ReferencedForeignKeys x1.ReferencedForeignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("ReferencedForeignKeys" :: path)
                       |> Compare.collectArray x0.CheckConstraints x1.CheckConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("CheckConstraints" :: path)
                       |> Compare.collectArray x0.DefaultConstraints x1.DefaultConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("DefaultConstraints" :: path)

        static member Collect (x0 : PROCEDURE, x1 : PROCEDURE) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)
                       |> CompareGen.Collect (x0.name, x1.name) ("name" :: path)
                       |> Compare.collectArray x0.parameters x1.parameters SortOrder.orderBy Sequence.elementId CompareGen.Collect ("parameters" :: path)
                       |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)
                       |> CompareGen.Collect (x0.orig_definition, x1.orig_definition) ("orig_definition" :: path)
                       |> CompareGen.Collect (x0.definition, x1.definition) ("definition" :: path)
                       |> Compare.collectArray x0.indexes x1.indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("indexes" :: path)

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

        static member Collect (x0 : DATABASE_TRIGGER, x1 : DATABASE_TRIGGER) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.trigger_name, x1.trigger_name) ("trigger_name" :: path)
                       |> CompareGen.Collect (x0.definition, x1.definition) ("definition" :: path)
                       |> CompareGen.Collect (x0.is_disabled, x1.is_disabled) ("is_disabled" :: path)
                       |> CompareGen.Collect (x0.is_instead_of_trigger, x1.is_instead_of_trigger) ("is_instead_of_trigger" :: path)

        static member Collect (x0 : SYNONYM, x1 : SYNONYM) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)
                       |> CompareGen.Collect (x0.base_object_name, x1.base_object_name) ("base_object_name" :: path)

        static member Collect (x0 : SEQUENCE, x1 : SEQUENCE) =
                    fun path diffs ->
                       diffs
                       |> CompareGen.Collect (x0.object, x1.object) ("object" :: path)
                       |> Compare.sequence_definition (x0.sequence_definition, x1.sequence_definition) ("sequence_definition" :: path)
                       |> CompareGen.Collect (x0.is_cycling, x1.is_cycling) ("is_cycling" :: path)
                       |> CompareGen.Collect (x0.is_cached, x1.is_cached) ("is_cached" :: path)
                       |> Compare.collectOption x0.cache_size x1.cache_size CompareGen.Collect ("cache_size" :: path)
                       |> CompareGen.Collect (x0.data_type, x1.data_type) ("data_type" :: path)
                       |> CompareGen.Collect (x0.is_exhausted, x1.is_exhausted) ("is_exhausted" :: path)

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
