using Microsoft.AspNetCore.Hosting;

namespace TodoSeUsa.Infrastructure.Hosting;

public static class KestrelExtensions
{
    public static void ConfigureHttps(this IWebHostBuilder host)
    {
        host.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5050);
            }
        );
    }
}