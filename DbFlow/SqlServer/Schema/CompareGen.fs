namespace DbFlow.SqlServer.Schema

open DbFlow

type CompareGen = CompareGenCase
    with

        static member IsSame (x0 : DATABASE, x1 : DATABASE) =
                Compare.listIsSame x0.SCHEMAS x1.SCHEMAS SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.TABLES x1.TABLES SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.VIEWS x1.VIEWS SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.TYPES x1.TYPES SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.TABLE_TYPES x1.TABLE_TYPES SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.PROCEDURES x1.PROCEDURES SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.XML_SCHEMA_COLLECTIONS x1.XML_SCHEMA_COLLECTIONS SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.TRIGGERS x1.TRIGGERS SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.SYNONYMS x1.SYNONYMS SortOrder.orderBy CompareGen.IsSame
                && Compare.listIsSame x0.SEQUENCES x1.SEQUENCES SortOrder.orderBy CompareGen.IsSame
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame
                && Compare.listIsSame x0.all_objects x1.all_objects SortOrder.orderBy CompareGen.IsSame
                && Compare.dependencies x0.dependencies x1.dependencies

        static member IsSame (x0 : SCHEMA, x1 : SCHEMA) =
                CompareGen.IsSame (x0.name, x1.name)
                && Compare.schema_id x0.schema_id x1.schema_id
                && CompareGen.IsSame (x0.principal_name, x1.principal_name)
                && CompareGen.IsSame (x0.is_system_schema, x1.is_system_schema)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : System.String, x1 : System.String) = (x0 = x1)

        static member IsSame (x0 : System.Int32, x1 : System.Int32) = (x0 = x1)

        static member IsSame (x0 : System.Boolean, x1 : System.Boolean) = (x0 = x1)

        static member IsSame (x0 : TABLE, x1 : TABLE) =
                CompareGen.IsSame (x0.schema, x1.schema)
                && CompareGen.IsSame (x0.table_name, x1.table_name)
                && CompareGen.IsSame (x0.object, x1.object)
                && Compare.arrayIsSame x0.columns x1.columns SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.indexes x1.indexes SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.triggers x1.triggers SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.foreignKeys x1.foreignKeys SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.referencedForeignKeys x1.referencedForeignKeys SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.checkConstraints x1.checkConstraints SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.defaultConstraints x1.defaultConstraints SortOrder.orderBy CompareGen.IsSame
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : OBJECT, x1 : OBJECT) =
                CompareGen.IsSame (x0.name, x1.name)
                && Compare.object_id x0.object_id x1.object_id
                && CompareGen.IsSame (x0.schema, x1.schema)
                && Compare.optionIsSame x0.parent_object_id x1.parent_object_id CompareGen.IsSame
                && Compare.object_type x0.object_type x1.object_type
                && Compare.create_date x0.create_date x1.create_date
                && Compare.modify_date x0.modify_date x1.modify_date

        static member IsSame (x0 : System.DateTime, x1 : System.DateTime) = (x0 = x1)

        static member IsSame (x0 : COLUMN, x1 : COLUMN) =
                CompareGen.IsSame (x0.column_name, x1.column_name)
                && CompareGen.IsSame (x0.object, x1.object)
                && CompareGen.IsSame (x0.column_id, x1.column_id)
                && CompareGen.IsSame (x0.data_type, x1.data_type)
                && CompareGen.IsSame (x0.is_ansi_padded, x1.is_ansi_padded)
                && Compare.optionIsSame x0.computed_definition x1.computed_definition CompareGen.IsSame
                && Compare.optionIsSame x0.identity_definition x1.identity_definition Compare.identity_definition
                && Compare.optionIsSame x0.masking_function x1.masking_function CompareGen.IsSame
                && CompareGen.IsSame (x0.is_rowguidcol, x1.is_rowguidcol)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : DATATYPE, x1 : DATATYPE) =
                CompareGen.IsSame (x0.name, x1.name)
                && CompareGen.IsSame (x0.schema, x1.schema)
                && CompareGen.IsSame (x0.system_type_id, x1.system_type_id)
                && CompareGen.IsSame (x0.user_type_id, x1.user_type_id)
                && CompareGen.IsSame (x0.parameter, x1.parameter)
                && CompareGen.IsSame (x0.is_user_defined, x1.is_user_defined)
                && Compare.optionIsSame x0.sys_datatype x1.sys_datatype Compare.sys_datatype
                && Compare.optionIsSame x0.table_datatype x1.table_datatype CompareGen.IsSame

        static member IsSame (x0 : System.Byte, x1 : System.Byte) = (x0 = x1)

        static member IsSame (x0 : DATATYPE_PARAMETER, x1 : DATATYPE_PARAMETER) =
                CompareGen.IsSame (x0.max_length, x1.max_length)
                && CompareGen.IsSame (x0.precision, x1.precision)
                && CompareGen.IsSame (x0.scale, x1.scale)
                && Compare.optionIsSame x0.collation_name x1.collation_name CompareGen.IsSame
                && CompareGen.IsSame (x0.is_nullable, x1.is_nullable)

        static member IsSame (x0 : System.Int16, x1 : System.Int16) = (x0 = x1)

        static member IsSame (x0 : TABLE_DATATYPE, x1 : TABLE_DATATYPE) =
                CompareGen.IsSame (x0.object, x1.object)

        static member IsSame (x0 : COMPUTED_DEFINITION, x1 : COMPUTED_DEFINITION) =
                CompareGen.IsSame (x0.computed_definition, x1.computed_definition)
                && CompareGen.IsSame (x0.is_persisted, x1.is_persisted)

        static member IsSame (x0 : INDEX, x1 : INDEX) =
                CompareGen.IsSame (x0.parent, x1.parent)
                && Compare.index_name x0 x1
                && CompareGen.IsSame (x0.index_id, x1.index_id)
                && Compare.index_type x0.index_type x1.index_type
                && CompareGen.IsSame (x0.is_unique, x1.is_unique)
                && CompareGen.IsSame (x0.data_space_id, x1.data_space_id)
                && CompareGen.IsSame (x0.ignore_dup_key, x1.ignore_dup_key)
                && CompareGen.IsSame (x0.is_primary_key, x1.is_primary_key)
                && CompareGen.IsSame (x0.is_unique_constraint, x1.is_unique_constraint)
                && CompareGen.IsSame (x0.fill_factor, x1.fill_factor)
                && CompareGen.IsSame (x0.is_padded, x1.is_padded)
                && CompareGen.IsSame (x0.is_disabled, x1.is_disabled)
                && CompareGen.IsSame (x0.is_hypothetical, x1.is_hypothetical)
                && CompareGen.IsSame (x0.allow_row_locks, x1.allow_row_locks)
                && CompareGen.IsSame (x0.allow_page_locks, x1.allow_page_locks)
                && Compare.optionIsSame x0.filter x1.filter CompareGen.IsSame
                && Compare.arrayIsSame x0.columns x1.columns SortOrder.orderBy CompareGen.IsSame
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : INDEX_COLUMN, x1 : INDEX_COLUMN) =
                CompareGen.IsSame (x0.object, x1.object)
                && CompareGen.IsSame (x0.index_id, x1.index_id)
                && CompareGen.IsSame (x0.index_column_id, x1.index_column_id)
                && CompareGen.IsSame (x0.column, x1.column)
                && Compare.optionIsSame x0.key_ordinal x1.key_ordinal CompareGen.IsSame
                && Compare.optionIsSame x0.partition_ordinal x1.partition_ordinal CompareGen.IsSame
                && CompareGen.IsSame (x0.is_descending_key, x1.is_descending_key)
                && CompareGen.IsSame (x0.is_included_column, x1.is_included_column)

        static member IsSame (x0 : TRIGGER, x1 : TRIGGER) =
                CompareGen.IsSame (x0.object, x1.object)
                && CompareGen.IsSame (x0.parent, x1.parent)
                && CompareGen.IsSame (x0.trigger_name, x1.trigger_name)
                && CompareGen.IsSame (x0.orig_definition, x1.orig_definition)
                && CompareGen.IsSame (x0.definition, x1.definition)
                && CompareGen.IsSame (x0.is_disabled, x1.is_disabled)
                && CompareGen.IsSame (x0.is_instead_of_trigger, x1.is_instead_of_trigger)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : FOREIGN_KEY, x1 : FOREIGN_KEY) =
                Compare.foreign_key_name x0 x1
                && CompareGen.IsSame (x0.parent, x1.parent)
                && CompareGen.IsSame (x0.referenced, x1.referenced)
                && CompareGen.IsSame (x0.key_index_id, x1.key_index_id)
                && CompareGen.IsSame (x0.is_disabled, x1.is_disabled)
                && Compare.arrayIsSame x0.columns x1.columns SortOrder.orderBy CompareGen.IsSame
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : FOREIGN_KEY_COLUMN, x1 : FOREIGN_KEY_COLUMN) =
                CompareGen.IsSame (x0.constraint_object, x1.constraint_object)
                && CompareGen.IsSame (x0.constraint_column_id, x1.constraint_column_id)
                && CompareGen.IsSame (x0.parent_column, x1.parent_column)
                && CompareGen.IsSame (x0.referenced_column, x1.referenced_column)

        static member IsSame (x0 : CHECK_CONSTRAINT, x1 : CHECK_CONSTRAINT) =
                CompareGen.IsSame (x0.parent, x1.parent)
                && CompareGen.IsSame (x0.parent_column_id, x1.parent_column_id)
                && Compare.optionIsSame x0.column x1.column CompareGen.IsSame
                && CompareGen.IsSame (x0.is_disabled, x1.is_disabled)
                && CompareGen.IsSame (x0.is_not_for_replication, x1.is_not_for_replication)
                && CompareGen.IsSame (x0.is_not_trusted, x1.is_not_trusted)
                && CompareGen.IsSame (x0.definition, x1.definition)
                && CompareGen.IsSame (x0.uses_database_collation, x1.uses_database_collation)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : DEFAULT_CONSTRAINT, x1 : DEFAULT_CONSTRAINT) =
                CompareGen.IsSame (x0.parent, x1.parent)
                && CompareGen.IsSame (x0.column, x1.column)
                && CompareGen.IsSame (x0.definition, x1.definition)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : VIEW, x1 : VIEW) =
                CompareGen.IsSame (x0.schema, x1.schema)
                && CompareGen.IsSame (x0.view_name, x1.view_name)
                && CompareGen.IsSame (x0.object, x1.object)
                && CompareGen.IsSame (x0.definition, x1.definition)
                && Compare.arrayIsSame x0.columns x1.columns SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.indexes x1.indexes SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.triggers x1.triggers SortOrder.orderBy CompareGen.IsSame
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : TABLE_TYPE, x1 : TABLE_TYPE) =
                CompareGen.IsSame (x0.schema, x1.schema)
                && CompareGen.IsSame (x0.type_name, x1.type_name)
                && CompareGen.IsSame (x0.object, x1.object)
                && Compare.arrayIsSame x0.columns x1.columns SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.indexes x1.indexes SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.foreignKeys x1.foreignKeys SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.referencedForeignKeys x1.referencedForeignKeys SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.checkConstraints x1.checkConstraints SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.defaultConstraints x1.defaultConstraints SortOrder.orderBy CompareGen.IsSame
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : PROCEDURE, x1 : PROCEDURE) =
                CompareGen.IsSame (x0.object, x1.object)
                && CompareGen.IsSame (x0.name, x1.name)
                && Compare.arrayIsSame x0.parameters x1.parameters SortOrder.orderBy CompareGen.IsSame
                && Compare.arrayIsSame x0.columns x1.columns SortOrder.orderBy CompareGen.IsSame
                && CompareGen.IsSame (x0.orig_definition, x1.orig_definition)
                && CompareGen.IsSame (x0.definition, x1.definition)
                && Compare.arrayIsSame x0.indexes x1.indexes SortOrder.orderBy CompareGen.IsSame
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : PARAMETER, x1 : PARAMETER) =
                CompareGen.IsSame (x0.object, x1.object)
                && CompareGen.IsSame (x0.name, x1.name)
                && CompareGen.IsSame (x0.parameter_id, x1.parameter_id)
                && CompareGen.IsSame (x0.data_type, x1.data_type)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : XML_SCHEMA_COLLECTION, x1 : XML_SCHEMA_COLLECTION) =
                CompareGen.IsSame (x0.xml_collection_id, x1.xml_collection_id)
                && CompareGen.IsSame (x0.schema, x1.schema)
                && CompareGen.IsSame (x0.name, x1.name)
                && Compare.create_date x0.create_date x1.create_date
                && Compare.modify_date x0.modify_date x1.modify_date
                && CompareGen.IsSame (x0.definition, x1.definition)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : DATABASE_TRIGGER, x1 : DATABASE_TRIGGER) =
                CompareGen.IsSame (x0.trigger_name, x1.trigger_name)
                && CompareGen.IsSame (x0.definition, x1.definition)
                && CompareGen.IsSame (x0.is_disabled, x1.is_disabled)
                && CompareGen.IsSame (x0.is_instead_of_trigger, x1.is_instead_of_trigger)
                && Compare.optionIsSame x0.ms_description x1.ms_description CompareGen.IsSame

        static member IsSame (x0 : SYNONYM, x1 : SYNONYM) =
                CompareGen.IsSame (x0.object, x1.object)
                && CompareGen.IsSame (x0.base_object_name, x1.base_object_name)

        static member IsSame (x0 : SEQUENCE, x1 : SEQUENCE) =
                CompareGen.IsSame (x0.object, x1.object)
                && Compare.sequence_definition x0.sequence_definition x1.sequence_definition
                && CompareGen.IsSame (x0.is_cycling, x1.is_cycling)
                && CompareGen.IsSame (x0.is_cached, x1.is_cached)
                && Compare.optionIsSame x0.cache_size x1.cache_size CompareGen.IsSame
                && CompareGen.IsSame (x0.data_type, x1.data_type)
                && CompareGen.IsSame (x0.is_exhausted, x1.is_exhausted)
