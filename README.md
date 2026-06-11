# StpFoodBlazor
Display downtown Saint Paul food and drink deals

![tests](https://github.com/eebbesen/StpFoodBlazor/actions/workflows/test.yml/badge.svg)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=eebbesen_StpFoodBlazor&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=eebbesen_StpFoodBlazor)

[See it in action!](https://stpfoodblazor.fly.dev)

## Parameterized Endpoints
Happy hour and alcohol only parameters are respected and available on the About page:
* `alcoholOnly` will limit results to deals with alcohol when true; when false deals including those with alcohol will be returned
* `hh` will return all deals _including_ happy hour (default is not to include happy hour deals)

Both parameters can be used with each other.

## Development

See [DEVELOPER.md](DEVELOPER.md) for more information on developing and running.

## Requirements

You can modify this code to use other data services and data attributes but this code assumes you are using [sheet_zoukas-lambda](https://github.com/eebbesen/sheet_zoukas-lambda/) deployed on AWS to expose a Google Sheet.

The backing Google Sheet requires the following tabs and corresponding column headers
* Deals
    * Day
    * Name
    * Deal
    * Happy Hour
    * Alcohol
    * Url
    * Start
    * End

* giftcards
    * Place
    * Deal
    * Deal Starts
    * Deal Ends


## Environment Variables

`SHEETID` is the Google Sheet ID from which data will be read.
`SHEETSURL` is the URL of the service that retrieves the data from the Google Sheet.
`HOLIDAYURL` is the URL of the service that retrieves holiday data (e.g., https://DOMAIN.azurewebsites.net/api).
`CACHEINVALIDATIONKEY` is the secret that allows the cache to be invalidated.
`KEYVAULTURL` is the the URL of the Key Vault in Azure (for Application Insights)

    APPCONFIG__SHEETID
    APPCONFIG__SHEETSURL
    APPCONFIG__HOLIDAYURL
    APPCONFIG__CACHEINVALIDATIONKEY
    APPCONFIG__KEYVAULTURL

## Run

### Local (dotnet)

    $ cd StpFoodBlazor/
    $ dotnet run

### Local (Docker)

    $ docker build -t stpfoodblazor .
    $ docker run -p 8080:8080 \
        -e APPCONFIG__HOLIDAYURL=https://DOMAIN.azurewebsites.net/api \
        stpfoodblazor

### Remote (Fly.io)

See [DEVELOPER.md](DEVELOPER.md) for deployment instructions.

## Cache cleaning
Caches can be invalidated when the underlying data changes if you add scripting to the Google Sheet.

### Google Sheets script

See [scripts/sheets-cache-invalidation.gs](scripts/sheets-cache-invalidation.gs)

### Invalidate all sheet caches

    curl -X POST https://stpfoodblazor.fly.dev/api/cache/invalidate \
    -H "X-Cache-Invalidation-Key: SECRET"

### Invalidate a specific cache

    curl -X POST "https://stpfoodblazor.fly.dev/api/cache/invalidate?key=deals" \
    -H "X-Cache-Invalidation-Key: SECRET"

    curl -X POST "https://stpfoodblazor.fly.dev/api/cache/invalidate?key=giftcards" \
    -H "X-Cache-Invalidation-Key: SECRET"

### Check cache status

    curl -X GET https://stpfoodblazor.fly.dev/api/cache \
    -H "X-Cache-Invalidation-Key: SECRET"
