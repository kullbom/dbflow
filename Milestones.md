# Grov plan

## Version 0.1.0(-beta)

- Möjlighet att testa script-output utan faktisk scriptmapp...?
- Tester som visar på problem/buggar:
    - CHECK_CONSTRAINT väl hårdkodad vad gäller "CHECK" etc. 
        - Täck ordentligt med tester
        - Lägg också till tester för andra saker som kan vara disabled...
    - Test för disabled triggers
    - Test för triggers, procedures och views man bytt namn på...
    - Test for anonymous (system named) unique keys
    - BUG: Named defaults blir fel - se [DF_TestTable01_ColWithNamedDefault]
    - XML index fungar inte bra ... se AW - sök på "[XMLPATH_Person_Demographics]"
      (Det finns ett fallerande test för detta) 
- Dokumentera dessa i readme - eller som issues i github...? 


## Version 1.0.0

- DbUp-kompatibel (kan använda dbo.SchemaVersion - om konfad så) 
- Någon form av db compare (syfte: hitta buggar)
    - Och test med hjälp av detta som jämför db från originalscripten med db från genererade script/clone
- Något form av dokumentationsgenerering
- Säkerställ lämpliga rättigheter innan "load" (https://github.com/sethreno/schemazen/issues/136)
- Testerna klarar inte att köras parallellt - pga delade mappar?
 
## Kommande version

- Stöd för CLR-grejor... (https://github.com/sethreno/schemazen/blob/master/Library/Models/Assembly.cs)
- Statistikstöd
- Data import
- Data export