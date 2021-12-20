import datetime
import json
import sys
import urllib.parse
import urllib.request


# Defines the require elements needed to process a domain table.
class TableDefinition:
    """__init__() functions as the class constructor"""

    def __init__(self, label=None, identity=None, begin=0, end=0, fields=None):
        if fields is None:
            fields = []
        self.label = label
        self.id = identity
        self.begin = datetime.datetime.fromtimestamp(int(begin))
        self.end = datetime.datetime.fromtimestamp(int(end))
        self.fields = fields


# Spaces are not accepted: they are defined as control characters. Encode them.
def encode_control_characters(value):
    return str.replace(value, ' ', '%20')


# Field names need to be combined in a special format.
def construct_field_names_part(field_names):
    field_name_part = ''
    sep = ''
    for field_name in field_names:
        field_name_part += sep + encode_control_characters(field_name)
        sep = '|?'
    return field_name_part


# Compose the url to retrieve the domain table definitions.
def get_table_definitions():
    read_table_definitions = {}
    offset = 0
    limit = 500
    while True:
        # Get the next block of domain tables
        get_domain_table_url = construct_table_definition_url(offset, limit)
        content = urllib.request.urlopen(get_domain_table_url)
        read_content = content.read()

        top = json.loads(read_content)
        if len(top) == 0:
            break
        # Retrieve the array of key values, defining the Ids of the domain tables.
        for identifier in top['results']:
            # With each id retrieve the relevant data: name of the domain table, id,
            # start date, end date and the known fields (metadata)
            read_table_definitions[top['results'][identifier]['printouts']['Voorkeurslabel'][0]] = TableDefinition(
                top['results'][identifier]['printouts']['Voorkeurslabel'][0], identifier,
                top['results'][identifier]['printouts']['Begin geldigheid'][0]['timestamp'],
                top['results'][identifier]['printouts']['Eind geldigheid'][0]['timestamp'],
                top['results'][identifier]['printouts']['Metadata'])
        if len(top) < limit:
            break

        offset += limit
    return read_table_definitions


def construct_table_definition_url(offset, limit):
    date = datetime.date.today().strftime("%Y-%m-%d")
    baseurl = 'https://www.aquo.nl'
    service = '/index.php'
    request = '?title=Speciaal:Vragen'
    topic = '&x=[[Categorie:Domeintabellen]]'
    condition = encode_control_characters(f'[[Eind geldigheid::>{date}]][[Begin geldigheid::<{date}]]')
    retrieve = encode_control_characters('/?Metadata|?Begin geldigheid|?Eind geldigheid|?Voorkeurslabel')
    format_specification = '&format=json&unescape=true'
    get_domain_table_url = f'{baseurl}{service}{request}{topic}{condition}{retrieve}&limit={limit}&offset={offset}{format_specification}'
    return get_domain_table_url


# Construct the url to retrieve the domain values.
def construct_table_request_url(definition, limit, offset):
    date = datetime.date.today().strftime("%Y-%m-%d")
    baseurl = 'https://www.aquo.nl'
    service = '/index.php'
    request = '?title=Speciaal:Vragen'
    topic = f'&q=[[Categorie:Domeinwaarden]]+[[Breder::{definition.id}]]'
    condition = encode_control_characters(f'[[Eind geldigheid::>{date}]][[Begin geldigheid::<{date}]]')
    retrieve = '&po=' + construct_field_names_part(definition.fields)
    format_specification = '&p[unescape]=true&p[format]=json&sort_num=&order_num=ASC&p[link]=none&p[sort]=&p[headers]=show'
    return f'{baseurl}{service}{request}{topic}{condition}{retrieve}&limit={limit}&offset={offset}{format_specification}'


# Find the table within the definitions.
def find_table_definition_by_name(value, definitions):
    list_of_items = definitions.items()
    for item in list_of_items:
        if item[0] == value:
            return item[1]
    return None


# Retrieve the table data.
def get_table_data(table_name, table_definitions):
    table = find_table_definition_by_name(table_name, table_definitions)
    if table is None:
        return None
    limit = 500
    offset = 0
    while True:
        url = construct_table_request_url(table, limit, offset)
        content = urllib.request.urlopen(url)
        read_content = content.read()
        records = []
        json_records = json.loads(read_content)
        if len(json_records) == 0:
            break
        for identifier in json_records['results']:
            record = {}
            for field in table.fields:
                if field == 'Begin geldigheid' or field == 'Eind geldigheid':
                    record[field] = datetime.datetime.fromtimestamp(int(json_records['results'][identifier]['printouts'][field][0]['timestamp'])).strftime("%Y-%m-%d")
                else:
                    if len(json_records['results'][identifier]['printouts'][field]) >= 1:
                        record[field] = json_records['results'][identifier]['printouts'][field][0]
                    else:
                        record[field] = None
            record["fulltext"] = json_records['results'][identifier]['fulltext']
            records.append(record)
        if len(json_records['results']) < limit:
            break
        offset += limit
    return records


# Print the table data as a CSV file.
def print_as_csv(table_definition, response):
    # print header
    sep = ''
    fields = table_definition.fields
    if "fulltext" not in fields:
        fields.append("fulltext")

    for field in fields:
        print(sep + '"' + field + '"', sep=",", end='')
        sep = ","
    print()

    fields = table_definition.fields
    for item in response:
        sep = ''
        for field in fields:
            if item[field] is None:
                print(sep + '""', sep=",", end='')
            else:
                if item[field] is int:
                    print(sep + '"' + str(item[field]) + '"', sep=",", end='')
                else:
                    if item[field] is dict:
                        print(type(item[field]))
                        print(sep + '"' + item[field][0] + '"', sep=",", end='')
                    else:
                        print(sep + '"' + str(item[field]) + '"', sep=",", end='')
            sep = ","
        print()


# Process a table.
def process_table(table_name):
    table_definitions = get_table_definitions()
    table_definition = find_table_definition_by_name(table_name, table_definitions)
    if table_definition is None:
        print("Onbekende of niet-actuele tabel")
        return
    table_data = get_table_data(table_name, table_definitions)
    print_as_csv(table_definition, table_data)


if __name__ == '__main__':
    if len(sys.argv) == 1:
        process_table('Eenheid')
    else:
        process_table(sys.argv[1])
else:
    process_table('Eenheid')
