using System.Diagnostics;
using TodoSeUsa.Application;
using TodoSeUsa.BlazorServer;
using TodoSeUsa.BlazorServer.Components;
using TodoSeUsa.BlazorServer.Components.Account;
using TodoSeUsa.BlazorServer.UI.Services;
using TodoSeUsa.Infrastructure;
using TodoSeUsa.Infrastructure.Hosting;
using TodoSeUsa.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment() && !SingleInstanceGuard.TryAcquire("TodoSeUsa.Offline.SingleInstance"))
    return;

string appUrl = string.Empty;
if (builder.Environment.IsProduction())
{
    appUrl = "http://localhost:5050";
    builder.WebHost.UseUrls(appUrl);

    builder.Services.AddHostedService(sp =>
    {
        var lifetime = sp.GetRequiredService<IHostApplicationLifetime>();
        return new TrayIconService(lifetime, appUrl);
    });
}

var paths = AppPathInitializer.Initialize(builder);
builder.ConfigureProductionHosting(paths);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.AddInfrastructureServices();
builder.AddApplicationServices();
builder.AddWebServices();

builder.Services.AddSecureSession();

var app = builder.Build();

app.UseSession();
await DatabaseInitializer.InitializeAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

if (app.Environment.IsProduction())
{
    Process.Start(new ProcessStartInfo
    {
        FileName = appUrl,
        UseShellExecute = true
    });
}

app.Run();