namespace DbFlow.SqlServer.Schema

open DbFlow

type CompareGen = CompareGenCase
    with

        static member Collect (x0 : DATABASE, x1 : DATABASE, path, diffs) =
                    diffs
                    |> Compare.collectList x0.SCHEMAS x1.SCHEMAS SortOrder.orderBy Sequence.elementId CompareGen.Collect ("SCHEMAS" :: path)
                    |> Compare.collectList x0.TABLES x1.TABLES SortOrder.orderBy Sequence.elementId CompareGen.Collect ("TABLES" :: path)
                    |> Compare.collectList x0.VIEWS x1.VIEWS SortOrder.orderBy Sequence.elementId CompareGen.Collect ("VIEWS" :: path)
                    |> Compare.collectList x0.TYPES x1.TYPES SortOrder.orderBy Sequence.elementId CompareGen.Collect ("TYPES" :: path)
                    |> Compare.collectList x0.TABLE_TYPES x1.TABLE_TYPES SortOrder.orderBy Sequence.elementId CompareGen.Collect ("TABLE_TYPES" :: path)
                    |> Compare.collectList x0.PROCEDURES x1.PROCEDURES SortOrder.orderBy Sequence.elementId CompareGen.Collect ("PROCEDURES" :: path)
                    |> Compare.collectList x0.XML_SCHEMA_COLLECTIONS x1.XML_SCHEMA_COLLECTIONS SortOrder.orderBy Sequence.elementId CompareGen.Collect ("XML_SCHEMA_COLLECTIONS" :: path)
                    |> Compare.collectList x0.TRIGGERS x1.TRIGGERS SortOrder.orderBy Sequence.elementId CompareGen.Collect ("TRIGGERS" :: path)
                    |> Compare.collectList x0.SYNONYMS x1.SYNONYMS SortOrder.orderBy Sequence.elementId CompareGen.Collect ("SYNONYMS" :: path)
                    |> Compare.collectList x0.SEQUENCES x1.SEQUENCES SortOrder.orderBy Sequence.elementId CompareGen.Collect ("SEQUENCES" :: path)

        static member Collect (x0 : SCHEMA, x1 : SCHEMA, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.name, x1.name, "name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.principal_name, x1.principal_name, "principal_name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_system_schema, x1.is_system_schema, "is_system_schema" :: path, diff')

        static member Collect (x0 : System.String, x1 : System.String, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)

        static member Collect (x0 : System.Int32, x1 : System.Int32, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)

        static member Collect (x0 : System.Boolean, x1 : System.Boolean, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)

        static member Collect (x0 : TABLE, x1 : TABLE, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.schema, x1.schema, "schema" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.table_name, x1.table_name, "table_name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)
                    |> Compare.collectArray x0.indexes x1.indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("indexes" :: path)
                    |> Compare.collectArray x0.triggers x1.triggers SortOrder.orderBy Sequence.elementId CompareGen.Collect ("triggers" :: path)
                    |> Compare.collectArray x0.foreignKeys x1.foreignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("foreignKeys" :: path)
                    |> Compare.collectArray x0.referencedForeignKeys x1.referencedForeignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("referencedForeignKeys" :: path)
                    |> Compare.collectArray x0.checkConstraints x1.checkConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("checkConstraints" :: path)
                    |> Compare.collectArray x0.defaultConstraints x1.defaultConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("defaultConstraints" :: path)

        static member Collect (x0 : OBJECT, x1 : OBJECT, path, diffs) =
                    diffs
                    |> fun diff' -> Compare.object_name (x0, x1, path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.schema, x1.schema, "schema" :: path, diff')
                    |> fun diff' -> Compare.equalCollector (x0.object_type, x1.object_type, ("object_type" :: path), diff')

        static member Collect (x0 : System.DateTime, x1 : System.DateTime, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)

        static member Collect (x0 : COLUMN, x1 : COLUMN, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.column_name, x1.column_name, "column_name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.data_type, x1.data_type, "data_type" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_ansi_padded, x1.is_ansi_padded, "is_ansi_padded" :: path, diff')
                    |> Compare.collectOption x0.computed_definition x1.computed_definition CompareGen.Collect ("computed_definition" :: path)
                    |> Compare.collectOption x0.identity_definition x1.identity_definition Compare.identity_definition ("identity_definition" :: path)
                    |> Compare.collectOption x0.masking_function x1.masking_function CompareGen.Collect ("masking_function" :: path)
                    |> fun diff' -> CompareGen.Collect (x0.is_rowguidcol, x1.is_rowguidcol, "is_rowguidcol" :: path, diff')

        static member Collect (x0 : DATATYPE, x1 : DATATYPE, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.name, x1.name, "name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.schema, x1.schema, "schema" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.system_type_id, x1.system_type_id, "system_type_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.user_type_id, x1.user_type_id, "user_type_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.parameter, x1.parameter, "parameter" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_user_defined, x1.is_user_defined, "is_user_defined" :: path, diff')
                    |> Compare.collectOption x0.sys_datatype x1.sys_datatype Compare.equalCollector ("sys_datatype" :: path)
                    |> Compare.collectOption x0.table_datatype x1.table_datatype CompareGen.Collect ("table_datatype" :: path)

        static member Collect (x0 : System.Byte, x1 : System.Byte, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)

        static member Collect (x0 : DATATYPE_PARAMETER, x1 : DATATYPE_PARAMETER, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.max_length, x1.max_length, "max_length" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.precision, x1.precision, "precision" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.scale, x1.scale, "scale" :: path, diff')
                    |> Compare.collectOption x0.collation_name x1.collation_name CompareGen.Collect ("collation_name" :: path)
                    |> fun diff' -> CompareGen.Collect (x0.is_nullable, x1.is_nullable, "is_nullable" :: path, diff')

        static member Collect (x0 : System.Int16, x1 : System.Int16, path, diffs) = Compare.equalCollector (x0, x1, path, diffs)

        static member Collect (x0 : TABLE_DATATYPE, x1 : TABLE_DATATYPE, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')

        static member Collect (x0 : COMPUTED_DEFINITION, x1 : COMPUTED_DEFINITION, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.computed_definition, x1.computed_definition, "computed_definition" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_persisted, x1.is_persisted, "is_persisted" :: path, diff')

        static member Collect (x0 : INDEX, x1 : INDEX, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.parent, x1.parent, "parent" :: path, diff')
                    |> fun diff' -> Compare.index_name (x0, x1, path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.index_id, x1.index_id, "index_id" :: path, diff')
                    |> fun diff' -> Compare.equalCollector (x0.index_type, x1.index_type, ("index_type" :: path), diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_unique, x1.is_unique, "is_unique" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.data_space_id, x1.data_space_id, "data_space_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.ignore_dup_key, x1.ignore_dup_key, "ignore_dup_key" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_primary_key, x1.is_primary_key, "is_primary_key" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_unique_constraint, x1.is_unique_constraint, "is_unique_constraint" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.fill_factor, x1.fill_factor, "fill_factor" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_padded, x1.is_padded, "is_padded" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_disabled, x1.is_disabled, "is_disabled" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_hypothetical, x1.is_hypothetical, "is_hypothetical" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.allow_row_locks, x1.allow_row_locks, "allow_row_locks" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.allow_page_locks, x1.allow_page_locks, "allow_page_locks" :: path, diff')
                    |> Compare.collectOption x0.filter x1.filter CompareGen.Collect ("filter" :: path)
                    |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)

        static member Collect (x0 : INDEX_COLUMN, x1 : INDEX_COLUMN, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.index_id, x1.index_id, "index_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.index_column_id, x1.index_column_id, "index_column_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.column, x1.column, "column" :: path, diff')
                    |> Compare.collectOption x0.key_ordinal x1.key_ordinal CompareGen.Collect ("key_ordinal" :: path)
                    |> Compare.collectOption x0.partition_ordinal x1.partition_ordinal CompareGen.Collect ("partition_ordinal" :: path)
                    |> fun diff' -> CompareGen.Collect (x0.is_descending_key, x1.is_descending_key, "is_descending_key" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_included_column, x1.is_included_column, "is_included_column" :: path, diff')

        static member Collect (x0 : TRIGGER, x1 : TRIGGER, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.parent, x1.parent, "parent" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.trigger_name, x1.trigger_name, "trigger_name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.definition, x1.definition, "definition" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_disabled, x1.is_disabled, "is_disabled" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_instead_of_trigger, x1.is_instead_of_trigger, "is_instead_of_trigger" :: path, diff')

        static member Collect (x0 : FOREIGN_KEY, x1 : FOREIGN_KEY, path, diffs) =
                    diffs
                    |> fun diff' -> Compare.foreign_key_name (x0, x1, path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.parent, x1.parent, "parent" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.referenced, x1.referenced, "referenced" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.key_index_id, x1.key_index_id, "key_index_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_disabled, x1.is_disabled, "is_disabled" :: path, diff')
                    |> fun diff' -> Compare.equalCollector (x0.delete_referential_action, x1.delete_referential_action, ("delete_referential_action" :: path), diff')
                    |> fun diff' -> Compare.equalCollector (x0.update_referential_action, x1.update_referential_action, ("update_referential_action" :: path), diff')
                    |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)

        static member Collect (x0 : FOREIGN_KEY_COLUMN, x1 : FOREIGN_KEY_COLUMN, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.constraint_object, x1.constraint_object, "constraint_object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.constraint_column_id, x1.constraint_column_id, "constraint_column_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.parent_column, x1.parent_column, "parent_column" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.referenced_column, x1.referenced_column, "referenced_column" :: path, diff')

        static member Collect (x0 : CHECK_CONSTRAINT, x1 : CHECK_CONSTRAINT, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.parent, x1.parent, "parent" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.parent_column_id, x1.parent_column_id, "parent_column_id" :: path, diff')
                    |> Compare.collectOption x0.column x1.column CompareGen.Collect ("column" :: path)
                    |> fun diff' -> CompareGen.Collect (x0.is_disabled, x1.is_disabled, "is_disabled" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_not_for_replication, x1.is_not_for_replication, "is_not_for_replication" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_not_trusted, x1.is_not_trusted, "is_not_trusted" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.definition, x1.definition, "definition" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.uses_database_collation, x1.uses_database_collation, "uses_database_collation" :: path, diff')

        static member Collect (x0 : DEFAULT_CONSTRAINT, x1 : DEFAULT_CONSTRAINT, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.parent, x1.parent, "parent" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.column, x1.column, "column" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.definition, x1.definition, "definition" :: path, diff')

        static member Collect (x0 : VIEW, x1 : VIEW, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.schema, x1.schema, "schema" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.view_name, x1.view_name, "view_name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.definition, x1.definition, "definition" :: path, diff')
                    |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)
                    |> Compare.collectArray x0.indexes x1.indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("indexes" :: path)
                    |> Compare.collectArray x0.triggers x1.triggers SortOrder.orderBy Sequence.elementId CompareGen.Collect ("triggers" :: path)

        static member Collect (x0 : TABLE_TYPE, x1 : TABLE_TYPE, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.schema, x1.schema, "schema" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.type_name, x1.type_name, "type_name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)
                    |> Compare.collectArray x0.indexes x1.indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("indexes" :: path)
                    |> Compare.collectArray x0.foreignKeys x1.foreignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("foreignKeys" :: path)
                    |> Compare.collectArray x0.referencedForeignKeys x1.referencedForeignKeys SortOrder.orderBy Sequence.elementId CompareGen.Collect ("referencedForeignKeys" :: path)
                    |> Compare.collectArray x0.checkConstraints x1.checkConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("checkConstraints" :: path)
                    |> Compare.collectArray x0.defaultConstraints x1.defaultConstraints SortOrder.orderBy Sequence.elementId CompareGen.Collect ("defaultConstraints" :: path)

        static member Collect (x0 : PROCEDURE, x1 : PROCEDURE, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.name, x1.name, "name" :: path, diff')
                    |> Compare.collectArray x0.parameters x1.parameters SortOrder.orderBy Sequence.elementId CompareGen.Collect ("parameters" :: path)
                    |> Compare.collectArray x0.columns x1.columns SortOrder.orderBy Sequence.elementId CompareGen.Collect ("columns" :: path)
                    |> fun diff' -> CompareGen.Collect (x0.definition, x1.definition, "definition" :: path, diff')
                    |> Compare.collectArray x0.indexes x1.indexes SortOrder.orderBy Sequence.elementId CompareGen.Collect ("indexes" :: path)

        static member Collect (x0 : PARAMETER, x1 : PARAMETER, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.name, x1.name, "name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.parameter_id, x1.parameter_id, "parameter_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.data_type, x1.data_type, "data_type" :: path, diff')

        static member Collect (x0 : XML_SCHEMA_COLLECTION, x1 : XML_SCHEMA_COLLECTION, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.xml_collection_id, x1.xml_collection_id, "xml_collection_id" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.schema, x1.schema, "schema" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.name, x1.name, "name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.definition, x1.definition, "definition" :: path, diff')

        static member Collect (x0 : DATABASE_TRIGGER, x1 : DATABASE_TRIGGER, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.trigger_name, x1.trigger_name, "trigger_name" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.definition, x1.definition, "definition" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_disabled, x1.is_disabled, "is_disabled" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_instead_of_trigger, x1.is_instead_of_trigger, "is_instead_of_trigger" :: path, diff')

        static member Collect (x0 : SYNONYM, x1 : SYNONYM, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.base_object_name, x1.base_object_name, "base_object_name" :: path, diff')

        static member Collect (x0 : SEQUENCE, x1 : SEQUENCE, path, diffs) =
                    diffs
                    |> fun diff' -> CompareGen.Collect (x0.object, x1.object, "object" :: path, diff')
                    |> fun diff' -> Compare.sequence_definition (x0.sequence_definition, x1.sequence_definition, "sequence_definition" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_cycling, x1.is_cycling, "is_cycling" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_cached, x1.is_cached, "is_cached" :: path, diff')
                    |> Compare.collectOption x0.cache_size x1.cache_size CompareGen.Collect ("cache_size" :: path)
                    |> fun diff' -> CompareGen.Collect (x0.data_type, x1.data_type, "data_type" :: path, diff')
                    |> fun diff' -> CompareGen.Collect (x0.is_exhausted, x1.is_exhausted, "is_exhausted" :: path, diff')
