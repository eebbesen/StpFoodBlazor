using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging.AzureAppServices;
using StpFoodBlazor.Endpoints;
using StpFoodBlazor.Components;
using StpFoodBlazor.Middleware;
using StpFoodBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

builder.Configuration.AddEnvironmentVariables();

if (builder.Environment.IsProduction())
{
    // In production, use managed identity to access Key Vault
    var keyVaultUrl = new Uri(builder.Configuration["APPCONFIG:KEYVAULTURL"] ?? throw new InvalidOperationException("KeyVault:Url is not configured"));
    var credential = new DefaultAzureCredential();
    var secretClient = new SecretClient(keyVaultUrl, credential);

    try
    {
        KeyVaultSecret secret = await secretClient.GetSecretAsync("AppInsightsConnectionString");
        string connectionString = secret.Value;

        builder.Logging.AddApplicationInsights(
        configureTelemetryConfiguration: (config) =>
            config.ConnectionString = connectionString,
        configureApplicationInsightsLoggerOptions: (options) => {
            options.IncludeScopes = true;
            options.TrackExceptionsAsExceptionTelemetry = true;
        });
    }
    catch (Exception ex)
    {
        // Log the exception but allow the app to start without Application Insights
        builder.Logging.AddConsole();
        var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger("Program");
        logger.LogError(ex, "Failed to retrieve Application Insights connection string from Key Vault");
    }
}
else
{
    // In development, use user secrets or environment variables
    // dotnet user-secrets set "ApplicationInsights:ConnectionString" "your-connection-string"
}

builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "applogs-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 5;
});

builder.Services.Configure<AzureBlobLoggerOptions>(options =>
{
    options.BlobName = "logs.txt";
});

builder.Logging.AddConsole(
    configure: (options) => {}
);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITimeService, TimeService>();

builder.Services.AddHttpClient<HttpDealService>().AddStandardResilienceHandler();
builder.Services.AddHttpClient<HttpGiftCardService>().AddStandardResilienceHandler();
builder.Services.AddHttpClient<HttpHolidayService>().AddStandardResilienceHandler();
builder.Services.AddScoped<IHolidayService>(sp => sp.GetRequiredService<HttpHolidayService>());
builder.Services.AddScoped<ICacheRefreshService, CacheRefreshService>();

builder.Services.AddScoped<IDealService>(sp => sp.GetRequiredService<HttpDealService>());
builder.Services.AddScoped<IGiftCardService>(sp => sp.GetRequiredService<HttpGiftCardService>());
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<PageAccessLoggingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .WithStaticAssets();

CacheEndpoints.Map(app);

app.Run();
