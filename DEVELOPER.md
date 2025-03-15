
## Run

    $ cd StpFoodBlazor/StpFoodBlazor/
    $ dotnet run

## Test
Unit tests and Selenium functional tests are available.

https://learn.microsoft.com/en-us/aspnet/core/blazor/test?view=aspnetcore-8.0

    $ dotnet test --collect "XPlat Code Coverage"

If you don't have an instance of the app running on localhost you can exclude Selenium tests from the run

    $ dotnet test --filter FullyQualifiedName\!~StpFoodBlazorTest.Integration --collect "XPlat Code Coverage"

To view tests in HTML, install `reportgenerator` https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

    $ dotnet tool install -g dotnet-reportgenerator-globaltool

and add it to your path

    $ export PATH="$PATH:/Users/username/.dotnet/tools"

Then run the following to convert the XML into HTML in the `coveragereport` directory

Here's an example for Windows using an absolute path

    $ reportgenerator \
      -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml" \
      -targetdir:"coveragereport" \
      -reporttypes:Html

and here's one for other platforms showing you can run it from StpFoodBlazorTest

    $ reportgenerator \
      -reports:"./TestResults/{guid}/coverage.cobertura.xml" \
      -targetdir:"coveragereport" \
      -reporttypes:Html


### GitHub Actions locally using [act](https://github.com/nektos/act)
Not working the same on an M1 Mac as it is in GitHub, in particular I'm seeing Selenium tests fail.
You'll need to uncomment the job in test.yml that installs Chrome.

    act -W '.github/workflows/test.yml' --container-architecture linux/amd64 \
    --secret ASPNETCORE_APPCONFIG__SHEETSURL=$ASPNETCORE_APPCONFIG__SHEETSURL \
    --secret ASPNETCORE_APPCONFIG__SHEETID=$ASPNETCORE_APPCONFIG__SHEETID \
    -P ubuntu-latest=catthehacker/ubuntu:act-latest

## Azure

### Logging
When deployed on Azure, info-level logs can be available to stream. To enable:
* navigate to your application in https://portal.azure.com
* turn Application Logging (Filesystem) to On
* set the Level to Information or Verbose

Refer to appsettings.json for more granular control.

## General

### bunit setup

https://bunit.dev/docs/getting-started/create-test-project.html?tabs=xunit

Install bunit template for initializing new projects with bunit. This only needs to be done once

    $ dotnet new --install bunit.template

Create your new test project

    $ dotnet new bunit --framework xunit -o StpFoodBlazor/StpFoodBlazorTest

Link the project and the test project

    $ dotnet sln StpFoodBlazor.sln add StpFoodBlazor/StpFoodBlazorTest/StpFoodBlazorTest.csproj
    $ dotnet add StpFoodBlazor/StpFoodBlazorTest/StpFoodBlazorTest.csproj reference StpFoodBlazor/StpFoodBlazor.csproj

### Bootstrap icons

    $ cd StpFoodBlazor/StpFoodBlazor
    $ dotnet add package BootstrapIcons.AspNetCore --version 1.11.0
