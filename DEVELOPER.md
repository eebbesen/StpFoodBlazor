
## Run

    $ cd StpFoodBlazor/StpFoodBlazor/
    $ dotnet run

https://localhost:7073/

## Test
Unit tests and Selenium functional tests are available.

https://learn.microsoft.com/en-us/aspnet/core/blazor/test?view=aspnetcore-8.0

    $ dotnet test --settings tests.runsettings --collect "XPlat Code Coverage"

If you don't have an instance of the app running on localhost you can exclude Selenium tests from the run

    $ dotnet test --settings tests.runsettings --filter FullyQualifiedName\!~StpFoodBlazorTest.Integration --collect "XPlat Code Coverage"

To view tests in HTML, install `reportgenerator` https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

    $ dotnet tool install -g dotnet-reportgenerator-globaltool

and add it to your path

    $ export PATH="$PATH:/Users/username/.dotnet/tools"

This HTML artifact is also stored during GitHub Actions runs.

Then run the following to convert the XML into HTML in the `coveragereport` directory.
Run from the StpFoodBlazorTest directory or modify the reports path

    $ reportgenerator \
      -reports:"./TestResults/{guid}/coverage.opencover.xml" \
      -targetdir:"coveragereport" \
      -reporttypes:Html


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
    --secret ASPNETCORE_APPCONFIG__SHEETSURL=$ASPNETCORE_APPCONFIG__SHEETSURL \
    --secret ASPNETCORE_APPCONFIG__SHEETID=$ASPNETCORE_APPCONFIG__SHEETID \
    -P ubuntu-latest=catthehacker/ubuntu:act-latest

## Azure

### Logging
Refer to appsettings.json for more granular control of logging.

When deployed on Azure logs aree available to stream. To enable:
* navigate to your application in https://portal.azure.com
* turn Application Logging (Filesystem) to On
* set the Level to Information or Verbose

You can also view the log stream using Azure CLI

    $ func azure functionapp logstream StpFoodBlazor


## General

### bunit setup used to create this project

https://bunit.dev/docs/getting-started/create-test-project.html?tabs=xunit

Install bunit template for initializing new projects with bunit. This only needs to be done once

    $ dotnet new --install bunit.template

Create your new test project

    $ dotnet new bunit --framework xunit -o StpFoodBlazor/StpFoodBlazorTest

Link the project and the test project

    $ dotnet sln StpFoodBlazor.sln add StpFoodBlazor/StpFoodBlazorTest/StpFoodBlazorTest.csproj
    $ dotnet add StpFoodBlazor/StpFoodBlazorTest/StpFoodBlazorTest.csproj reference StpFoodBlazor/StpFoodBlazor.csproj

### Check for outdated packages

    $ dotnet list package --outdated
