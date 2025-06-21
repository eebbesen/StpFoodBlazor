using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.AzureAppServices;
using StpFoodBlazor.Components;
using StpFoodBlazor.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (builder.Environment.IsProduction())
{
    // In production, use managed identity to access Key Vault
    var keyVaultUrl = new Uri("https://feastival.vault.azure.net/");
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
        // Log the exception, but don't expose sensitive details
        builder.Logging.AddConsole();
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
        logger.LogError(ex, "Failed to retrieve Application Insights connection string from Key Vault");
        throw;
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
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IDealService, HttpDealService>();
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IGiftCardService, HttpGiftCardService>();
builder.Services.AddScoped<IHolidayService, HttpHolidayService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var nonceBytes = new byte[16];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(nonceBytes);
    }
    var nonce = WebEncoders.Base64UrlEncode(nonceBytes);

    context.Items["csp-nonce"] = nonce;

    context.Response.Headers.Append(
        "Content-Security-Policy",
        $"default-src 'self'; " +
        $"script-src 'self'; " +
        $"style-src 'self' 'nonce-{nonce}' 'unsafe-hashes' " +
        $"'sha256-phSae2Ud+nJs666rsURzxXg7FV5Tg7c+iiSFDGd3tAw=' " +
        $"'sha256-oLiTjTy/4afiaW/t7b0OVz122l2am89Dh+080MmksZM='; " +
        $"img-src 'self' data:; " +
        $"font-src 'self' data:; " +
        $"connect-src 'self';" +
        $"frame-ancestors 'none';" +
        $"form-action 'self'; " +
        $"base-uri 'self'; " +
        $"object-src 'none';" +
        $"frame-src 'self'; " +
        $"media-src 'self'; " +
        $"manifest-src 'self'");

    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

    await next();
});

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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
