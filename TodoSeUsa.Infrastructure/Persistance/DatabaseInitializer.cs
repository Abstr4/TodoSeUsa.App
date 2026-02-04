using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoSeUsa.Infrastructure.Persistance.Seed;

namespace TodoSeUsa.Infrastructure.Persistance;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        WebApplication app
    )
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        if (app.Environment.IsDevelopment())
        {
            var seeder = services.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync(context);
        }
    }
}