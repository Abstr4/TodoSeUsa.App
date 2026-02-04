using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoSeUsa.Application;
using TodoSeUsa.BlazorServer;
using TodoSeUsa.BlazorServer.Components;
using TodoSeUsa.BlazorServer.Components.Account;
using TodoSeUsa.Infrastructure;
using TodoSeUsa.Infrastructure.Data;
using TodoSeUsa.Infrastructure.Hosting;
using TodoSeUsa.Infrastructure.Persistance;
using TodoSeUsa.Infrastructure.Security;

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

app.Run();