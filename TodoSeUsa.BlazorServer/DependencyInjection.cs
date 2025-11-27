using CurrieTechnologies.Razor.SweetAlert2;
using Radzen;
using TodoSeUsa.BlazorServer.Helpers;

namespace TodoSeUsa.BlazorServer;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddRadzenComponents();

        builder.Services.AddRadzenCookieThemeService(options =>
        {
            options.Name = "TodoSeUsaApplicationTheme";
            options.Duration = TimeSpan.FromDays(365);
        });

        builder.Services.AddSweetAlert2();
        builder.Services.AddScoped<SimpleNotifications>();
    }
}