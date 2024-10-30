# StpFoodBlazor
Display downtown Saint Paul food and drink deals

![tests](https://github.com/eebbesen/StpFoodBlazor/actions/workflows/test.yml/badge.svg)

## Requirements

You can modify this code to use other data services and data attributes but this code assumes you are using [sheet_zoukas-lambda](https://github.com/eebbesen/sheet_zoukas-lambda/) deployed on AWS to expose a Google Sheet.

The backing Google Sheet requires the following tabs and corresponding column headers
* Deals
    * Day
    * Name
    * Deal
    * Happy Hour
    * Alcohol

## Environment Variables
`SheetId` is the Google Sheet ID from which data will be read. `SheetsUrl` is the URL of the service that retrieves the data from the Google Sheet.

    ASPNETCORE_AppConfig__SheetId
    ASPNETCORE_AppConfig__SheetsUrl

## Run

    $ dotnet run

## Development

See [DEVELOPER.md](DEVELOPER.md) for more information on developing and running.
