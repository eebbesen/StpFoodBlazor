
## Run

    $ cd StpFoodBlazor/StpFoodBlazor/
    $ dotnet run

## Test

https://learn.microsoft.com/en-us/aspnet/core/blazor/test?view=aspnetcore-8.0

    $ dotnet test --collect "XPlat Code Coverage"

To view tests in HTML, install `reportgenerator` https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

    $ dotnet tool install -g dotnet-reportgenerator-globaltool

and add it to your path

    $ export PATH="$PATH:/Users/username/.dotnet/tools"

Then run the following to convert the XML into HTML in the `coveragereport` directory

    $ reportgenerator \
      -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml" \
      -targetdir:"coveragereport" \
      -reporttypes:Html


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
