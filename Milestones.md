# Grov plan

## Version 0.1.0(-beta)

- C#-vänligare interface
- Möjlighet att testa script-output utan faktisk scriptmapp...?
- Tester som visar på problem/buggar (eller lösning på problemet):
    - CHECK_CONSTRAINT väl hårdkodad vad gäller "CHECK" etc. 
        - Täck ordentligt med tester
        - Lägg också till tester för andra saker som kan vara disabled...
    - Test för disabled triggers
    - Test för triggers, procedures och views man bytt namn på...
    - Test for anonymous (system named) unique keys
    - BUG: Named defaults blir fel - se [DF_TestTable01_ColWithNamedDefault]
    - XML index fungar inte bra ... se AW - sök på "[XMLPATH_Person_Demographics]"
✓   - Index "FILLFACTOR" är inte med i de genererade scripten - och blir därför fel i kloner.
    - Column "is ansi padded" blir fel 
✓   - Det är fel på metadatat för vyer (?!) vars kolumner definierats om. (prova att definiera om (drop och create) vyer utifrån sin - uppdaterade - definition i samband med inläsning)
✓   - För att kunna definiera om views behöver dependency resolvern köras på vyerna
    - Index på vyer försvinner när man definierar om dem... hm... måste jag köra två full svep?!
    - ms_description is not part of the generated scripts (and temporary excluded in the schema comparison)
- Dokumentera dessa i readme - eller som issues i github...?


## Version 1.0.0

- DbUp-kompatibel (kan använda dbo.SchemaVersion - om konfad så) 
✓ - Någon form av db compare (syfte: hitta buggar)
✓ - Test med hjälp av detta som jämför db från originalscripten med db från genererade script/clone
- Något form av dokumentationsgenerering
- Säkerställ lämpliga rättigheter innan "load" (https://github.com/sethreno/schemazen/issues/136)
- Testerna klarar inte att köras parallellt - pga delade mappar?
 
## Kommande version

- Stöd för CLR-grejor... (https://github.com/sethreno/schemazen/blob/master/Library/Models/Assembly.cs)
- Statistikstöd
- Data import
- Data export