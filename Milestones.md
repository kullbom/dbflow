# Grov plan

## Version 0.1.0(-beta)

[x] C#-vänligare interface
[x] Testen måste städa bort databasfiler efter sig. Blir snabbt mycket skräpdata...
[x] Det är fel på metadatat för vyer (?!) vars kolumner definierats om. (prova att definiera om (drop och create) vyer utifrån sin - uppdaterade - definition i samband med inläsning)
    [x] För att kunna definiera om views behöver dependency resolvern köras på vyerna
    [x] Index på vyer försvinner när man definierar om dem... hm... måste jag köra två full svep?!
[ ] It seems that there is a problem with the order of foreign key columns - in the db comparison...?


## Version 1.0.0

[ ] Säkerställ lämpliga rättigheter innan "load" (https://github.com/sethreno/schemazen/issues/136)
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