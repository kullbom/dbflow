﻿# Release plan

## Version 0.1.0(-beta) - 2025-08-23

[x] C# friendly interface to all functionality
[x] Add C# example 
    [x] as a test-project 
    [x] and to the readme.md
[x] The test must remove database files (both data and log) on clean up. 
[x] The meta data for views is wrong when underlying columns has been changed. Experiment with drop and recreate all views from the views (possibly updated) definition.
    [x] Since views can depend on each other the dependency resolver must be used.
    [x] Indexes on views must also be recreated
[x] It seems that there is a problem with the order of foreign key columns - in the db comparison...?
[x] It seems that there is a problem with the order of other constraint etc as well - in the db comparison...?
[x] Check constaint missing after clone...? (See test "Test suite" for "test_db")
[x] There is a problem with (comparison of) is_nullable of user defined types
[x] There is a problem with (comparison of) ignore_dup_key of indexes 
[x] Add test to illustrate problem with is_not-trusted of check constraints
[x] Add test to illustrate problem with collation_name
[x] There is a (possible) problem with mismatching collation with a clone
    [x] Add all collation db settings to the model
    [x] Add all (relevant) db settings to the model
    [x] Add collation to columns (script) where not the same as the database default
    [x] Add settings/property file 
[x] There is a problem with (comparison of) collation_name
[x] Should have the possibility to create a LocalTempDatabase as a clone of a source in one step
[x] There is a problem with (comparison of) is_not-trusted of check constraints
[x] Add a case in test_db to illustrate problem with "is ansi padded" of columns
[x] MSDescription is not part of the generated scripts (and temporary excluded in the schema comparison)
[x] All MSDescriptions is not included in generated scripts
[x] Error handling in commit
[x] Add support for disabled triggers
[x] Add support for disabled indexes
[x] Add support for disabled check constraints

## Version 0.2.0(-beta) - 2025-08-23

[x] Add (experimental) support for cloning/copy data

## Version 0.3.0(-beta) 

[x] XProperties of types not scripted
[x] Clone all xproperties - not only ms_description
[x] Generate to temp directory and rename when finished
[x] Go through the schema model and clean it up... 
    [x] Specifically think through the model of types
[x] Make sure that key constraints is handled correct
[x] "LocalTempDatabase" does not match its module (...and make the module private?)

## Version 1.0.0

[ ] There is a problem with XML indexes... see failing test for "AdventureWorks 2014-2022"" - or search for "[XMLPATH_Person_Demographics]"
[ ] Investigate/play with improved interface to connections
    [ ] a Union like Connection | ConnectionStr | ConnectionFactory... ?
[ ] Improved support for cloning/copy data
    [ ] (Basic) Support for specific data (not only TopN)
    [ ] Consider some kind of support for exclusion and/or modification of cloned data (to be able to avoid certain data and anonymize)
[ ] Add support for disabled default constraints
[ ] Add support for disabled keys
[ ] Add a case in test_db to illustrate problem with XML indexes
[x] Ensure that the db user has suitable privileges before readSchema (IS_ROLEMEMBER ('db_ddladmin'))
[ ] Go through all the meta data SELECTs and make sure to pick up everything (primary for the comparision)
[ ] Add interface to find a specific table, view etc. from schema+name or similar
[ ] Consider adding concepts 
    [ ] "key" (a tuple of columns?)
    [ ] "value" (a concrete sql value of a specific type?)
[ ] Add test for triggers, procedures and views that has changed name (with `sp_rename`)...
[x] BUG: Named defaults is generated wrong - see [DF_TestTable01_ColWithNamedDefault]
[x] Add some kind of db-compare (to find more bugs)
[x] Add test that uses db-compare (on clones) for all regression suites 
[x] Index "FILLFACTOR" is not part of the generated scripts - and generates error when clones are compared.
[ ] The tests sometimes fail when run in parallell - might be the shared folders...?
[ ] DbFlow should track executed update scripts like DbUp do
[ ] It should be possible to configure DbFlow to be compatible with DbUp (use table dbo.SchemaVersion) 

## Some upcoming version?

[ ] Generate documentation using something like literate programming... 
[ ] ANSI PADDING is not supported. It is not considered in the db comparison and is not part of generated scripts.
[ ] Support for generating documentation
[ ] Support (model) for query input/output
[ ] Investigate how a "health report" can be constructed (on deploy?) (Validate Rules, compare to "dbup" scripts)
[ ] Support for generating cached snapshots for update-scripts - to save time on cloning
[ ] Support for CLR types and functions...
[ ] Support for running and storing db statistics (Incl. TOP-usage)
[ ] Data import from files
[ ] Support for importing data from another database - following keys and copy data recursivly ...?
[ ] Data export to files

 