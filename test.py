import requests
import json
import sys

url = sys.argv[1]
requesttype = sys.argv[2]
role = sys.argv[3]

data = {'requestType': requesttype, 'role': role}
headers = {'Content-type': 'application/json', 'Accept': 'text/plain'}

res = requests.post(url, data=json.dumps(data), headers=headers)

print(res.status_code, res.reason, end='\n\n')
res = json.loads(res.text)
print(res)
# print(res.text)