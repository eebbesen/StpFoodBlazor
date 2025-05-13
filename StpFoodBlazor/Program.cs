using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.AzureAppServices;
using StpFoodBlazor.Components;
using StpFoodBlazor.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

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

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) =>
        config.ConnectionString = builder.Configuration.GetConnectionString("AppInsights"),
        configureApplicationInsightsLoggerOptions: (options) => { }
);

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
    // Generate a cryptographically secure random nonce
    var nonceBytes = new byte[16];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(nonceBytes);
    }
    var nonce = WebEncoders.Base64UrlEncode(nonceBytes);

    // Store nonce in HttpContext.Items for retrieval in components
    context.Items["csp-nonce"] = nonce;

    // Set CSP header with nonce
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
        $"object-src 'none';");

    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
