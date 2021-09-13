from pathlib import Path
import csv
import json

input_file = csv.DictReader(open("people.csv"))
output_file = open(Path(__file__).parent / "../src/lib" / "people.json", "w")
output_file.write(json.dumps({"new": [row for row in input_file]}))
output_file.close()
