# Accessing-MediaWiki-Aquo-Code-Examples
De Aquo-wiki is gebouwd op het MediaWiki-platform met de Semantic Mediawiki extensie. Dit platform biedt verschillende tools om specifieke informatie uit de Aquo-wiki te halen.
De twee meest gebruikelijke zijn de speciaal:vragen API en de standaard Mediawiki-api. Voor de Aquo-wiki biedt de speciaal:vragen API de meeste mogelijkheden.

Hier is een ontleding van een voorbeeld-URL en de componenten:
https://www.aquo.nl/index.php?title=Speciaal:Vragen&q=%5B%5BCategorie%3ADomeinwaarden%5D%5D+%5B%5BBreder%3A%3AId-ecbe0e11-3cdc-4661-903f-f9382b32a343%5D%5D%5B%5BBegin+geldigheid%3A%3A%3E1+januari+2000%5D%5D%5B%5BEind+geldigheid%3A%3A%3E1+juli+2020%5D%5D%5B%5Bwijzigingsdatum%3A%3A%3E1+juli+2021%5D%5D%5B%5BStatus%3A%3AG%5D%5D%0A&p=format%3Dtable%2Flink%3Dnone%2Fheaders%3Dshow%2Fsearchlabel%3D%E2%80%A6-20overige-20resultaten%2Fclass%3Dsortable-20wikitable-20smwtable%2Ftheme%3Dbootstrap&po=%3FVoorkeurslabel%0A%3FOmschrijving%0A%3FId%0A%3FCodes%0A%3FGroep%0A%3FStatus%0A%3FBegin+geldigheid%0A%3FEind+geldigheid%0A%3FModification+date%0A%3FGerelateerd%0A&sort=&order=asc&eq=no&offset=0&limit=250

1. **Basis URL**: `https://www.aquo.nl/index.php`
   - Dit is de standaard entry point voor de MediaWiki API van de website.

2. **Parameters na het vraagteken (?):**
   - `title=Speciaal:Vragen`: Specificeert dat het een speciale pagina-aanvraag is voor "Vragen" (Ask).
   
3. **Query Parameters (&):**
   - `q=`: De zoekquery parameter. De waarde die volgt, is de eigenlijke zoekopdracht, geëncodeerd voor gebruik in een URL.
     - `%5B%5B`: URL-encoding voor `[[`, wat een begin van een semantische zoekquery in MediaWiki markeert.
     - `Categorie:Domeinwaarden`: Zoekt naar pagina's in de categorie "Domeinwaarden".
     - `%5D%5D`: URL-encoding voor `]]`, wat het einde van een zoekparameter markeert.
     - `Breder::Id-ecbe0e11-3cdc-4661-903f-f9382b32a343`: Zoekt naar een specifieke eigenschap met een identifier.
     - `Begin geldigheid::>1 januari 2000`: Filtert resultaten die een "Begin geldigheid" hebben na 1 januari 2000.
     - `Eind geldigheid::>1 juli 2020`: Filtert resultaten die een "Eind geldigheid" hebben na 1 juli 2020.
     - `wijzigingsdatum::>1 juli 2021`: Filtert resultaten met een wijzigingsdatum na 1 juli 2021.
     - `Status::G`: Zoekt naar resultaten met de status "G".

   - `p=`: De weergaveparameters voor hoe de resultaten moeten worden weergegeven.
     - `format=table`: Resultaten moeten worden weergegeven als een tabel.
     - `link=none`: Er mogen geen links zijn in de resultaten.
     - `headers=show`: Toon kopteksten in de tabel.
     - `searchlabel=... overige resultaten`: De tekst voor een link naar aanvullende resultaten.
     - `class=sortable wikitable smwtable`: CSS-klassen om op te nemen in de tabel.
     - `theme=bootstrap`: Gebruikt het bootstrap-thema voor de tabelopmaak.

   - `po=`: De eigenschappen die moeten worden weergegeven in de resultaten.
     - Dit bevat dingen zoals `Voorkeurslabel`, `Omschrijving`, `Id`, enz.

   - `sort=` & `order=asc`: Sorteervolgorde van de resultaten.
   - `eq=no`: Mogelijk een parameter voor de zoekquery.
   - `offset=0`: Het startpunt voor de resultaten.
   - `limit=250`: Het aantal resultaten om te tonen.

Elk van de bovenstaande parameters is een instructie voor de API over hoe de gegevens moeten worden opgehaald en gepresenteerd. URL-encoding wordt gebruikt om speciale karakters die de URL-syntax zouden verstoren veilig door te geven. Zo staat `%5B` voor de open vierkante haak `[` en `%5D` voor de sluitende vierkante haak `]`. Deze zijn essentieel voor de syntaxis van de querytaal die door de Special:Ask-functie wordt gebruikt.

Naast de Speciaal:Vragen-API is er ook nog de standaard mediawiki-API
https://www.aquo.nl/w/api.php?action=query&list=categorymembers&cmtitle=Categorie:Domeinwaarden&format=json
Met deze standaard MediaWiki API kun je weliswaar pagina's in een categorie ophalen of zoeken naar pagina's die bepaalde tekst bevatten, maar je kunt niet zoeken op specifieke eigenschapswaarden of complexe logische combinaties van deze eigenschappen zonder aanzienlijke post-processing van de opgehaalde gegevens. De standaard API is meer gericht op het ophalen van pagina-inhoud, metadata (zoals bewerkingsgeschiedenis en pagina-links), en het uitvoeren van acties zoals bewerken of uploaden.
https://www.mediawiki.org/wiki/API:Main_page

De MediaWiki API is een web service die interactie biedt met MediaWiki-wiki's zoals de Aquo. Het is een HTTP-based API, wat betekent dat je verzoeken kunt doen via standaard HTTP-methoden zoals GET en POST. De API kan een verscheidenheid aan taken uitvoeren, zoals zoeken naar inhoud, ophalen van pagina's, bewerken, en het aanmaken van accounts.

Een typisch API-verzoek naar een MediaWiki-site ziet eruit als een URL met verschillende parameters die de actie specificeren die je wilt uitvoeren. Bijvoorbeeld, om de inhoud van de Wikipedia-pagina voor "API" op te halen in JSON-formaat, zou je een GET-verzoek kunnen sturen dat er ongeveer zo uitziet:

```
https://en.wikipedia.org/w/api.php?action=query&titles=API&format=json&prop=revisions&rvprop=content
```

Hier is een uitleg van de parameters in deze URL:

- `action=query`: Dit vertelt de API dat je een zoekopdracht wilt uitvoeren.
- `titles=API`: Dit specificeert de titel van de pagina die je wilt opvragen.
- `format=json`: Dit vraagt om de uitvoer in JSON-formaat.
- `prop=revisions`: Dit vraagt om de revisie-eigenschappen van de pagina.
- `rvprop=content`: Dit vraagt om de inhoud van de laatste revisie van de pagina.
