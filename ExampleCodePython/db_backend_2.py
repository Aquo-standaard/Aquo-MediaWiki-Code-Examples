import json

import requests
import psycopg2

# Voer de API-call uit met de opgegeven URL en verkrijg de respons in JSON-formaat
#url = "https://www.aquo.nl/index.php?p[limit]=1000&p[offset]=0&title=Speciaal%3AVragen&q=%5B%5BCategorie%3ADomeinwaarden%5D%5D+%5B%5BBreder%3A%3AId-0b6c3b35-155f-4519-aec4-bcbcbdba4169%5D%5D%5B%5BWijzigingsdatum%3A%3A%3E2023-02-21+02%3A00%3A38%5D%5D&p[format]=json&format=json&po=%3FCodes%0D%0A%3FOmschrijving%0D%0A%3FGroep%0D%0A%3FBegin+geldigheid%0D%0A%3FEind+geldigheid%0D%0A%3FWijzigingsdatum%0D%0A"
# URL zonder wijzigingsdatum
#url = "https://www.aquo.nl/index.php?p[limit]=1000&p[offset]=0&title=Speciaal%3AVragen&q=%5B%5BCategorie%3ADomeinwaarden%5D%5D+%5B%5BBreder%3A%3AId-0b6c3b35-155f-4519-aec4-bcbcbdba4169%5D%5D%5B%5BWijzigingsdatum%3A%3A%3E2023-02-21+02%3A00%3A38%5D%5D&p[format]=json&format=json&po=%3FCodes%0D%0A%3FOmschrijving%0D%0A%3FGroep%0D%0A%3FBegin+geldigheid%0D%0A%3FEind+geldigheid%0D%0A"
# Geneste URL
url = "https://www.aquo.nl/index.php?p[limit]=3&p[offset]=0&title=Speciaal%3AVragen&q=%5B%5BCategorie%3ADomeinwaarden%5D%5D%5B%5BBreder%3A%3AId-a225c399-903d-4ee1-ec2f-521b4f255cb5%5D%5D%5B%5BWijzigingsdatum%3A%3A%3E2022-01-01%5D%5D&p[format]=json&format=json&po=Id|?Codes|?Typering|?Grootheid|?ChemischeStofObject|?Eenheid2|?Hoedanigheid2|?Compartiment2|?Begin%20geldigheid|?Eind%20geldigheid|?Status|?Omschrijving|?Wijzigingsdatum%0D%0A"

response = requests.get(url)
if response.status_code == 200 and response.text:
    try:
        data = response.json()
        # Print de JSON-respons
        # print(data)
    except json.JSONDecodeError:
        print("Er is een fout opgetreden bij het decoderen van de JSON-respons.")

# Verbinden met PostgreSQL-database
conn = psycopg2.connect(
    dbname="",
    user="",
    password="",
    host="",
    port=""
)
cursor = conn.cursor()

for item in data:
    if item == 'results':
        results = data[item]
        print(results)
        bk_id = None
        bk_code = None
        bk_omschrijving = None
        bk_groep = None
        bk_begin = None
        bk_eind = None
        for key, value in results.items():
            # data has GUID beginning with "Id-". The 3: cuts that off
            bk_id = key[3:]
            for k, v in value.items():
                if k == 'printouts':
                    for k_printouts, v_printouts in v.items():
                        if k_printouts == 'Codes':
                            bk_code = v_printouts[0]
                        if k_printouts == 'Omschrijving':
                            bk_omschrijving = v_printouts[0]
                        if k_printouts == 'Groep':
                            bk_groep = v_printouts[0]
                        if k_printouts == 'Begin geldigheid':
                            #There is a weird 1/ in front the raw date
                            bk_begin = v_printouts[0]['raw'][2:]
                        if k_printouts == 'Eind geldigheid':
                            #There is a weird 1/ in front the raw date
                            bk_eind = v_printouts[0]['raw'][2:]
                        if k_printouts == 'Typering':
                            bk_displaytitle = v_printouts[0]['displaytitle']
                            print(bk_displaytitle)
            print(bk_id, bk_code, bk_groep, bk_begin, bk_eind)
            #Voeg de gegevens toe aan de database
            #bk_id (the guid) is not used yet
            cursor.execute("INSERT INTO waarnemingssoort (code, omschrijving, groep, begin_geldigheid, eind_geldigheid) VALUES (%s, %s, %s, %s, %s)",
                   (bk_code, bk_omschrijving, bk_groep, bk_begin, bk_eind))

# Commit de transactie en sluit de databaseverbinding
conn.commit()
conn.close()
