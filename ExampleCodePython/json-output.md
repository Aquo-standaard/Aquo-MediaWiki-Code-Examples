# json-output
De json-output van een speciaal:vragen request vraagt wat aandacht. Normaal gesproken is json opgebouwd uit key:value paren. 
Als je nu naar mediawiki JSONs kijkt, zie je dat de key:value combinatie niet consequent gebruikt wordt zoals de GUID die geen key aanduiding heeft, 
	"results": {
		**"Id-4009c898-831e-452e-bc44-42c669d1641d": {**
			"printouts": {
				"Codes": \[
					"VL-40-90mm"
				\],
				"Omschrijving": [
					"Vislengteklasse groter dan 40 en kleiner dan 90 mm"

Deze GUID komt later terug als pageid. Deze redundantie nemen we maar voor lief.

Een andere bijzonderheid is dat regelmatig de array notitie wordt gebruikt waar die niet nodig is of zelfs onjuist. Uit geneste_url bijvoorbeeld:
"Typering": \[
					{
						"fulltext": "Id-928ab723-a732-4e1a-a1b8-33a029521035",
						"fullurl": "https://www.aquo.nl/index.php/Id-928ab723-a732-4e1a-a1b8-33a029521035",
						"namespace": 0,
						"exists": "1",
						"displaytitle": "Afmeldreden niet genomen monster"
					}
				\],
Typering is in dit geval geen array. Het is een object, met attributen fulltext, fullurl, etc. Maar daarom moet je in de code steeds een [0] opnemen om de eerste tupel van de array te pakken. Terwijl er altijd maar 1 is.

Normaal gesproken kan een client redelijk simpel recursief door een JSON heen wandelen, maar in het geval van mediawiki moet je weten wat je tegenkomt en daarom hard coderen. Daarnaast moet je om de arrays heen programmeren door steeds [0] extra op te nemen. Een van de duidelijkste voorbeelden is "Id" (in printouts). In de json heeft ID een array, waardoor de mogelijkheid bestaat op meerdere IDs? 

Het script db_packend_2.py lost een aantal van bovenstaande issues op voor de domeintabel Waarnemingssoort. 

