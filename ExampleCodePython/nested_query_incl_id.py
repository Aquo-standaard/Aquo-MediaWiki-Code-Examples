
import json
import requests

# URL van de API-aanroep
url = "https://www.aquo.nl/index.php?&title=Speciaal%3AVragen&q=%5B%5BCategorie%3ADomeinwaarden%5D%5D%5B%5BBreder%3A%3AId-1598f000-a066-42f3-ac92-b0e9d6b61d55%5D%5D%5B%5BEind%20geldigheid::%E2%89%A55%20augustus%202024%5D%5D&p[format]=json&format=json&po=Id|?Voorkeurslabel|?Codes|?Begin%20geldigheid|?Eind%20geldigheid|?Omschrijving|?Gerelateerd%0D%0A"
print(url)
#%5B%5BEind%20geldigheid::%E2%89%A55%20augustus%202024%5D%5D
response = requests.get(url)
data = None

if response.status_code == 200 and response.text:
    try:
        data = response.json()
    except json.JSONDecodeError:
        print("Er is een fout opgetreden bij het decoderen van de JSON-respons.")

# Verzamel alle resultaten in een lijst
results_list = []

if data:
    for item in data:
        if item == 'results':
            results = data[item]
            print(results)
            bk_guid = None
            bk_id = None
            bk_voorkeurslabel = None
            bk_code = None
            bk_omschrijving = None
            bk_begin = None
            bk_eind = None
            bk_gerelateerd = None
            for key, value in results.items():
                bk_guid = key[0:]
                for k, v in value.items():
                    if k == 'printouts':
                        for k_printouts, v_printouts in v.items():
                            if k_printouts == 'Voorkeurslabel' and v_printouts:
                                bk_voorkeurslabel = v_printouts[0]
                            if k_printouts == 'Id' and v_printouts:
                                bk_id = v_printouts[0]
                            if k_printouts == 'Codes' and v_printouts:
                                bk_code = v_printouts[0]
                            if k_printouts == 'Omschrijving' and v_printouts:
                                bk_omschrijving = v_printouts[0]
                            if k_printouts == 'Begin geldigheid' and v_printouts:
                                bk_begin = v_printouts[0]['raw'][2:]
                            if k_printouts == 'Eind geldigheid' and v_printouts:
                                bk_eind = v_printouts[0]['raw'][2:]
                            if k_printouts == 'Gerelateerd' and v_printouts:
                                bk_gerelateerd = v_printouts[0]
                # Print de resultaten naar het scherm
                print(bk_guid, bk_id, bk_voorkeurslabel, bk_code, bk_begin, bk_eind, bk_gerelateerd)
                
                # Voeg de resultaten toe aan de lijst
                results_list.append({
                    "guid": bk_guid,
                    "ID": bk_id,
                    "voorkeurslabel": bk_voorkeurslabel,
                    "code": bk_code,
                    "begin_geldigheid": bk_begin,
                    "eind_geldigheid": bk_eind,
                    "omschrijving": bk_omschrijving,
                    "gerelateerd": bk_gerelateerd
                })

# Schrijf de resultaten naar een JSON-bestand
# Geef het volledige pad voor het opslaan van output.json
output_path = r"..\..\output.json"

# Schrijf de resultaten naar een JSON-bestand
with open(output_path, 'w') as json_file:
    json.dump(results_list, json_file, indent=4)

print(f"Data succesvol opgeslagen in {output_path}")
