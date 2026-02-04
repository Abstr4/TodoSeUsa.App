using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace TodoSeUsa.Infrastructure.Hosting;

public static class SessionConfiguration
{
    public static void AddSecureSession(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.IdleTimeout = TimeSpan.FromMinutes(5);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
    }
}