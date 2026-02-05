using System.Diagnostics;
using TodoSeUsa.Application;
using TodoSeUsa.BlazorServer;
using TodoSeUsa.BlazorServer.Components;
using TodoSeUsa.BlazorServer.Components.Account;
using TodoSeUsa.BlazorServer.UI.Services;
using TodoSeUsa.Infrastructure;
using TodoSeUsa.Infrastructure.Hosting;
using TodoSeUsa.Infrastructure.Persistance;

// Prevents multiple instances of the app from running
if (!SingleInstanceGuard.TryAcquire("TodoSeUsa.Offline.SingleInstance"))
    return;

var builder = WebApplication.CreateBuilder(args);

var appUrl = "http://localhost:5050";
builder.WebHost.UseUrls(appUrl);
var paths = AppPathInitializer.Initialize(builder);

builder.ConfigureProductionHosting(paths);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddSecureSession();

builder.Services.AddHostedService(sp =>
{
    var lifetime = sp.GetRequiredService<IHostApplicationLifetime>();
    return new TrayIconService(lifetime, appUrl);
});

builder.AddInfrastructureServices();
builder.AddApplicationServices();
builder.AddWebServices();

var app = builder.Build();

// 4. Middleware Pipeline
app.UseSession();

await DatabaseInitializer.InitializeAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
}

app.UseStorageFiles(paths.Storage);
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

// Auto-launch browser in Production
if (app.Environment.IsProduction())
{
    Process.Start(new ProcessStartInfo
    {
        FileName = appUrl,
        UseShellExecute = true
    });
}

app.Run();