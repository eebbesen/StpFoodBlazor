# StpFoodBlazor
Display downtown Saint Paul food and drink deals

![tests](https://github.com/eebbesen/StpFoodBlazor/actions/workflows/test.yml/badge.svg)

[See it in action!](stpfoodblazor-d3f0aqbuf5bxfugt.centralus-01.azurewebsites.net)

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
`SHEETID` is the Google Sheet ID from which data will be read. `SHEETSURL` is the URL of the service that retrieves the data from the Google Sheet.

    ASPNETCORE_APPCONFIG__SHEETID
    ASPNETCORE_APPCONFIG__SHEETSURL

## Run

    $ dotnet run

## Development

See [DEVELOPER.md](DEVELOPER.md) for more information on developing and running.
