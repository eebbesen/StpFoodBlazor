
### Run

    $ cd StpFoodBlazor/StpFoodBlazor/
    $ dotnet run

### Testing

https://learn.microsoft.com/en-us/aspnet/core/blazor/test?view=aspnetcore-8.0


#### bunit setup

https://bunit.dev/docs/getting-started/create-test-project.html?tabs=xunit

Install bunit template for initializing new projects with bunit. This only needs to be done once
```bash
dotnet new --install bunit.template
```

Create your new test project
```bash
dotnet new bunit --framework xunit -o StpFoodBlazor/StpFoodBlazorTest
```

Link the project and the test project
```bash
dotnet sln StpFoodBlazor.sln add StpFoodBlazor/StpFoodBlazorTest/StpFoodBlazorTest.csproj
dotnet add StpFoodBlazor/StpFoodBlazorTest/StpFoodBlazorTest.csproj reference StpFoodBlazor/StpFoodBlazor.csproj
```

# General

### Setup
