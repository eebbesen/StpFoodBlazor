
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
dotnet new bunit --framework xunit -o <NAME OF TEST PROJECT>
```

Link the project and the test project
```bash
dotnet sln blazor_tut.generated.sln add blazor_tut_test
dotnet add blazor_tut_test.csproj reference blazor_tut.csproj
```


# General

### Setup

    $ dotnet sln <NAME OF PROJECT>.sln add <NAME OF TEST PROJECT>
    $ dotnet add <NAME OF TEST PROJECT>.csproj reference <NAME OF COMPONENT PROJECT>.csproj


