{
  "Proxy": {
    "AllowedMethods": [
      "importlightwalletaddress",
      "getwatchonlystatus",
      "getwatchonlytxes",
      "checkkeyimages",
      "getanonoutputs",
      "sendrawtransaction",
      "getblockchaininfo",
      "getrawmempool"
    ],
    "Node": {
      "Url": "http://127.0.0.1:1234/",
      "Username": "",
      "Password": ""
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "EndPointDefaults": {
      "Protocols": "Http1AndHttp2"
    },
    "Endpoints": {
      "HTTP": {
        "Url": "http://*:5000"
      }
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "System.Net.Http.HttpClient": "Error"
      }
    },
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithThreadId"
    ],
    "Destructure": [
      {
        "Name": "ToMaximumDepth",
        "Args": {
          "maximumDestructuringDepth": 4
        }
      },
      {
        "Name": "ToMaximumStringLength",
        "Args": {
          "maximumStringLength": 100
        }
      },
      {
        "Name": "ToMaximumCollectionCount",
        "Args": {
          "maximumCollectionCount": 10
        }
      }
    ],
    "Properties": {
      "Application": "VeilNodeProxy"
    },
    "WriteTo": [
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "Console"
            }
          ]
        }
      },
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "File",
              "Path": "./logs/log-.txt",
              "BufferSize": 4096,
              "BlockWhenFull": true
            }
          ]
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "./logs/log-.log",
          "rollingInterval": "Day",
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  }
}