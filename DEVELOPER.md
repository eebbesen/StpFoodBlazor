
## Run

### dotnet

    $ cd StpFoodBlazor/
    $ dotnet run

https://localhost:7073/ or http://localhost:5020

### Docker

Build and run locally using Docker (mirrors the production container):

    $ docker build -t stpfoodblazor .
    $ docker run -p 8080:8080 --env-file .env stpfoodblazor

Create a `.env` file in the project root with the required variables:

    ASPNETCORE_ENVIRONMENT=Staging
    APPCONFIG__SHEETSURL=https://DOMAIN.vault.azure.net/
    APPCONFIG__SHEETID=12345abcde
    APPCONFIG__DEALURL=https://DOMAIN.azurewebsites.net/api
    APPCONFIG__GIFTCARDURL=https://DOMAIN.azurewebsites.net/api
    APPCONFIG__HOLIDAYURL=https://DOMAIN.azurewebsites.net/api
    APPCONFIG__KEYVAULTURL=https://DOMAIN.vault.azure.net/

Alternatively, if the variables are already exported in your shell:

    $ docker run -p 8080:8080 \
    -e APPCONFIG__SHEETSURL=<URL> \
    -e APPCONFIG__SHEETID=<SHEET_ID> \
    -e APPCONFIG__HOLIDAYURL=<URL> \
    -e APPCONFIG__CACHEINVALIDATIONKEY=<VALUE> \
    -e APPCONFIG__KEYVAULTURL=<VALUE> \
    stpfoodblazor

http://localhost:8080

## Test

Unit tests and Selenium functional tests are available.

https://learn.microsoft.com/en-us/aspnet/core/blazor/test?view=aspnetcore-8.0

    $ dotnet test --settings tests.runsettings --collect "XPlat Code Coverage"

If you don't have an instance of the app running on localhost you can exclude Selenium tests from the run

    $ dotnet test --settings tests.runsettings --collect "XPlat Code Coverage" --filter FullyQualifiedName\!~StpFoodBlazorTest.Integration

on Windows

    $ dotnet test --settings tests.runsettings --collect "XPlat Code Coverage"  --filter "FullyQualifiedName!~StpFoodBlazorTest.Integration"

To view tests in HTML, install `reportgenerator` https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

    $ dotnet tool install -g dotnet-reportgenerator-globaltool

and add it to your path

    $ export PATH="$PATH:/Users/username/.dotnet/tools"

This HTML artifact is also stored during GitHub Actions runs.

Then run the following to convert the XML into HTML in the `coveragereport` directory:

    $ ./scripts/generate-coverage-report.sh

### GitHub Actions

test.yml runs unit and integration tests. It will upload the following artifacts
* Opencover coverage report in XML
* HTML version of the coverage report using https://reportgenerator.io/
* PNG screenshot for each failed integration test
* HTML source code for each failed integration test (does not include CSS or JS)

#### GitHub Actions locally using [act](https://github.com/nektos/act)

Not working the same on an M1 Mac as it is in GitHub, in particular I'm seeing Selenium tests fail.
You'll need to uncomment the job in test.yml that installs Chrome.

    $ act -W '.github/workflows/test.yml' --container-architecture linux/amd64 \
    --secret APPCONFIG__SHEETSURL=$APPCONFIG__SHEETSURL \
    --secret APPCONFIG__SHEETID=$APPCONFIG__SHEETID \
    --secret APPCONFIG__HOLIDAYURL=$APPCONFIG__HOLIDAYURL \
    --secret APPCONFIG__CACHEINVALIDATIONKEY=$APPCONFIG__CACHEINVALIDATIONKEY \
    --secret APPCONFIG__KEYVAULTURL=$APPCONFIG__KEYVAULTURL \
    -P ubuntu-latest=catthehacker/ubuntu:act-latest

## Fly.io Deployment

The app is hosted on [Fly.io](https://fly.io) and deployed via Docker.

### First-time setup

    $ brew install flyctl
    $ fly auth login
    $ fly launch --no-deploy   # uses existing fly.toml

### Secrets

Set required secrets via the CLI (never commit these):

    $ fly secrets set APPCONFIG__SHEETSURL=<URL>
    $ fly secrets set APPCONFIG__SHEETID=<GOOGLE_SHEETS_ID>
    $ fly secrets set APPCONFIG__HOLIDAYURL=<URL>
    $ fly secrets set APPCONFIG__CACHEINVALIDATIONKEY=<KEY>
    $ fly secrets set APPCONFIG__KEYVAULTURL=<URL>

### Deploy

    $ fly deploy

### Logs

    $ fly logs

### SSH into running machine

    $ fly ssh console


## General

### bunit setup used to create this project

https://bunit.dev/docs/getting-started/create-test-project.html?tabs=xunit

Install bunit template for initializing new projects with bunit. This only needs to be done once

    $ dotnet new --install bunit.template

Create your new test project

    $ dotnet new bunit --framework xunit -o StpFoodBlazorTest

Link the project and the test project

    $ dotnet sln StpFoodBlazor.sln add tpFoodBlazorTest/StpFoodBlazorTest.csproj
    $ dotnet add StpFoodBlazorTest/StpFoodBlazorTest.csproj reference StpFoodBlazor/StpFoodBlazor.csproj

### Check for outdated packages

    $ dotnet list package --outdated
