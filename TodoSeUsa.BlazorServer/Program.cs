using Microsoft.EntityFrameworkCore;
using Radzen;
using TodoSeUsa.Application;
using TodoSeUsa.BlazorServer.Components;
using TodoSeUsa.Infrastructure;
using TodoSeUsa.Infrastructure.Data;
using TodoSeUsa.Infrastructure.Persistance.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "TodoSeUsaApplicationTheme";
    options.Duration = TimeSpan.FromDays(365);
});

builder.AddApplicationServices();
builder.AddInfrastructureServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
    if (app.Environment.IsDevelopment())
    {
        await DbSeeder.SeedAsync(context);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
