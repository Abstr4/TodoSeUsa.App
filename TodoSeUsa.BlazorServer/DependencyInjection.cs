using Radzen;
using TodoSeUsa.BlazorServer.UI.Notifications;
using TodoSeUsa.BlazorServer.UI.State;

namespace TodoSeUsa.BlazorServer;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        var storageRoot = Path.Combine(
            builder.Environment.ContentRootPath,
            "Storage"
        );

        Directory.CreateDirectory(storageRoot);

        builder.Configuration["Storage:BasePath"] = storageRoot;


        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddRadzenComponents();

        builder.Services.AddRadzenCookieThemeService(options =>
        {
            options.Name = "TodoSeUsaApplicationTheme";
            options.Duration = TimeSpan.FromDays(365);
        });

        builder.Services.AddScoped<SimpleNotifications>();
        builder.Services.AddScoped<RecoveryCodeSession>();
    }
}