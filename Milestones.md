# Grov plan

## Version 0.1.0(-beta)

[x] C# friendly interface to all functionality
[ ] Add C# example - as a test-project and to the readme.md
[x] The test most remove database files (both data och log) on clean up. 
[x] THe meta data for views is wrong when underlying columns has been changed. Experiment with drop and recreate all views from the views (possibly updated) definition.
    [x] Since views can depend on each other the dependency resolver must be used.
    [x] Indexes on views must also be recreated
[x] It seems that there is a problem with the order of foreign key columns - in the db comparison...?
[x] It seems that there is a problem with the order of other constraint etc as well - in the db comparison...?
[ ] There is some problems with (comparision of) user defined types
[ ] There is a problem with (comparision of) collation_name
[ ] There is a problem with ignore_dup_key of indexes 
[ ] There is a problem with is_not-trusted of check constraints


## Version 1.0.0

[ ] Ensure that the db user has suitable privileges before readSchema (https://github.com/sethreno/schemazen/issues/136)
[ ] Tester som visar på problem/buggar (eller lösning på problemet):
    [ ] CHECK_CONSTRAINT väl hårdkodad vad gäller "CHECK" etc. 
        [ ] Täck ordentligt med tester
        [ ] Lägg också till tester för andra saker som kan vara disabled...
    [ ] Test för disabled triggers
    [ ] Test för triggers, procedures och views man bytt namn på...
    [ ] Test for anonymous (system named) unique keys
    [ ] BUG: Named defaults blir fel - se [DF_TestTable01_ColWithNamedDefault]
    [ ] XML index fungar inte bra ... se AW - sök på "[XMLPATH_Person_Demographics]"
    [x] Index "FILLFACTOR" är inte med i de genererade scripten - och blir därför fel i kloner.
[ ] Column "is ansi padded" blir fel 
[ ] ms_description is not part of the generated scripts (and temporary excluded in the schema comparison)
[ ] Dokumentera alla kända problem i någon slags readme - eller som issues i github...? Eller så får fallerande test räcka...
[ ] DbUp-kompatibel (kan använda dbo.SchemaVersion - om konfad så) 
[x] Någon form av db compare (syfte: hitta buggar)
[x] Test med hjälp av detta som jämför db från originalscripten med db från genererade script/clone
[ ] Något form av dokumentationsgenerering
[ ] Testerna klarar inte att köras parallellt - pga delade mappar?
 
## Kommande version

[ ] ("DbUp") Stöd för att generera "cachade brytpunkter" som körs istället för originalen (vid behov)
[ ] Stöd för CLR-grejor... (https://github.com/sethreno/schemazen/blob/master/Library/Models/Assembly.cs)
[ ] Statistikstöd (Inkl. TOP-usage)
[ ] Data import
[ ] Support for importing data from another database - following keys and copy data recursivly ...?
[ ] Data export