import urllib.request
import json

url = 'http://localhost:5000/api/device/register'
data = {
    'deviceToken': 'test_token_final_ultimate',
    'deviceName': 'Ultimate Test Device'
}

req = urllib.request.Request(url, data=json.dumps(data).encode('utf-8'), headers={'Content-Type': 'application/json'})

try:
    with urllib.request.urlopen(req) as response:
        print(f"Status: {response.getcode()}")
        print(f"Response: {response.read().decode('utf-8')}")
except urllib.error.HTTPError as e:
    print(f"Status: {e.code}")
    print(f"Error Body: {e.read().decode('utf-8')}")
except Exception as e:
    print(f"General Error: {e}")
