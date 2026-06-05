FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY StpFoodBlazor/StpFoodBlazor.csproj StpFoodBlazor/
RUN dotnet restore StpFoodBlazor/StpFoodBlazor.csproj

COPY StpFoodBlazor/ StpFoodBlazor/
RUN dotnet publish StpFoodBlazor/StpFoodBlazor.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "StpFoodBlazor.dll"]
