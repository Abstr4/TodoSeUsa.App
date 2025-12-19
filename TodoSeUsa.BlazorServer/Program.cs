using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TodoSeUsa.Application;
using TodoSeUsa.BlazorServer;
using TodoSeUsa.BlazorServer.Components;
using TodoSeUsa.Infrastructure;
using TodoSeUsa.Infrastructure.Data;
using TodoSeUsa.Infrastructure.Persistance.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var storageRoot = Path.Combine(
    builder.Environment.ContentRootPath,
    "Storage",
    "ProductImages"
);

Directory.CreateDirectory(storageRoot);

builder.Configuration["Storage:BasePath"] = storageRoot;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        var seeder = services.GetRequiredService<DbSeeder>();
        await seeder.SeedAsync(context);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        builder.Configuration["Storage:BasePath"]!
    ),
    RequestPath = "/files"
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
