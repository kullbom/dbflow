# Grov plan

## Version 0.1.0(-beta)

- M�jlighet att testa script-output utan faktisk scriptmapp...?
- Tester som visar p� problem/buggar:
    - CHECK_CONSTRAINT v�l h�rdkodad vad g�ller "CHECK" etc. 
        - T�ck ordentligt med tester
        - L�gg ocks� till tester f�r andra saker som kan vara disabled...
    - Test f�r disabled triggers
    - Test f�r triggers, procedures och views man bytt namn p�...
    - Test for anonymous (system named) unique keys
    - BUG: Named defaults blir fel - se [DF_TestTable01_ColWithNamedDefault]
    - XML index fungar inte bra ... se AW - s�k p� "[XMLPATH_Person_Demographics]"
      (Det finns ett fallerande test f�r detta) 
- Dokumentera dessa i readme - eller som issues i github...? 


## Version 1.0.0

- DbUp-kompatibel (kan anv�nda dbo.SchemaVersion - om konfad s�) 
- N�gon form av db compare (syfte: hitta buggar)
    - Och test med hj�lp av detta som j�mf�r db fr�n originalscripten med db fr�n genererade script/clone
- N�got form av dokumentationsgenerering
- S�kerst�ll l�mpliga r�ttigheter innan "load" (https://github.com/sethreno/schemazen/issues/136)
- Testerna klarar inte att k�ras parallellt - pga delade mappar?
 
## Kommande version

- St�d f�r CLR-grejor... (https://github.com/sethreno/schemazen/blob/master/Library/Models/Assembly.cs)
- Statistikst�d
- Data import
- Data export