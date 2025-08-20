# Release plan

## Version 0.1.0(-beta)

[x] C# friendly interface to all functionality
[ ] Add C# example 
    [x] as a test-project 
    [ ] and to the readme.md
[x] The test must remove database files (both data och log) on clean up. 
[x] THe meta data for views is wrong when underlying columns has been changed. Experiment with drop and recreate all views from the views (possibly updated) definition.
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
    [x] Add all (relevant) db settings to the model
    [x] Add collation to columns (script) where not the same as the database default
    [x] Add settings/property file 
    [ ] Include root folder in the schemazen compatibility tests
[x] There is a problem with (comparison of) collation_name
[ ] Should have the possibility to create a LocalTempDatabase as a clone of a source in one step
[ ] There is a problem with (comparison of) is_not-trusted of check constraints
[ ] Add test to illustrate problem with "is ansi padded" of columns
[ ] There is a problem with (comparison of) "is ansi padded" of columns

## Make the repo public

[ ] License-header in all files
[ ] Generate to temp directory and rename when finished
[ ] Go through the schema model and clean it up... 
    [ ] Specifically think through the model of types
[ ] Add interface to find i specific table, view etc. from schema+name or similar
[ ] Consider adding concepts 
    [ ] "key" (for a tuple of columns)
    [ ] "value" (for a concrete sql value of a specifc type)
[ ] "LocalTempDatabase" does not match its module (...and make the module private?)

## Version 1.0.0

[x] Ensure that the db user has suitable privileges before readSchema ( IS_ROLEMEMBER ('db_ddladmin'))
[ ] Tester som visar på problem/buggar (eller lösning på problemet):
    [ ] CHECK_CONSTRAINT väl hårdkodad vad gäller "CHECK" etc. 
        [ ] Täck ordentligt med tester
        [ ] Lägg också till tester för andra saker som kan vara disabled...
    [ ] Test för disabled triggers
    [ ] Test för triggers, procedures och views man bytt namn på...
    [ ] Test for anonymous (system named) unique keys
    [x] BUG: Named defaults blir fel - se [DF_TestTable01_ColWithNamedDefault]
    [ ] XML index fungar inte bra ... se AW - sök på "[XMLPATH_Person_Demographics]"
    [x] Index "FILLFACTOR" är inte med i de genererade scripten - och blir därför fel i kloner.
[ ] Dokumentera alla kända problem i någon slags readme - eller som issues i github...? Eller så får fallerande test räcka...
[ ] DbUp-kompatibel (kan använda dbo.SchemaVersion - om konfad så) 
[x] Någon form av db compare (syfte: hitta buggar)
[x] Test med hjälp av detta som jämför db från originalscripten med db från genererade script/clone
[ ] Testerna klarar inte att köras parallellt - pga delade mappar?

## Därpå följande version?

[ ] ms_description is not part of the generated scripts (and temporary excluded in the schema comparison)
[ ] Något form av dokumentationsgenerering
[ ] (Recursive) copy of data (following all FKs) - and not interfere with constraint trust!
[ ] Support (model) for  query input/output
[ ] Investigate how a "health report" can be constructed (on deploy?) (Validate Rules, compare to "dbup" scripts)

## Kommande version

[ ] ("DbUp") Stöd för att generera "cachade brytpunkter" som körs istället för originalen (vid behov)
[ ] Stöd för CLR-grejor... (https://github.com/sethreno/schemazen/blob/master/Library/Models/Assembly.cs)
[ ] Statistikstöd (Inkl. TOP-usage)
[ ] Data import
[ ] Support for importing data from another database - following keys and copy data recursivly ...?
[ ] Data export