using System.Diagnostics;
using TodoSeUsa.Application;
using TodoSeUsa.BlazorServer;
using TodoSeUsa.BlazorServer.Components;
using TodoSeUsa.BlazorServer.Components.Account;
using TodoSeUsa.Infrastructure;
using TodoSeUsa.Infrastructure.Hosting;
using TodoSeUsa.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

var paths = AppPathInitializer.Initialize(builder);

builder.ConfigureProductionHosting(paths);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddSecureSession();

builder.AddInfrastructureServices();
builder.AddApplicationServices();
builder.AddWebServices();

var app = builder.Build();

app.UseSession();

await DatabaseInitializer.InitializeAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

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
    const string url = "https://localhost:5050";

    Process.Start(new ProcessStartInfo
    {
        FileName = url,
        UseShellExecute = true
    });
}

app.Run();